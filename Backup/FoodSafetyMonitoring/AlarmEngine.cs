using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBUtility;
using System.Threading;
using System.Data;

namespace FoodSafetyMonitoring
{
    class AlarmEngine
    {
        DbHelperMySQL dbHelper = null;
        private Thread thread;
        private bool isOver = false;//线程是否终止
        public delegate void AlarmEventHandler(List<AlarmData> msgDataList);
        public event AlarmEventHandler AlarmEvent;

        public AlarmEngine(DbHelperMySQL dbHelper)
        {
            this.dbHelper = dbHelper;
        }

        /// <summary>
        /// 启动Alarm线程
        /// </summary>
        public void Start()
        {
            if (thread == null)
            {
                thread = new Thread(new ThreadStart(GetAlarmData));
            }
            thread.Start();
        }
        /// <summary>
        /// 关闭线程
        /// </summary>
        public void Close()
        {
            this.isOver = true;
        }

        /// <summary>
        /// 从数据库中获取报警信息
        /// </summary>
        private void GetAlarmData()
        {
            int aaa = -2;
            while (!isOver)
            {
                //获取告警信息
                List<string> alarmList = GetAlarmMsg();
                string[] alarmSplit = { "@#" };
                if (alarmList != null)
                {
                    List<AlarmData> msgDataList = new List<AlarmData>();
                    foreach (string kv in alarmList)
                    {
                        string[] values = kv.Split(alarmSplit, StringSplitOptions.None);
                        if (values.Length == 7)
                        {
                            string id = values[0];
                            string cardid = values[1];
                            string Name = values[2];
                            string locatorid = values[3];
                            string time = values[4];
                            string alarmType = values[5];
                            string areaName = values[6];

                            AlarmData msg = new AlarmData(id, cardid, Name, locatorid, areaName, time, alarmType);
                            msgDataList.Add(msg);

                        }
                    }
                    if (AlarmEvent != null)
                    {
                        AlarmEvent(msgDataList);
                    }
                }
                Thread.Sleep(4000);
            }
        }

        public List<string> GetAlarmMsg()
        {
            List<string> list = new List<string>();
            string str_sq = "SELECT a.NUMB_ALARM AS OID,a.FK_DEVICE_CODE AS MSFNBR,B.PERSON_NAME AS PersonName,c.INFO_LOACTIONNAME AS AREANAME,a.FK_INFO_NAME AS ALERM_TYPE,a.ALARM_DATETIME as CREATIME,a.FK_DEVICE_LOCATION AS BSFNBR FROM sys_alarm_data AS a LEFT JOIN sys_client_person AS b ON a.FK_DEVICE_CODE=b.FK_DEVICE_CODE LEFT JOIN sys_client_device AS c ON a.FK_DEVICE_LOCATION=c.INFO_CODE where a.ALARM_STATE='1'";
            DataSet ds = dbHelper.GetDataSet(str_sq);
            if (ds != null)
            {
                foreach (DataRow tr in ds.Tables[0].Rows)
                {
                    string re_value = tr["OID"].ToString() + "@#" + tr["MSFNBR"].ToString() + "@#" + tr["PersonName"].ToString() + "@#" + tr["BSFNBR"].ToString() + "@#" + tr["CREATIME"].ToString() + "@#" + tr["ALERM_TYPE"].ToString() + "@#" + tr["AREANAME"].ToString();
                    list.Add(re_value);
                }
            }
            return list;
        }
    }
}
