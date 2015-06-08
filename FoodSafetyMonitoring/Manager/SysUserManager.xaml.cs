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
using Toolkit = Microsoft.Windows.Controls;
using DBUtility;
using System.Data;
using FoodSafetyMonitoring.Common;
using FoodSafetyMonitoring.dao;
using System.Security.Cryptography;

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// Test.xaml 的交互逻辑
    /// </summary>
    public partial class SysUserManager : UserControl
    {
        private DbHelperMySQL dbHelper = null;
        private string user_flag_tier;
        private string password_old;

        public SysUserManager()
        {
            InitializeComponent();
            user_flag_tier = (Application.Current.Resources["User"] as UserInfo).FlagTier;

            this.Loaded += new RoutedEventHandler(Test_Loaded);
        }

        void Test_Loaded(object sender, RoutedEventArgs e)
        {
            BindData();
            Clear();
            ComboboxTool.InitComboboxSource(_department, "call p_dept_count(" + (Application.Current.Resources["User"] as UserInfo).ID + ")", "lr");
            ComboboxTool.InitComboboxSource(_cmbRoleType, "SELECT NUMB_ROLE,INFO_NAME FROM sys_client_role", "lr");
            _department.SelectionChanged += new SelectionChangedEventHandler(_department_SelectionChanged);
        }

        /// <summary>
        /// 从数据库获取数据
        /// </summary>
        private void BindData()
        {
            string strSql = "select RECO_PKID,NUMB_USER,sys_client_user.INFO_USER,sys_client_sysdept.INFO_NAME,sys_client_role.INFO_NAME as role_expl " +
                            "FROM sys_client_user ,sys_client_sysdept,sys_client_role " +
                            "WHERE sys_client_user.fk_dept = sys_client_sysdept.INFO_CODE " +
                            "and sys_client_user.ROLE_ID = sys_client_role.NUMB_ROLE " +
                            "and sys_client_user.cuserid = " + (Application.Current.Resources["User"] as UserInfo).ID;

            try
            {
                dbHelper = DbHelperMySQL.CreateDbHelper();
                DataTable dt = dbHelper.GetDataSet(strSql).Tables[0];
                lvlist.DataContext = null;
                lvlist.DataContext = dt;
                lvlist.Tag = dt;
            }
            catch (Exception)
            {
                return;
            }
        }

        //private void TextboxSearchControl_ImageClick(object sender, EventArgs e)
        //{
        //    SelectUser(txtSearch.Text.Trim(), "");
        //}

        /// <summary>
        /// 从内存表中查询用户
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="userType">角色类型</param>
        private void SelectUser(string userName, string userType)
        {
            StringBuilder sbSql = new StringBuilder();
            if (userName != "")
            {
                sbSql.Append("INFO_USER like '%" + userName + "%'");
                if (userType != "")
                {
                    sbSql.Append(" AND ROLE_ID = '" + userType + "'");
                }
            }
            else
            {
                if (userType != "")
                {
                    sbSql.Append(" ROLE_ID = '" + userType + "'");
                }
            }

            if (sbSql.ToString() != "")
            {
                DataRow[] drs = (lvlist.Tag as DataTable).Select(sbSql.ToString());
                DataTable temp = (lvlist.Tag as DataTable).Clone();
                foreach (DataRow row in drs)
                {
                    DataRow dr = temp.NewRow();
                    dr.ItemArray = row.ItemArray;
                    temp.Rows.Add(dr);
                }
                lvlist.DataContext = temp;
            }
            else
            {
                lvlist.DataContext = (lvlist.Tag as DataTable);
            }
        }

        void _department_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_department.SelectedIndex > 0)
            {
                switch (user_flag_tier)
                {
                    case "0": _cmbRoleType.Text = "省级管理员";
                        _cmbRoleType.IsEnabled = false;
                        _subDetails.Text = dbHelper.GetSingle("select GROUP_CONCAT(SUB_NAME) as SUB_NAME from v_sub_details where ROLE_ID = '79' and SUB_FATHER_ID = '0'").ToString();
                        break;
                    case "1": _cmbRoleType.Text = "地市级管理员";
                        _cmbRoleType.IsEnabled = false;
                        _subDetails.Text = dbHelper.GetSingle("select GROUP_CONCAT(SUB_NAME) as SUB_NAME from v_sub_details where ROLE_ID = '76' and SUB_FATHER_ID = '0'").ToString();
                        break;
                    case "2": _cmbRoleType.Text = "区县级管理员";
                        _cmbRoleType.IsEnabled = false;
                        _subDetails.Text = dbHelper.GetSingle("select GROUP_CONCAT(SUB_NAME) as SUB_NAME from v_sub_details where ROLE_ID = '77' and SUB_FATHER_ID = '0'").ToString();
                        break;
                    case "3": string flag = dbHelper.GetSingle(string.Format("SELECT FLAG_TIER FROM sys_client_sysdept where INFO_CODE ='{0}'",(_department.SelectedItem as Label).Tag)).ToString();
                        if (flag == "3")
                        {
                            _cmbRoleType.Text = "复核检测师";
                            _cmbRoleType.IsEnabled = false;
                            _subDetails.Text = dbHelper.GetSingle("select GROUP_CONCAT(SUB_NAME) as SUB_NAME from v_sub_details where ROLE_ID = '80' and SUB_FATHER_ID = '0'").ToString();
                        }
                        else if (flag == "4")
                        {
                            _cmbRoleType.Text = "检测师";
                            _cmbRoleType.IsEnabled = false;
                            _subDetails.Text = dbHelper.GetSingle("select GROUP_CONCAT(SUB_NAME) as SUB_NAME from v_sub_details where ROLE_ID = '60' and SUB_FATHER_ID = '0'").ToString();
                        }
                        //ComboboxTool.InitComboboxSource(_cmbRoleType, "SELECT NUMB_ROLE,INFO_NAME FROM sys_client_role where NUMB_ROLE = '60' or NUMB_ROLE = '80'", "lr");
                        //_cmbRoleType.SelectionChanged += new SelectionChangedEventHandler(_cmbRoleType_SelectionChanged);
                        //_cmbRoleType.IsEnabled = true;
                        break;
                    default: break;
                }
            }
        }

        //void _cmbRoleType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (_cmbRoleType.SelectedIndex > 0)
        //    {
        //        _subDetails.Text = dbHelper.GetSingle(string.Format("select GROUP_CONCAT(SUB_NAME) as SUB_NAME from v_sub_details where ROLE_ID = '{0}' and SUB_FATHER_ID = '0'", (_cmbRoleType.SelectedItem as Label).Tag)).ToString();
        //    }
        //}



        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            user_details.Visibility = Visibility.Visible;
            btnSave.Visibility = Visibility.Visible;
            btnCancel.Visibility = Visibility.Visible;
            Clear();
            this._department.IsEnabled = true;
            this._loginName.IsEnabled = true;
            this._loginPassword.IsEnabled = true;
            this.txtUserName.IsEnabled = true;
            this._user_manger.IsEnabled = true;
            this._user_manger_2.IsEnabled = true;
            this._user_manger.IsChecked = true;
            this._dept_flag.Text = "(必填)";
            //this._role_flag.Text = "(必填)";
            this._user_flag.Text = "(必填)";
            this._password_flag.Text = "(必填)";
            this._name_flag.Text = "(必填)";
            this._manager_flag.Text = "(必填)";

        }


        private void FrameworkElement_GotFocus(object sender, RoutedEventArgs e)
        {
            ClearTip(sender);
        }

        private void Clear()
        {
            this._department.SelectedIndex = 0;
            this._cmbRoleType.SelectedIndex = 0;
            this._subDetails.Text = "";
            this._loginName.Text = "";
            this._loginPassword.Password = "";
            this.txtUserName.Text = "";
            this._user_manger.IsChecked = false;
            this._user_manger_2.IsChecked = false;
            this.txtMsg.Text = "";
            this._department.IsEnabled = false;
            this._cmbRoleType.IsEnabled = false;
            this._subDetails.IsEnabled = false;
            this._loginName.IsEnabled = false;
            this._loginPassword.IsEnabled = false;
            this.txtUserName.IsEnabled = false;
            this._user_manger.IsEnabled = false;
            this._user_manger_2.IsEnabled = false;
            this.btnSave.Tag = null;
            this._dept_flag.Text = "";
            //this._role_flag.Text = "";
            this._user_flag.Text = "";
            this._password_flag.Text = "";
            this._name_flag.Text = "";
            this._manager_flag.Text = "";
            //this.txtUserName.Focus();
        }

        #region 删除用户

        private bool DeleteUser(string id)
        {
            bool result = false;
            string strSql = string.Format("delete from sys_client_user where RECO_PKID=" + id);
            try
            {
                int num = dbHelper.ExecuteSql(strSql);
                if (num == 1)
                {
                    result = true;
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }
        #endregion



        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string user_manager_flag = "";
            if (this._department.SelectedIndex < 1)
            {
                txtMsg.Text = "*请选择帐号使用单位！";
                txtMsg.Tag = "_department";
                return;
            }

            if (this._cmbRoleType.SelectedIndex < 1)
            {
                txtMsg.Text = "*请选择帐号权限！";
                txtMsg.Tag = "_cmbRoleType";
                return;
            }
            if (this._loginName.Text.Trim() == "")
            {
                txtMsg.Text = "*请输入登录帐号！";
                txtMsg.Tag = "_loginName";
                return;
            }

            if (this._loginPassword.Password.Trim() == "")
            {
                txtMsg.Text = "*请输入密码！";
                txtMsg.Tag = "txtPwd";
                return;
            }

            if (this.txtUserName.Text.Trim() == "")
            {
                txtMsg.Text = "*请输入帐号使用人姓名！";
                txtMsg.Tag = "txtUserName";
                return;
            }

            if (this._user_manger.IsChecked == true)
            {
                user_manager_flag = "1";
            }
            else if (this._user_manger_2.IsChecked == true)
            {
                user_manager_flag = "0";
            }

            //根据btnSave按钮的Tag属性判断是添加还是修改（null为添加，反之为修改）
            string strSql = string.Empty;

            if (btnSave.Tag == null)
            {
                bool exit_flag = dbHelper.Exists(string.Format("SELECT count(RECO_PKID) from sys_client_user where NUMB_USER ='{0}' ", _loginName.Text));
                if (exit_flag)
                {
                    Toolkit.MessageBox.Show("该登录帐号已存在，请重新输入！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                MD5 md5 = new MD5CryptoServiceProvider();
                string password = BitConverter.ToString(md5.ComputeHash(Encoding.Default.GetBytes(this._loginPassword.Password))).Replace("-", "");

                strSql = string.Format(@"INSERT INTO sys_client_user(NUMB_USER,INFO_USER,INFO_PASSWORD,fk_dept,START_DATE,ROLE_ID,cuserid,expired) VALUES 
                                      ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')",
                                      _loginName.Text, txtUserName.Text, password, (_department.SelectedItem as Label).Tag.ToString(),
                                      DateTime.Now,(_cmbRoleType.SelectedItem as Label).Tag.ToString(),(Application.Current.Resources["User"] as UserInfo).ID,user_manager_flag);
            }
            else
            {
                if (this._loginPassword.Password == password_old)
                {
                    strSql = string.Format(@"UPDATE sys_client_user SET NUMB_USER = '{0}', INFO_USER = '{1}', fk_dept = '{2}',
                                       ROLE_ID = '{3}',expired = '{4}' WHERE RECO_PKID = {5}",
                                           _loginName.Text, txtUserName.Text.Trim(), (_department.SelectedItem as Label).Tag.ToString(),
                                          (_cmbRoleType.SelectedItem as Label).Tag.ToString(), user_manager_flag, btnSave.Tag.ToString());
                }
                else
                {
                    MD5 md5 = new MD5CryptoServiceProvider();
                    string password = BitConverter.ToString(md5.ComputeHash(Encoding.Default.GetBytes(this._loginPassword.Password))).Replace("-", "");
                    strSql = string.Format(@"UPDATE sys_client_user SET NUMB_USER = '{0}', INFO_USER = '{1}', fk_dept = '{2}',
                                       ROLE_ID = '{3}',expired = '{4}',INFO_PASSWORD = '{5}' WHERE RECO_PKID = {6}",
                                           _loginName.Text, txtUserName.Text.Trim(), (_department.SelectedItem as Label).Tag.ToString(),
                                          (_cmbRoleType.SelectedItem as Label).Tag.ToString(), user_manager_flag,password, btnSave.Tag.ToString());
                }
            }

            try
            {
                int num = dbHelper.ExecuteSql(strSql);
                if (num == 1)
                {
                    
                    Toolkit.MessageBox.Show("保存成功！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    if (btnSave.Tag == null)
                    {
                        Common.SysLogEntry.WriteLog("系统用户管理", (Application.Current.Resources["User"] as UserInfo).ShowName, OperationType.Add, "添加系统用户");
                    }
                    else
                    {
                        Common.SysLogEntry.WriteLog("系统用户管理", (Application.Current.Resources["User"] as UserInfo).ShowName, OperationType.Modify, "修改系统用户");
                    }
                    Clear();
                    BindData();
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

        /// <summary>
        /// 清除提示信息
        /// </summary>
        /// <param name="sender">控件</param>
        private void ClearTip(object sender)
        {
            string name = (sender as FrameworkElement).Name;
            if (txtMsg.Tag != null)
            {
                if (name == txtMsg.Tag.ToString())
                {
                    txtMsg.Text = "";
                }
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            Clear();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (Toolkit.MessageBox.Show("确定要删除该用户吗？", "系统询问", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                string id = (sender as Button).Tag.ToString();
                if (DeleteUser(id))
                {
                    Toolkit.MessageBox.Show("删除成功！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    Common.SysLogEntry.WriteLog("系统用户管理", (Application.Current.Resources["User"] as UserInfo).ShowName, OperationType.Delete, "删除系统用户");
                    Clear();
                    BindData();
                }
                else
                {
                    Toolkit.MessageBox.Show("删除失败！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                e.Handled = false;
            }
        }

        private void btnModify_Click(object sender, RoutedEventArgs e)
        {
            user_details.Visibility = Visibility.Visible;
            btnSave.Visibility = Visibility.Visible;
            btnCancel.Visibility = Visibility.Visible;
            this._department.IsEnabled = true;
            this._loginName.IsEnabled = true;
            this._loginPassword.IsEnabled = true;
            this.txtUserName.IsEnabled = true;
            this._user_manger.IsEnabled = true;
            this._user_manger_2.IsEnabled = true;
            this._dept_flag.Text = "(必填)";
            //this._role_flag.Text = "(必填)";
            this._user_flag.Text = "(必填)";
            this._password_flag.Text = "(必填)";
            this._name_flag.Text = "(必填)";
            this._manager_flag.Text = "(必填)";
            string id = (sender as Button).Tag.ToString();

            DataRow dr = dbHelper.GetDataSet("SELECT RECO_PKID,NUMB_USER,INFO_USER,INFO_PASSWORD,fk_dept,sys_client_sysdept.INFO_NAME,ROLE_ID,expired " +
                        "FROM sys_client_user ,sys_client_sysdept " +
                        "WHERE sys_client_user.fk_dept = sys_client_sysdept.INFO_CODE " +
                        "AND sys_client_user.RECO_PKID = " + id).Tables[0].Rows[0];

            this.txtUserName.Text = dr["INFO_USER"].ToString();
            this._loginName.Text = dr["NUMB_USER"].ToString();

            for (int i = 0; i < _department.Items.Count; i++)
            {
                if ((_department.Items[i] as Label).Tag.ToString() == dr["fk_dept"].ToString())
                {
                    _department.SelectedItem = _department.Items[i];
                    break;
                }
            }

            for (int i = 0; i < _cmbRoleType.Items.Count; i++)
            {
                if ((_cmbRoleType.Items[i] as Label).Tag.ToString() == dr["ROLE_ID"].ToString())
                {
                    _cmbRoleType.SelectedItem = _cmbRoleType.Items[i];
                    break;
                }
            }

            this._loginPassword.Password = dr["INFO_PASSWORD"].ToString();
            password_old = dr["INFO_PASSWORD"].ToString();
            if (dr["expired"].ToString() == "1")
            {
                this._user_manger.IsChecked = true;
            }
            else if (dr["expired"].ToString() == "0")
            {
                this._user_manger_2.IsChecked = true;
            }

            this.btnSave.Tag = id;
        }

        //private void Reset_Click(object sender, RoutedEventArgs e)
        //{
        //    string strSql = string.Empty;

        //    MD5 md5 = new MD5CryptoServiceProvider();
        //    string password = BitConverter.ToString(md5.ComputeHash(Encoding.Default.GetBytes("123456"))).Replace("-", "");

        //    strSql = string.Format(@"UPDATE sys_client_user SET INFO_PASSWORD = '{0}' WHERE RECO_PKID = {1}", password, btnSave.Tag.ToString());

        //    try
        //    {
        //        int num = dbHelper.ExecuteSql(strSql);
        //        if (num == 1)
        //        {

        //            Toolkit.MessageBox.Show("密码重置成功！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
        //            Common.SysLogEntry.WriteLog("系统用户管理", (Application.Current.Resources["User"] as UserInfo).ShowName, OperationType.Add, "重置用户密码");
        //            Clear();
        //            BindData();
        //        }
        //        else
        //        {
        //            Toolkit.MessageBox.Show("密码重置失败！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
        //            return;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        Toolkit.MessageBox.Show("数据处理失败！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
        //        return;
        //    }
        //}

        private void _user_manger_Checked(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).Name == "_user_manger")
            {
                _user_manger_2.IsChecked = false;
            }
            else if ((sender as CheckBox).Name == "_user_manger_2")
            {
                _user_manger.IsChecked = false;
            }
        }

        private void loginName_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            //if (e.DataObject.GetDataPresent(typeof(String)))
            //{
            //    String text = (String)e.DataObject.GetData(typeof(String));
            //    if (!isNumberic(text))
            //    { e.CancelCommand(); }
            //}
            //else { e.CancelCommand(); }
        }

        private void loginName_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

    }
}
