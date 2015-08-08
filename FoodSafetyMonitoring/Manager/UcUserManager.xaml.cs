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
using System.ComponentModel;
using System.Collections.ObjectModel;
using FoodSafetyMonitoring.dao;
using System.Security.Cryptography;

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// UcUserManager.xaml 的交互逻辑
    /// </summary>
    public partial class UcUserManager : UserControl
    {
        private IDBOperation dbOperation;
        FamilyTreeViewModel departmentViewModel;

        readonly Dictionary<string, string> cityLevelDictionary = new Dictionary<string, string>() { { "0", "国家" }, { "1", "省级" }, { "2", "地市" }, { "3", "区县" }, { "4", "检测站" } };
        private Department department;
        private DataTable ProvinceCityTable = null;
        private string user_flag_tier;
        private DataTable SupplierTable;
        private string password_old;
        //private int flag_init = 0;//初始化,0未初始化,1已初始化

        //当前选中的部门id,名称,部门等级,检测站点的类别
        private string dept_id;
        private string dept_name;
        private string dept_flag;
        private string dept_type;

        public UcUserManager(IDBOperation dbOperation)
        {
            InitializeComponent();
            this.dbOperation = dbOperation;
            ProvinceCityTable = Application.Current.Resources["省市表"] as DataTable;
            user_flag_tier = (Application.Current.Resources["User"] as UserInfo).FlagTier.ToString();
            SupplierTable = dbOperation.GetDbHelper().GetDataSet("select supplierId,supplierName from t_supplier").Tables[0];
            this.Loaded += new RoutedEventHandler(Load_DeptManager);
            //用户管理的初始化
            ComboboxTool.InitComboboxSource(_department, "select INFO_CODE,INFO_NAME from sys_client_sysdept", "lr");
            ComboboxTool.InitComboboxSource(_cmbRoleType, "SELECT NUMB_ROLE,INFO_NAME FROM sys_client_role where roletype = '1'", "lr");
            _department.SelectionChanged += new SelectionChangedEventHandler(_department_SelectionChanged);
        }

        public void Load_DeptManager(object sender, RoutedEventArgs e)
        {
            //if (flag_init == 1)
            //{
            //    return;
            //}
            //flag_init = 1;

            DataTable table = dbOperation.GetDepartment();
            if (table != null)
            {
                department = new Department();
                //DataRow[] rows = table.Select("FK_CODE_DEPT='0'");
                //if (rows.Length == 0)
                //{
                //    return;
                //}
                DataRow[] rows = table.Select();
                department.Name = rows[0]["INFO_NAME"].ToString();
                department.Row = rows[0];
                //对应湖北省级有3个部门（101 湖北畜安处，102 湖北动监处，103 湖北屠宰办），数据库中存在下级部门的是102
                string deptId = "";
                if (rows[0]["INFO_CODE"].ToString() == "101" || rows[0]["INFO_CODE"].ToString() == "103")
                {
                    deptId = "102";
                }
                else
                {
                    deptId = rows[0]["INFO_CODE"].ToString();
                }
                rows = table.Select("FK_CODE_DEPT='" + deptId + "'");
                foreach (DataRow row1 in rows)
                {
                    Department department1 = new Department();
                    department1.Parent = department;
                    department1.Row = row1;
                    department1.Name = row1["INFO_NAME"].ToString();
                    //对应湖北省级有3个部门（101 湖北畜安处，102 湖北动监处，103 湖北屠宰办），数据库中存在下级部门的是102
                    string deptId2 = "";
                    if (row1["INFO_CODE"].ToString() == "101" || row1["INFO_CODE"].ToString() == "103")
                    {
                        deptId2 = "102";
                    }
                    else
                    {
                        deptId2 = row1["INFO_CODE"].ToString();
                    }
                    rows = table.Select("FK_CODE_DEPT='" + deptId2 + "'");
                    foreach (DataRow row2 in rows)
                    {
                        Department department2 = new Department();
                        department2.Parent = department1;
                        department2.Row = row2;
                        department2.Name = row2["INFO_NAME"].ToString();
                        rows = table.Select("FK_CODE_DEPT='" + row2["INFO_CODE"].ToString() + "'");
                        foreach (DataRow row3 in rows)
                        {
                            Department department3 = new Department();
                            department3.Parent = department2;
                            department3.Row = row3;
                            department3.Name = row3["INFO_NAME"].ToString();
                            rows = table.Select("FK_CODE_DEPT='" + row3["INFO_CODE"].ToString() + "'");
                            foreach (DataRow row4 in rows)
                            {
                                Department department4 = new Department();
                                department4.Parent = department3;
                                department4.Row = row4;
                                department4.Name = row4["INFO_NAME"].ToString();
                                department3.Children.Add(department4);
                            }
                            department2.Children.Add(department3);
                        }
                        department1.Children.Add(department2);
                    }
                    department.Children.Add(department1);
                }
                departmentViewModel = new FamilyTreeViewModel(department);
                _treeView.DataContext = departmentViewModel;
            }
        }

        //省市上双击事件
        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount > 0)
            {
                //获取当前部门树形菜单上选中的部门id和名称
                Department department = (sender as TextBlock).Tag as Department;
                DataRow row = department.Row;
                dept_id = row["INFO_CODE"].ToString();
                dept_name = row["INFO_NAME"].ToString();
                dept_flag = row["FLAG_TIER"].ToString();
                dept_type = row["type"].ToString();
                Load_UserManager(dept_id);
                btnCreate.Visibility = Visibility.Visible;
                Clear();
            }
        }

        private void Load_UserManager(string dept_id)
        {
            string strSql = "select RECO_PKID,NUMB_USER,sys_client_user.INFO_USER,sys_client_sysdept.INFO_NAME,sys_client_role.INFO_NAME as role_expl " +
                            "FROM sys_client_user ,sys_client_sysdept,sys_client_role " +
                            "WHERE sys_client_user.fk_dept = sys_client_sysdept.INFO_CODE " +
                            "and sys_client_user.ROLE_ID = sys_client_role.NUMB_ROLE " +
                            "and sys_client_user.fk_dept = " + dept_id;

            try
            {
                DataTable dt = dbOperation.GetDbHelper().GetDataSet(strSql).Tables[0];
                lvlist.DataContext = null;
                lvlist.DataContext = dt;
                lvlist.Tag = dt;
            }
            catch (Exception)
            {
                return;
            }
        }

        void _department_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_department.SelectedIndex > 0)
            {
                switch (dept_flag)
                {
                    case "0": _cmbRoleType.IsEnabled = true;
                        ComboboxTool.InitComboboxSource(_cmbRoleType, "SELECT NUMB_ROLE,INFO_NAME FROM sys_client_role where NUMB_ROLE =1", "lr");
                        break;
                    case "1": _cmbRoleType.IsEnabled = true;
                        ComboboxTool.InitComboboxSource(_cmbRoleType, "SELECT NUMB_ROLE,INFO_NAME FROM sys_client_role where roletype = '1' and FLAG_TIER = 1", "lr");
                        break;
                    case "2": _cmbRoleType.IsEnabled = true;
                        ComboboxTool.InitComboboxSource(_cmbRoleType, "SELECT NUMB_ROLE,INFO_NAME FROM sys_client_role where roletype = '1' and (FLAG_TIER = 2 or FLAG_TIER = 5)", "lr");
                        break;
                    case "3": _cmbRoleType.IsEnabled = true;
                        ComboboxTool.InitComboboxSource(_cmbRoleType, "SELECT NUMB_ROLE,INFO_NAME FROM sys_client_role where roletype = '1' and (FLAG_TIER = 3 or FLAG_TIER = 5)", "lr");
                        break;
                    case "4": _cmbRoleType.IsEnabled = true;
                        string type = dbOperation.GetDbHelper().GetSingle(string.Format("select typeflag from t_dept_type where typeId = '{0}'", dept_type)).ToString();
                        if (type == "yz")
                        {
                            ComboboxTool.InitComboboxSource(_cmbRoleType, "SELECT NUMB_ROLE,INFO_NAME FROM sys_client_role where NUMB_ROLE = 502", "lr");
                        }
                        else if (type == "cz")
                        {
                            ComboboxTool.InitComboboxSource(_cmbRoleType, "SELECT NUMB_ROLE,INFO_NAME FROM sys_client_role where NUMB_ROLE = 503", "lr");
                        }
                        else if (type == "tz")
                        {
                            ComboboxTool.InitComboboxSource(_cmbRoleType, "SELECT NUMB_ROLE,INFO_NAME FROM sys_client_role where (NUMB_ROLE = 504 or NUMB_ROLE = 505)", "lr");
                        }
                        break;
                    default: break;
                }
                _cmbRoleType.SelectionChanged += new SelectionChangedEventHandler(_cmbRoleType_SelectionChanged);
            }
        }

        void _cmbRoleType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_cmbRoleType.SelectedIndex > 0)
            {
                _subDetails.Text = dbOperation.GetDbHelper().GetSingle(string.Format("select GROUP_CONCAT(SUB_NAME) as SUB_NAME from v_sub_details_hb where ROLE_ID = '{0}' and SUB_FATHER_ID = '0'", (_cmbRoleType.SelectedItem as Label).Tag.ToString())).ToString();
            }
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            user_details.Visibility = Visibility.Visible;
            btnSave.Visibility = Visibility.Visible;
            btnCancel.Visibility = Visibility.Visible;
            Clear();
            this._department.Text = dept_name;
            //this._department.IsEnabled = true;
            this._loginName.IsEnabled = true;
            this._loginPassword.IsEnabled = true;
            this.txtUserName.IsEnabled = true;
            this._user_manger.IsEnabled = true;
            this._user_manger_2.IsEnabled = true;
            this._user_manger.IsChecked = true;
            //this._dept_flag.Text = "(必填)";
            this._role_flag.Text = "(必填)";
            this._user_flag.Text = "(必填)";
            this._password_flag.Text = "(必填)";
            this._name_flag.Text = "(必填)";
            this._manager_flag.Text = "(必填)";
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
            //this.txtMsg.Text = "";
            this._department.IsEnabled = false;
            this._cmbRoleType.IsEnabled = false;
            this._subDetails.IsEnabled = false;
            this._loginName.IsEnabled = false;
            this._loginPassword.IsEnabled = false;
            this.txtUserName.IsEnabled = false;
            this._user_manger.IsEnabled = false;
            this._user_manger_2.IsEnabled = false;
            this.btnSave.Tag = null;
            //this._dept_flag.Text = "";
            this._role_flag.Text = "";
            this._user_flag.Text = "";
            this._password_flag.Text = "";
            this._name_flag.Text = "";
            this._manager_flag.Text = "";
            //this.txtUserName.Focus();
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
                    Load_UserManager(dept_id);
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

        private bool DeleteUser(string id)
        {
            bool result = false;
            string strSql = string.Format("delete from sys_client_user where RECO_PKID=" + id);
            try
            {
                int num = dbOperation.GetDbHelper().ExecuteSql(strSql);
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

        private void btnModify_Click(object sender, RoutedEventArgs e)
        {
            user_details.Visibility = Visibility.Visible;
            btnSave.Visibility = Visibility.Visible;
            btnCancel.Visibility = Visibility.Visible;
            //this._department.IsEnabled = true;
            this._loginName.IsEnabled = true;
            this._loginPassword.IsEnabled = true;
            this.txtUserName.IsEnabled = true;
            this._user_manger.IsEnabled = true;
            this._user_manger_2.IsEnabled = true;
            //this._dept_flag.Text = "(必填)";
            this._role_flag.Text = "(必填)";
            this._user_flag.Text = "(必填)";
            this._password_flag.Text = "(必填)";
            this._name_flag.Text = "(必填)";
            this._manager_flag.Text = "(必填)";
            string id = (sender as Button).Tag.ToString();

            DataRow dr = dbOperation.GetDbHelper().GetDataSet("SELECT RECO_PKID,NUMB_USER,INFO_USER,INFO_PASSWORD,fk_dept,sys_client_sysdept.INFO_NAME,ROLE_ID,expired " +
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


        private void FrameworkElement_GotFocus(object sender, RoutedEventArgs e)
        {
            //ClearTip(sender);
        }

        /// <summary>
        /// 清除提示信息
        /// </summary>
        /// <param name="sender">控件</param>
        //private void ClearTip(object sender)
        //{
        //    string name = (sender as FrameworkElement).Name;
        //    if (txtMsg.Tag != null)
        //    {
        //        if (name == txtMsg.Tag.ToString())
        //        {
        //            txtMsg.Text = "";
        //        }
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

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string user_manager_flag = "";
            if (this._department.SelectedIndex < 1)
            {
                //txtMsg.Text = "*请选择帐号使用单位！";
                //txtMsg.Tag = "_department";
                Toolkit.MessageBox.Show("请选择帐号使用单位!", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (this._cmbRoleType.SelectedIndex < 1)
            {
                //txtMsg.Text = "*请选择帐号权限！";
                //txtMsg.Tag = "_cmbRoleType";
                Toolkit.MessageBox.Show("请选择帐号权限!", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (this._loginName.Text.Trim() == "")
            {
                //txtMsg.Text = "*请输入登录帐号！";
                //txtMsg.Tag = "_loginName";
                Toolkit.MessageBox.Show("请输入登录帐号!", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (this._loginPassword.Password.Trim() == "")
            {
                //txtMsg.Text = "*请输入密码！";
                //txtMsg.Tag = "txtPwd";
                Toolkit.MessageBox.Show("请输入密码!", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (this.txtUserName.Text.Trim() == "")
            {
                //txtMsg.Text = "*请输入帐号使用人姓名！";
                //txtMsg.Tag = "txtUserName";
                Toolkit.MessageBox.Show("请输入帐号使用人姓名!", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
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
                bool exit_flag = dbOperation.GetDbHelper().Exists(string.Format("SELECT count(RECO_PKID) from sys_client_user where NUMB_USER ='{0}' ", _loginName.Text));
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
                                      DateTime.Now, (_cmbRoleType.SelectedItem as Label).Tag.ToString(), (Application.Current.Resources["User"] as UserInfo).ID, user_manager_flag);
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
                                          (_cmbRoleType.SelectedItem as Label).Tag.ToString(), user_manager_flag, password, btnSave.Tag.ToString());
                }
            }

            try
            {
                int num = dbOperation.GetDbHelper().ExecuteSql(strSql);
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
                    Load_UserManager(dept_id);
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

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            Clear();
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
