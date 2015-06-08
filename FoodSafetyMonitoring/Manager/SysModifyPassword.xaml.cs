using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DBUtility;
using System.Security.Cryptography;
using Toolkit = Microsoft.Windows.Controls;
using FoodSafetyMonitoring.Common;
using FoodSafetyMonitoring.dao;

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// SysModifyPassword.xaml 的交互逻辑
    /// </summary>
    public partial class SysModifyPassword : UserControl
    {
        private DbHelperMySQL dbHelper = null;

        public SysModifyPassword()
        {
            dbHelper = DbHelperMySQL.CreateDbHelper();

            InitializeComponent();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (this._password_old.Password.Trim() == "")
            {
                txtMsg.Text = "*原密码不能为空！";
                return;
            }

            if (this._password.Password.Trim() == "")
            {
                txtMsg.Text = "*修改密码不能为空！";
                return;
            }

            if (this._password_2.Password.Trim() == "")
            {
                txtMsg.Text = "*确认密码不能为空！";
                return;
            }

            if (this._password.Password != this._password_2.Password)
            {
                txtMsg.Text = "*修改密码和确认密码不一致！";
                return;
            }

            MD5 md5 = new MD5CryptoServiceProvider();
            string password_old = BitConverter.ToString(md5.ComputeHash(Encoding.Default.GetBytes(this._password_old.Password))).Replace("-", "");
            bool exit_flag = dbHelper.Exists(string.Format("SELECT count(RECO_PKID) from sys_client_user where RECO_PKID ='{0}' and INFO_PASSWORD = '{1}'", (Application.Current.Resources["User"] as UserInfo).ID, password_old));
            if (!exit_flag)
            {
                Toolkit.MessageBox.Show("原密码输入错误，请重新输入！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string password = BitConverter.ToString(md5.ComputeHash(Encoding.Default.GetBytes(this._password.Password))).Replace("-", "");

            string strSql = string.Format(@"UPDATE sys_client_user SET INFO_PASSWORD = '{0}'  WHERE RECO_PKID = {1}",password,(Application.Current.Resources["User"] as UserInfo).ID);

            try
            {
                int num = dbHelper.ExecuteSql(strSql);
                if (num == 1)
                {

                    Toolkit.MessageBox.Show("保存成功！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    Common.SysLogEntry.WriteLog("系统用户管理", (Application.Current.Resources["User"] as UserInfo).ShowName, OperationType.Add, "修改密码");
                    this._password.Password = "";
                    this._password_2.Password = "";
                    this._password_old.Password = "";
                }
                else
                {
                    Toolkit.MessageBox.Show("保存失败！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
            }
            catch (Exception)
            {
                Toolkit.MessageBox.Show("保存失败！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
        }
    }
}
