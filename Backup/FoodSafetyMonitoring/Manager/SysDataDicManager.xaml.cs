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
using FoodSafetyMonitoring.Manager.UserControls;
using System.Data;
using Toolkit = Microsoft.Windows.Controls;
using FoodSafetyMonitoring.dao;

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// SysDataDicManager.xaml 的交互逻辑
    /// </summary>
    public partial class SysDataDicManager : UserControl, IClickChildMenuInitUserControlUI
    {
        private DBUtility.DbHelperMySQL dbHelper = null;
        private DataTable currentTable = new DataTable();
        private ButtonAddControl control = null;

        public SysDataDicManager()
        {
            InitializeComponent();
        }


        #region IClickChildMenuInitUserControlUI 成员

        private int flag_init = 0;//初始化,0未初始化,1已初始化

        public void InitUserControlUI()
        {
            if (flag_init == 1)
            {
                return;
            }
            flag_init = 1;
            BindData();
            string sql = "select * from sys_client_dict where " +
                         "dept_level=(select flag_tier from sys_client_sysdept where info_code= " +
                         "(select fk_dept from sys_client_user where reco_pkid='" + (Application.Current.Resources["User"] as UserInfo).ID + "'));";
            DataTable table = dbHelper.GetDataSet(sql).Tables[0];
            foreach (DataRow row in table.Rows)
            {
                ButtonAddControl buttonAddControl = new ButtonAddControl();
                buttonAddControl.Text = row["INFO_NAME"].ToString();
                buttonAddControl.Tag = row["INFO_CODE"].ToString();
                buttonAddControl.AddClick += new EventHandler(buttonAddControl_AddClick);
                buttonAddControl.ContentClick += new EventHandler(buttonAddControl_ContentClick);
                container.Children.Add(buttonAddControl);
            }
        }

        #endregion

        void buttonAddControl_ContentClick(object sender, EventArgs e)
        {
            if (sender == null)
            {
                return;
            }
            Digui(container);

            (sender as ButtonAddControl).btn.IsEnabled = false;

            //从内存表查出数据填充listview 
            FillListView((sender as ButtonAddControl).Tag.ToString());
            Clear();
        }

        void buttonAddControl_AddClick(object sender, EventArgs e)
        {
            this.txtDataDic.Text = (sender as ButtonAddControl).Text.ToString();
            this.txtCode.Text = CreateCode((sender as ButtonAddControl).Tag.ToString());
            this.btnSave.Tag = Common.OperationType.Add;
        }

        private void BindData()
        {
            string strSql = "select NUMB_DATADICT,INFO_CODE,INFO_NAME,INFO_GBCODE,Dept_ID, " +
                            "(CASE WHEN INFO_USE=1 THEN '启用' ELSE '不启用' END) AS INFO_USE " +
                            "from sys_client_dict " +
                            "where dept_id=(select fk_dept from sys_client_user where reco_pkid='" + (Application.Current.Resources["User"] as UserInfo).ID + "')";

            try
            {
                dbHelper = DBUtility.DbHelperMySQL.CreateDbHelper();
                DataTable dt = dbHelper.GetDataSet(strSql).Tables[0];
                currentTable = dt;
            }
            catch (Exception)
            {
                return;
            }
        }

        private void FillListView(string content)
        {
            DataRow[] drs = currentTable.Select("INFO_GBCODE='" + content + "'");
            DataTable temp = currentTable.Clone();
            foreach (DataRow row in drs)
            {
                DataRow dr = temp.NewRow();
                dr.ItemArray = row.ItemArray;
                temp.Rows.Add(dr);
            }
            this.lvlist.DataContext = temp;
        }


        private void Digui(DependencyObject obj)
        {
            if (obj is Button)
            {
                (obj as Button).IsEnabled = true;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                Digui(VisualTreeHelper.GetChild(obj, i));
            }
        }

        /// <summary>
        /// 生成子项代码
        /// </summary>
        /// <param name="content">字典大项</param>
        /// <returns>子项代码</returns>
        private string CreateCode(string id)
        {
            string code = string.Empty;
            DataRow[] drs = currentTable.Select("INFO_GBCODE='" + id + "'", "INFO_CODE DESC");
            if (drs.Length > 0)
            {
                code = (Convert.ToInt32(drs[0]["INFO_CODE"]) + 1).ToString();
            }
            else
            {
                code = id + "01";
            }

            return code;
        }

        private void btnModify_Click(object sender, RoutedEventArgs e)
        {
            string id = (sender as Button).Tag.ToString();
            DataRow[] drs = currentTable.Select("NUMB_DATADICT=" + id);
            if (!Exists(control.Text.ToString(), drs[0]["INFO_CODE"].ToString()))
            {
                if (drs.Length == 1)
                {
                    this.txtDataDic.Text = drs[0]["INFO_GBCODE"].ToString();
                    this.txtCode.Text = drs[0]["INFO_CODE"].ToString();
                    this.txtName.Text = drs[0]["INFO_NAME"].ToString();
                    this.txtNote.Text = drs[0]["INFO_NOTE"].ToString();
                    this.txtCode.Tag = drs[0]["dept_id"].ToString();
                    this.cmbIsEnable.IsChecked = drs[0]["INFO_USE"].ToString() == "启用" ? true : false;
                    this.btnSave.Tag = Common.OperationType.Modify;
                }
            }
            else
            {
                Toolkit.MessageBox.Show("修改项被引用，不能修改！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (Toolkit.MessageBox.Show("您确定要删除吗？", "系统询问", MessageBoxButton.YesNoCancel, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                string id = (sender as Button).Tag.ToString();

                DataRow[] drs = currentTable.Select("NUMB_DATADICT=" + id);

                if (!Exists(control.Text.ToString(), drs[0]["INFO_CODE"].ToString()))
                {
                    try
                    {
                        int num = dbHelper.ExecuteSql(string.Format("delete from sys_client_datadict where NUMB_DATADICT={0}", id));
                        if (num == 1)
                        {
                            Toolkit.MessageBox.Show("删除成功！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                            RemoveRow(id);
                            Common.SysLogEntry.WriteLog("系统字典设置", Application.Current.Resources["UserName"].ToString(), Common.OperationType.Delete, "删除系统字典");
                        }
                        else
                        {
                            Toolkit.MessageBox.Show("删除失败！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }
                    }
                    catch (Exception)
                    {
                        Toolkit.MessageBox.Show("删除失败！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                }
                else
                {
                    Toolkit.MessageBox.Show("删除项被引用，不能删除！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
            }
            else
            {
                e.Handled = false;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (this.btnSave.Tag == null)
            {
                return;
            }

            if ((Common.OperationType)this.btnSave.Tag == Common.OperationType.Add)
            {
                if (dbHelper.Exists("select INFO_CODE from sys_client_datadict where INFO_CODE='" + this.txtCode.Text + "'"))
                {
                    txtMsg.Text = "*代码已存在！请点击添加！";
                    return;
                }
            }

            if (this.txtName.Text.Trim() == "")
            {
                txtMsg.Text = "*名称不能为空！";
                return;
            }

            string strSql = "";
            switch ((Common.OperationType)this.btnSave.Tag)
            {
                case Common.OperationType.Add:
                    strSql = string.Format("insert into sys_client_datadict (INFO_CODE,INFO_NAME,INFO_GBCODE,INFO_NOTE,INFO_USE,Dept_ID) values ('{0}','{1}','{2}','{3}','{4}')",
                                           txtCode.Text, txtName.Text.Trim(), txtDataDic.Text, txtNote.Text, cmbIsEnable.IsChecked, txtCode.Tag.ToString());
                    break;
                case Common.OperationType.Modify:
                    strSql = string.Format("update sys_client_datadict set INFO_NAME='{0}',INFO_NOTE='{1}',INFO_USE={2} where INFO_CODE='{3}'",
                                           txtName.Text.Trim(), txtNote.Text.Trim(), cmbIsEnable.IsChecked, txtCode.Text);
                    break;
            }

            if (strSql != "")
            {
                try
                {
                    int num = dbHelper.ExecuteSql(strSql);
                    if (num == 1)
                    {
                        Toolkit.MessageBox.Show("保存成功！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        Common.SysLogEntry.WriteLog("系统字典设置", Application.Current.Resources["UserName"].ToString(), (Common.OperationType)this.btnSave.Tag, "编辑系统字典");
                        BindData();
                        FillListView(txtDataDic.Text);
                        Clear();
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

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Clear();
        }

        private void Clear()
        {
            this.txtDataDic.Text = "";
            this.txtCode.Text = "";
            this.txtName.Text = "";
            this.txtNote.Text = "";
            this.txtCode.Tag = "";
            this.cmbIsEnable.IsChecked = false;
            this.btnSave.Tag = null;
        }


        /// <summary>
        /// 从datatable里移除行
        /// </summary>
        /// <param name="id">id号</param>
        private void RemoveRow(string id)
        {
            DataRow[] drs = currentTable.Select("NUMB_DATADICT=" + id);
            if (drs.Length == 1)
            {
                currentTable.Rows.Remove(drs[0]);
                if (control != null)
                {
                    FillListView(control.Text.ToString());
                }
            }
        }

        /// <summary>
        /// 判断是否被引用
        /// </summary>
        /// <param name="content">字典大项</param>
        /// <param name="code">子项代码</param>
        /// <returns>Bool值，true存在，false不存在</returns>
        private bool Exists(string content, string code)
        {
            string strSql = string.Empty;

            bool result = false;

            switch (content)
            {
                case "区域类型":
                    strSql = string.Format("SELECT FK_INFO_CODE FROM sys_client_sysarea WHERE FK_INFO_CODE='{0}'", code);
                    break;
                case "区域属性":
                    strSql = string.Format("SELECT FK_ATTRIBUTE_CODE FROM sys_client_sysarea WHERE FK_ATTRIBUTE_CODE='{0}'", code);
                    break;
                case "系统设备类型":
                    strSql = string.Format("SELECT FK_TYPE_CODE FROM sys_client_device WHERE FK_TYPE_CODE='{0}'", code);
                    break;
                case "告警类型":
                    strSql = string.Format("SELECT FK_INFO_CODE FROM sys_alarm_set WHERE FK_INFO_CODE='{0}'", code);
                    break;
                case "告警处理方式":
                    strSql = string.Format("SELECT FK_HANDLE_CODE FROM sys_alarm_data WHERE FK_HANDLE_CODE='{0}'", code);
                    break;
                case "考勤班次":
                    strSql = string.Format("SELECT FK_INFO_CODE FROM sys_attendance_set WHERE FK_INFO_CODE='{0}'", code);
                    break;
                case "人员类型":
                    strSql = string.Format("SELECT FK_TYPE_CODE FROM sys_client_person WHERE FK_TYPE_CODE='{0}'", code);
                    break;
                case "人员属性":
                    strSql = string.Format("SELECT FK_POST_CODE FROM sys_client_person WHERE FK_POST_CODE='{0}'", code);
                    break;
            }

            try
            {
                result = dbHelper.Exists(strSql);
            }
            catch (Exception)
            {
                result = true;
            }

            return result;
        }

    }
}
