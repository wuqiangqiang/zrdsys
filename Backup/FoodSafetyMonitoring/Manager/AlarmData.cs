using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBUtility;
using System.Collections;

namespace FoodSafetyMonitoring
{
    public class AlarmData : IEnumerable
    {
        public string Alarmid { get; set; }
        public bool Bdealwith { get; set; }
        public string Name { get; set; }
        public string Locatorid { get; set; }
        public string Time { get; set; }
        public string Alarmname { get; set; }
        public string Areaname { get; set; }
        public string Cardid { get; set; }
        public bool bdealwith = false;
        public AlarmData(string id, string cardid, string name, string locatorid, string areaname, string time, string alarmname)
        {
            this.Alarmid = id;
            this.Locatorid = locatorid;
            this.Cardid = cardid;
            this.Time = time;
            this.Alarmname = alarmname;
            this.Areaname = areaname;
            this.Name = name;
        }



        public bool DealWithAlarm(DbHelperMySQL dbHelper)
        {
            bool bok = false;
            if (DealwithAlarmMsg(this.Alarmid, dbHelper))
            {
                bdealwith = true;
                bok = true;
            }
            return bok;
        }

        private bool DealwithAlarmMsg(string id, DbHelperMySQL dbHelper)
        {
            bool r_bool = false;
            string str_sql = "UPDATE sys_alarm_data SET ALARM_STATE='0' WHERE NUMB_ALARM=" + id;
            try
            {
                dbHelper.ExecuteSql(str_sql);
                r_bool = true;
            }
            catch (System.Exception e)
            {
                r_bool = false;
            }

            return r_bool;
        }

        #region IEnumerable 成员

        public IEnumerator GetEnumerator()
        {
            return (IEnumerator)this;
        }

        #endregion
    }
}
