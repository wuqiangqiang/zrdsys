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

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// Test.xaml 的交互逻辑
    /// </summary>
    public partial class SysRoleManager : UserControl
    {
        // private String mouseEnterImage = "pack://application:,,/Images/小人3.png";
        private DbHelperMySQL dbHelper = null;

        private DataTable currentTable = new DataTable();
        public SysRoleManager()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(Test_Loaded);

        }

        void Test_Loaded(object sender, RoutedEventArgs e)
        {
            BindData();
            Clear();
        }
        /// <summary>
        /// 初始化Combobox，绑定数据源
        /// </summary>
        /// <param name="cmb">Combobox</param>
        /// <param name="strSql">sql语句</param>
        public void InitCombobox(ComboBox cmb)
        {
            try
            {

                if (cmb != null)
                {
                    if (cmb.Items.Count > 0)
                    {
                        cmb.Items.Clear();
                    }
                }
                Label item = new Label();
                item.Tag = -1;
                item.Content = "-请选择-";
                cmb.Items.Add(item);
                cmb.SelectedItem = item;

                for (int i = 1; i < 6; i++)
                {
                    string strValue = i.ToString();//隐藏值
                    string strItem = i.ToString();//显示值
                    Label item1 = new Label();
                    item1.Tag = strValue;
                    item1.Content = strItem;
                    cmb.Items.Add(item1);
                }
            }
            catch (Exception)
            {
                return;
            }
        }
        /// <summary>
        /// 从数据库获取数据
        /// </summary>
        /// 
        private void BindData()
        {

            DataTable dt = null;
            lvlist.DataContext = null;
            string strSql = string.Format("select NUMB_ROLE,INFO_NAME,INFO_EXPL,FLAG_TIER,FK_NUMB_ROLE,INFO_NOTE FROM sys_client_role where cuserid = '{0}'", (Application.Current.Resources["User"] as UserInfo).ID);
            try
            {

                dbHelper = DbHelperMySQL.CreateDbHelper();

                dt = dbHelper.GetDataSet(strSql).Tables[0];
                lvlist.DataContext = null;
                lvlist.DataContext = dt;
                currentTable = dt;
            }
            catch (Exception)
            {
                return;
            }
        }
        private void TextboxSearchControl_ImageClick(object sender, EventArgs e)
        {
            SelectUser(txtSearch.Text.Trim());
        }

        /// <summary>
        /// 从内存表中查询用户
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="userType">角色类型</param>
        private void SelectUser(string userName)
        {
            if (userName != "")
            {
                DataRow[] drs = currentTable.Select("INFO_NAME like '%" + userName + "%'");
                DataTable temp = currentTable.Clone();
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
                lvlist.DataContext = currentTable;
            }
        }
        //判断角色代码是否存在
        private bool isExistVisifnbr()
        {
            string sql = "select count(*) from sys_client_role where INFO_NAME='" + this.txt_RoleName.Text.ToString() + "'";
            try
            {
                int i = Convert.ToInt32(dbHelper.GetSingle(sql));
                if (i > 0)
                {
                    Toolkit.MessageBox.Show("角色代码已经存在！", "系统提示");
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        //判断角色下是否存在人员
        private bool isExistRoleName(string oid)
        {
            string sql = "";
            bool result = false;
            try
            {
                if (this.txt_RoleName.Text.Trim() == "")
                {
                    DataRow[] drs = currentTable.Select("NUMB_ROLE=" + oid);

                    string INFO_ROLE = drs[0]["INFO_CODE"].ToString();

                    sql = string.Format("select FK_CODE_ROLE from sys_client_user where  FK_CODE_ROLE ='{0}'", INFO_ROLE);

                }
                else
                {
                    sql = string.Format("select FK_CODE_ROLE from sys_client_user where FK_CODE_ROLE ='{0}'", this.txt_RoleName.Tag.ToString());
                }
                if (Exists(sql))
                {
                    result = true;
                }
            }

            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }
        public bool Exists(string strSql)
        {
            object obj = dbHelper.GetSingle(strSql);
            string cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = "";
            }
            else
            {
                cmdresult = obj.ToString();
            }
            if (cmdresult == "")
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private void Clear()
        {
            this.txt_RoleName.IsEnabled = false;
            this.txt_RoleExplain.IsEnabled = false;

            this.txt_vComment.IsEnabled = false;

            this.txt_RoleName.Text = "";
            this.txt_RoleExplain.Text = "";



            this.txt_vComment.Text = "";
            this.btnSave.Tag = null;
            this.txtMsg.Text = "";
        }
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (Toolkit.MessageBox.Show("确定要删除该角色吗？", "询问", MessageBoxButton.YesNoCancel, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                string id = (sender as Button).Tag.ToString();
                if (!isExistRoleName(id))//判断角色下是否存在人员
                {
                    if (DeleteUser(id))
                    {
                        Toolkit.MessageBox.Show("删除成功！", "系统提示");
                        BindData();
                        Clear();
                    }
                    else
                    {
                        Toolkit.MessageBox.Show("删除失败！", "系统提示");
                    }
                }
                else
                {
                    Toolkit.MessageBox.Show("角色下存在人员，不能删除！", "系统提示");
                    return;
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
            string strSql = string.Format("delete from sys_client_role where NUMB_ROLE=" + id);
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
        private void btnModify_Click(object sender, RoutedEventArgs e)
        {
            this.txt_RoleName.IsEnabled = true;
            this.txt_RoleExplain.IsEnabled = true;


            this.txt_vComment.IsEnabled = true;

            string id = (sender as Button).Tag.ToString();
            DataRow[] drs = currentTable.Select("NUMB_ROLE=" + id);
            if (drs.Length == 1)
            {
                this.txt_RoleName.Tag = drs[0]["NUMB_ROLE"].ToString();
                this.txt_RoleName.Text = drs[0]["INFO_NAME"].ToString();
                this.txt_RoleExplain.Text = drs[0]["INFO_EXPL"].ToString();


                this.txt_vComment.Text = drs[0]["INFO_NOTE"].ToString();

                this.btnSave.Tag = id;
            }
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (this.txt_RoleName.Text.Trim() == "")
            {
                txtMsg.Text = "*角色名不能为空！";
                txtMsg.Tag = "txt_RoleName";
                return;
            }


            //根据btnSave按钮的Tag属性判断是添加还是修改（null为添加，反之为修改）
            string strSql = string.Empty;
            if (btnSave.Tag == null)
            {
                if (isExistVisifnbr())//判断角色代码是否存在
                {
                    strSql = string.Format("call p_insert_role({0},'{1}','{2}','{3}')",
                        (Application.Current.Resources["User"] as UserInfo).ID, txt_RoleName.Text,txt_vComment.Text, txt_RoleExplain.Text);
                }
                else
                {
                    this.txtMsg.Text = "";
                    return;
                }
            }
            else
            {
                strSql = string.Format(@"update sys_client_role set INFO_NAME='{0}',INFO_EXPL='{1}',MUSERID='{2}',INFO_NOTE='{3}'
                                                         where NUMB_ROLE={4} ", txt_RoleName.Text, txt_RoleExplain.Text.Trim(), (Application.Current.Resources["User"] as UserInfo).ID,
                                    txt_vComment.Text, btnSave.Tag.ToString());
            }

            try
            {

                int num = dbHelper.ExecuteSql(strSql);
                if (num == 1)
                {
                    Toolkit.MessageBox.Show("保存成功！", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                    Clear();
                    BindData();
                }
                else
                {
                    Toolkit.MessageBox.Show("保存失败！", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                    return;
                }
            }
            catch (Exception)
            {
                Toolkit.MessageBox.Show("保存失败！", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                return;
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            Clear();
        }


        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            Clear();
            this.txt_RoleName.IsEnabled = true;
            this.txt_RoleExplain.IsEnabled = true;
            this.txt_vComment.IsEnabled = true;
        }


        /// <summary>
        /// 从内存表中查询角色名称
        /// </summary>

        private void SelectRole(string RoleName, string Rolelevel)
        {

            StringBuilder strsql = new StringBuilder();

            DataTable dt_SelectRole = null;
            lvlist.DataContext = null;
            strsql.Append("select NUMB_ROLE,INFO_CODE,INFO_NAME,INFO_EXPL,FLAG_TIER,FK_NUMB_ROLE,FK_CODE_ROLE,FK_NAME_ROLE,INFO_NOTE FROM sys_client_role where 1=1 ");


            if (RoleName != "")
            {

                strsql.Append(" and INFO_NAME like '%" + RoleName + "%'");

            }
            if (Rolelevel != "-1")
            {
                strsql.Append(" and FLAG_TIER = " + Convert.ToInt16(Rolelevel) + "");

            }
            try
            {
                dt_SelectRole = dbHelper.GetDataSet(strsql.ToString()).Tables[0];
                lvlist.DataContext = null;
                lvlist.DataContext = dt_SelectRole;

            }
            catch (Exception)
            {
                return;
            }


        }
    }
    public class Myinfo
    {
        private string _info;

        public string Info
        {
            get { return _info; }
            set
            {
                _info = value;
            }
        }

        private string _content;
        public string Content
        {
            get { return _content; }
            set
            {
                _content = value;
            }
        }

        public string ImgSource
        {
            get;
            set;
        }
    }
}
