using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBUtility;
using System.Data;

namespace FoodSafetyMonitoring.dao
{
    public interface IDBOperation
    {
        DbHelperMySQL GetDbHelper();
        /// <summary>
        /// 根据账号密码获取信息
        /// </summary>
        /// <param name="loginName">登录名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        UserInfo GetMenu(string loginName,string password);
        /// <summary>
        /// 获取部门
        /// </summary>
        /// <returns></returns>
        DataTable GetDepartment();
        /// <summary>
        /// 获取省市结构表
        /// </summary>
        /// <returns></returns>
        DataTable GetProvinceCity();
        /// <summary>
        /// 获取对比分析数据
        /// </summary>
        /// <param name="theme">主题</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">截止时间</param>
        /// <returns></returns>
        DataTable GetComparisonAndAnalysiseData(string theme, DateTime startTime, DateTime endTime);
        /// <summary>
        /// 获取来源单位
        /// </summary>
        /// <returns></returns>
        DataTable GetCompany();

        

    }

    public class UserInfo
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public DateTime LastLoginTime { get; set; }

        public string LoginName { get; set; }

        public string ID { get; set; }

        public string ShowName { get; set; }

        public string DepartmentID { get; set; }

        public string FlagTier { get; set; }

        public string SupplierId { get; set; }

        private List<string> menus = new List<string>();

        public List<string> Menus
        {
            get { return menus; }
            set { menus = value; }
        }
    }

}
