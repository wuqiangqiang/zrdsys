using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace DBUtility
{
    public class ConnectionString
    {
        /// <summary>
        /// 配置文件里的连接名,默认‘ConnectionInfo’
        /// </summary>
        private string _connectionConfigurationName = "ConnectionInfo";

        /// <summary>
        /// 配置文件里的连接名
        /// </summary>
        public string ConnectionConfigurationName
        {
            get { return _connectionConfigurationName; }
            set { _connectionConfigurationName = value; }
        }

        /// <summary>
        /// 获取配置文件中的连接字符串
        /// </summary>
        /// <param name="type">数据库的类型</param>
        /// <returns>数据库连接字符串</returns>
        public string GetConnString()
        {
            string strConn = string.Empty;
            try
            {
                strConn = ConfigurationManager.ConnectionStrings[ConnectionConfigurationName].ConnectionString;
            }
            catch (Exception)
            {
                throw new Exception("未能找到配置文件连接字符串！");
            }
            return strConn;
        }
    }
}
