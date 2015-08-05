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
using FoodSafetyMonitoring.dao;
using FoodSafetyMonitoring.Common;
using System.Data;
using FoodSafetyMonitoring.Manager.UserControls;

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// SysCultureFileReport.xaml 的交互逻辑
    /// </summary>
    public partial class SysCultureFileReport : UserControl
    {

        private IDBOperation dbOperation;
        private string user_flag_tier;
        private DataTable currenttable;

        public SysCultureFileReport(IDBOperation dbOperation)
        {
            InitializeComponent();

            this.dbOperation = dbOperation;
            user_flag_tier = (Application.Current.Resources["User"] as UserInfo).FlagTier;


            dtpStartDate.SelectedDate = DateTime.Now.AddDays(-1);
            dtpEndDate.SelectedDate = DateTime.Now;

            //检测站点
            switch (user_flag_tier)
            {
                case "0": _dept_name.Text = "选择省:";
                    break;
                case "1": _dept_name.Text = "选择地市:";
                    break;
                case "2": _dept_name.Text = "选择区县:";
                    break;
                case "3": _dept_name.Text = "选择养殖场:";
                    break;
                case "4": _dept_name.Text = "选择养殖场:";
                    break;
                default: break;
            }
            ComboboxTool.InitComboboxSource(_detect_dept, string.Format("call p_dept_cxtj_hb({0},'{1}')", (Application.Current.Resources["User"] as UserInfo).ID, "yz"), "cxtj");

            //如果登录用户的部门是站点级别，则将查询条件检测站点赋上默认值
            if (isDept())
            {
                _detect_dept.SelectedIndex = 1;
            }

            _tableview.DetailsRowEnvent += new UcTableView_NoTitle.DetailsRowEventHandler(_tableview_DetailsRowEnvent);
        }

        //判断登录用户的部门级别
        private bool isDept()
        {
            string flag = dbOperation.GetDbHelper().GetSingle("select FLAG_TIER from sys_client_sysdept where INFO_CODE =" + (Application.Current.Resources["User"] as UserInfo).DepartmentID).ToString();

            if (flag == "4")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void _query_Click(object sender, RoutedEventArgs e)
        {
            DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_report_culture_file('{0}','{1}','{2}','{3}')",
                              (Application.Current.Resources["User"] as UserInfo).ID,
                              ((DateTime)dtpStartDate.SelectedDate).ToShortDateString(),
                              ((DateTime)dtpEndDate.SelectedDate).ToShortDateString(),
                               _detect_dept.SelectedIndex < 1 ? "" : (_detect_dept.SelectedItem as Label).Tag)).Tables[0];

            currenttable = table;

            //创建DataTable
            DataTable tabledisplay = new DataTable();
            tabledisplay.Columns.Add(new DataColumn("序号"));
            switch (user_flag_tier)
            {
                case "0": tabledisplay.Columns.Add(new DataColumn("省名称"));
                    break;
                case "1": tabledisplay.Columns.Add(new DataColumn("地市名称"));
                    break;
                case "2": tabledisplay.Columns.Add(new DataColumn("区县名称"));
                    break;
                case "3": tabledisplay.Columns.Add(new DataColumn("检测站点名称"));
                    break;
                case "4": tabledisplay.Columns.Add(new DataColumn("检测站点名称"));
                    break;
                default: break;
            }

            tabledisplay.Columns.Add(new DataColumn("数量"));

            for (int i = 0; i < table.Rows.Count; i++ )
            {
                var row = tabledisplay.NewRow();
                row[0] = i + 1;
                row[1] = table.Rows[i][1].ToString();
                row[2] = table.Rows[i][2].ToString();

                tabledisplay.Rows.Add(row);
            }

            //表格的标题
            _tableview.BShowDetails = true;
            _title.Text = string.Format("合计{0}条数据", table.Rows.Count);
            _tableview.SetDataTable(tabledisplay, "", new List<int>());

        }

        void _tableview_DetailsRowEnvent(string id)
        {
            string dept_id;

            DataRow[] rows = currenttable.Select("PART_NAME = '" + id + "'");
            dept_id = rows[0]["PART_ID"].ToString();
            grid_info.Children.Add(new UcCultureFileReportDetails(dbOperation, ((DateTime)dtpStartDate.SelectedDate).ToShortDateString(),
                              ((DateTime)dtpEndDate.SelectedDate).ToShortDateString(), dept_id));
        }

        private void _export_Click(object sender, RoutedEventArgs e)
        {
            _tableview.ExportExcel();
        }
        
    }
}
