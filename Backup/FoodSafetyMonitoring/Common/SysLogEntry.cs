using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoodSafetyMonitoring.Common
{
    class SysLogEntry
    {
        public static void WriteLog(string menu, string userName, OperationType operationType, string content)
        {
            string type = string.Empty;
            switch (operationType)
            {
                case OperationType.Login:
                    type = "登录";
                    break;
                case OperationType.Add:
                    type = "添加";
                    break;
                case OperationType.Modify:
                    type = "修改";
                    break;
                case OperationType.Delete:
                    type = "删除";
                    break;
                case OperationType.AddAndModify:
                    type = "添加和修改";
                    break;
                case OperationType.AddAndDelete:
                    type = "添加和删除";
                    break;
                case OperationType.ModifyAndDelete:
                    type = "修改和删除";
                    break;
                case OperationType.All:
                    type = "全部";
                    break;
            }
            string strSql = string.Format(@"insert into sys_client_syslog (FK_NAME_MENU,FLAG_LOGSORT,FK_NAME_USER,INFO_CONT,INFO_DATE) values 
                                           ('{0}','{1}','{2}','{3}','{4}')", menu, type, userName, content, DateTime.Now);
            DBUtility.DbHelperMySQL dbHelper = DBUtility.DbHelperMySQL.CreateDbHelper();
            try
            {
                dbHelper.ExecuteSql(strSql);
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}
