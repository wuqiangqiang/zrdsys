using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBUtility;

using System.Data;

namespace FoodSafetyMonitoring.dao
{
    class DBOperation_MySQL : IDBOperation
    {
        private DbHelperMySQL dbHelper = DbHelperMySQL.CreateDbHelper();
        private string userId;

        public DBOperation_MySQL()
        {

        }

        #region IDBOperation 成员

        public DbHelperMySQL GetDbHelper()
        {
            return dbHelper;
        }

        //根据账户名和密码,或者ID,用户名,菜单权限
        public UserInfo GetMenu(string loginName, string password)
        {
            UserInfo userInfo = new UserInfo();

            string sql = string.Format("select reco_pkid uid, INFO_USER uname, fk_dept deptid,FLAG_TIER flagtier,supplierId, SUB_NAME menu " +
                                       "from v_user_sub " +
                                       "where NUMB_USER ='{0}' and  INFO_PASSWORD = '{1}'", loginName, password);


            try
            {
                DataTable table = dbHelper.GetDataSet(sql).Tables[0];
                if (table.Rows.Count == 1)
                {
                    userId = table.Rows[0][0].ToString();
                    userInfo.ID = table.Rows[0][0].ToString();
                    userInfo.ShowName = table.Rows[0][1].ToString();
                    userInfo.DepartmentID = table.Rows[0][2].ToString();
                    userInfo.FlagTier = table.Rows[0][3].ToString();
                    userInfo.SupplierId = table.Rows[0][4].ToString();
                    userInfo.Menus.AddRange(table.Rows[0][5].ToString().Split(new char[] { ',' }));
                }
                return userInfo;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("GetMenu异常");
                throw new Exception(e.Message);
            }
        }

        //根据用户ID获取树型部门列表
        public DataTable GetDepartment()
        {
            string sql = string.Format("SELECT * from sys_client_sysdept where INFO_CODE like concat((select FK_dept from sys_client_user where reco_pkid='{0}'),'%')", userId);
            try
            {
                return dbHelper.GetDataSet(sql).Tables[0];
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("GetDepartment异常");
                throw new Exception(e.Message);
            }
        }

        //获取省市信息
        public DataTable GetProvinceCity()
        {
            string sql = "select id,name,pid from sys_city";
            try
            {
                return dbHelper.GetDataSet(sql).Tables[0];
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("GetProvinceCity异常");
                throw new Exception(e.Message);
            }
        }

        public DataTable GetComparisonAndAnalysiseData(string theme, DateTime startTime, DateTime endTime)
        {
            string sql = string.Format("select t.info_name,t.report_count " +
                "from (select info_name,count(*) report_count " +
                "from (select INFO_CODE,INFO_NAME " +
                "from sys_client_sysdept where INFO_CODE like '{0}%' and flag_tier = 4) m " +
                "INNER JOIN t_detect_report n on m.INFO_CODE = n.DETECTID and " +
                "TO_DAYS(DETECTDATE) BETWEEN TO_DAYS('{1}') and TO_DAYS('{2}')  GROUP BY info_code) t ",
                this.userId, startTime.ToShortDateString(), endTime.ToShortDateString());
            try
            {
                return dbHelper.GetDataSet(sql).Tables[0];
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("GetComparisonAndAnalysiseData异常");
                throw new Exception(e.Message);
            }
        }
 
        public DataTable GetCompany()
        {
            try
            {
                string sql = string.Format("call p_user_company_details( '{0}')", userId);
                return dbHelper.GetDataSet(sql).Tables[0];
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("GetComparisonAndAnalysiseData异常");
                throw new Exception(e.Message);
            }
        }

        #endregion
    }
}
