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
using System.Windows.Forms.Integration;
using System.Data;
using FoodSafetyMonitoring.Common;
using FoodSafetyMonitoring.Manager.UserControls;
using Toolkit = Microsoft.Windows.Controls;

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// SysWarningReport.xaml 的交互逻辑
    /// </summary>
    public partial class SysWarningReport : UserControl
    {
        private IDBOperation dbOperation;
        private DataTable current_table;
        private Dictionary<string, MyColumn> MyColumns = new Dictionary<string, MyColumn>();
        private string user_flag_tier;
        private string dept_name;

        public SysWarningReport(IDBOperation dbOperation)
        {
            InitializeComponent();

            this.dbOperation = dbOperation;
            user_flag_tier = (Application.Current.Resources["User"] as UserInfo).FlagTier;

            //初始化查询条件
            reportDate_kssj.SelectedDate = DateTime.Now.AddDays(-1);
            reportDate_jssj.SelectedDate = DateTime.Now;
            //检测站点
            switch (user_flag_tier)
            {
                case "0": _dept_name.Text = "省:";
                    dept_name = "省名称";
                    break;
                case "1": _dept_name.Text = "市(州):";
                    dept_name = "市(州)名称";
                    break;
                case "2": _dept_name.Text = "区县:";
                    dept_name = "区县名称";
                    break;
                case "3": _dept_name.Text = "检测单位:";
                    dept_name = "检测单位名称";
                    break;
                case "4": _dept_name.Text = "检测单位:";
                    dept_name = "检测单位名称";
                    break;
                default: break;
            }
            //复核状态
            DataTable table_detect_type = new DataTable();
            table_detect_type.Columns.Add("id", Type.GetType("System.String"));
            table_detect_type.Columns.Add("name", Type.GetType("System.String"));
            table_detect_type.Rows.Add(new object[] { "3", "饲料检测" });
            table_detect_type.Rows.Add(new object[] { "0", "养殖检测" });
            table_detect_type.Rows.Add(new object[] { "1", "出证检测" });
            table_detect_type.Rows.Add(new object[] { "4", "宰前检测" });
            table_detect_type.Rows.Add(new object[] { "2", "屠宰检测" });
            ComboboxTool.InitComboboxSource(_detect_type, table_detect_type, "cxtj");
            _detect_type.SelectionChanged += new SelectionChangedEventHandler(_detect_type_SelectionChanged);

            //ComboboxTool.InitComboboxSource(_detect_dept, string.Format("call p_dept_cxtj_hb({0},'{1}')", (Application.Current.Resources["User"] as UserInfo).ID, depttype), "cxtj");
            ComboboxTool.InitComboboxSource(_detect_dept, string.Format("call p_dept_cxtj({0})", (Application.Current.Resources["User"] as UserInfo).ID), "cxtj");
            //检测项目
            ComboboxTool.InitComboboxSource(_detect_item, "SELECT ItemID,ItemNAME FROM t_det_item_hb WHERE OPENFLAG = '1'", "cxtj");
            //复核状态
            DataTable table_detect_result = new DataTable();
            table_detect_result.Columns.Add("id", Type.GetType("System.String"));
            table_detect_result.Columns.Add("name", Type.GetType("System.String"));
            table_detect_result.Rows.Add(new object[] { "0", "未复核" });
            table_detect_result.Rows.Add(new object[] { "1", "已复核" });
            ComboboxTool.InitComboboxSource(_review_flag, table_detect_result, "cxtj");

            MyColumns.Add("partid", new MyColumn("partid", "检测站点id") { BShow = false });
            MyColumns.Add("partname", new MyColumn("partname", dept_name) { BShow = true, Width = 18 });
            MyColumns.Add("yang", new MyColumn("yang", "阳性预警数") { BShow = true, Width = 12 });
            MyColumns.Add("yang_like", new MyColumn("yang_like", "疑似阳性预警数") { BShow = true, Width = 14 });
            MyColumns.Add("count", new MyColumn("count", "预警数合计") { BShow = true, Width = 12 });
            MyColumns.Add("review_yes", new MyColumn("review_yes", "已复核数") { BShow = true, Width = 12 });
            MyColumns.Add("review_no", new MyColumn("review_no", "未复核数") { BShow = true, Width = 12 });
            MyColumns.Add("sum_num", new MyColumn("sum_num", "总行数") { BShow = false });

            _tableview.MyColumns = MyColumns;
            _tableview.BShowDetails = true;

            _tableview.DetailsRowEnvent += new UcTableOperableView_NoTitle.DetailsRowEventHandler(_tableview_DetailsRowEnvent);
        }

        void _detect_type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(_detect_type.SelectedIndex > 0)
            {
                switch ((_detect_type.SelectedItem as Label).Tag.ToString())
                {
                    case "0": ComboboxTool.InitComboboxSource(_detect_dept, string.Format("call p_dept_cxtj_hb({0},'{1}')", (Application.Current.Resources["User"] as UserInfo).ID, "yz"), "cxtj");
                        break;
                    case "1": ComboboxTool.InitComboboxSource(_detect_dept, string.Format("call p_dept_cxtj_hb({0},'{1}')", (Application.Current.Resources["User"] as UserInfo).ID, "cz"), "cxtj");
                        break;
                    case "2": ComboboxTool.InitComboboxSource(_detect_dept, string.Format("call p_dept_cxtj_hb({0},'{1}')", (Application.Current.Resources["User"] as UserInfo).ID, "tz"), "cxtj");
                        break;
                    case "3": ComboboxTool.InitComboboxSource(_detect_dept, string.Format("call p_dept_cxtj_hb({0},'{1}')", (Application.Current.Resources["User"] as UserInfo).ID, "yz"), "cxtj");
                        break;
                    case "4": ComboboxTool.InitComboboxSource(_detect_dept, string.Format("call p_dept_cxtj_hb({0},'{1}')", (Application.Current.Resources["User"] as UserInfo).ID, "tz"), "cxtj");
                        break;
                    default: break;
                }
            }
        }

        private void _query_Click(object sender, RoutedEventArgs e)
        {
            if (reportDate_kssj.SelectedDate.Value.Date > reportDate_jssj.SelectedDate.Value.Date)
            {
                Toolkit.MessageBox.Show("开始时间大于结束时间，请重新选择！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            _tableview.GetDataByPageNumberEvent += new UcTableOperableView_NoTitle.GetDataByPageNumberEventHandler(_tableview_GetDataByPageNumberEvent);
            GetData();
            //_tableview.Title = string.Format("数据统计时间:{0}年{1}月{2}日到{3}年{4}月{5}日", reportDate_kssj.Value.Value.Year, reportDate_kssj.Value.Value.Month, reportDate_kssj.Value.Value.Day,
            //              reportDate_jssj.Value.Value.Year, reportDate_jssj.Value.Value.Month, reportDate_jssj.Value.Value.Day);
            //_title.Text = string.Format("▪ 数据统计时间:{0}年{1}月{2}日到{3}年{4}月{5}日  合计{6}条数据", reportDate_kssj.Value.Value.Year, reportDate_kssj.Value.Value.Month, reportDate_kssj.Value.Value.Day,
            //              reportDate_jssj.Value.Value.Year, reportDate_jssj.Value.Value.Month, reportDate_jssj.Value.Value.Day, _tableview.RowTotal);
            _sj.Visibility = Visibility.Visible;
            _hj.Visibility = Visibility.Visible;
            _title.Text = _tableview.RowTotal.ToString();
            _tableview.PageIndex = 1;

            if (_tableview.RowTotal == 0)
            {
                Toolkit.MessageBox.Show("没有查询到数据！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
        }

        private void GetData()
        {

            DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_warning_report_hb('{0}','{1}','{2}','{3}','{4}','{5}','{6}',{7},{8})",
                             (Application.Current.Resources["User"] as UserInfo).ID, reportDate_kssj.SelectedDate, reportDate_jssj.SelectedDate,
                              _detect_dept.SelectedIndex < 1 ? "" : (_detect_dept.SelectedItem as Label).Tag,
                              _detect_item.SelectedIndex < 1 ? "" : (_detect_item.SelectedItem as Label).Tag,
                              _review_flag.SelectedIndex < 1 ? "" : (_review_flag.SelectedItem as Label).Tag,
                              _detect_type.SelectedIndex < 1 ? "" : (_detect_type.SelectedItem as Label).Tag,
                             (_tableview.PageIndex - 1) * _tableview.RowMax,
                             _tableview.RowMax)).Tables[0];
            _tableview.Table = table;
            current_table = table;
        }

        void _tableview_GetDataByPageNumberEvent()
        {
            GetData();
        }

        void _tableview_DetailsRowEnvent(string id)
        {
            string dept_id;
            string item_id;
            string review_id;
            string detect_type;

            dept_id = id;
            item_id = _detect_item.SelectedIndex < 1 ? "" : (_detect_dept.SelectedItem as Label).Tag.ToString();
            review_id = _review_flag.SelectedIndex < 1 ? "" : (_review_flag.SelectedItem as Label).Tag.ToString();
            detect_type = _detect_type.SelectedIndex < 1 ? "" : (_detect_type.SelectedItem as Label).Tag.ToString();

            grid_info.Children.Add(new UcWarningReportDetails(dbOperation, reportDate_kssj.SelectedDate.ToString(), reportDate_jssj.SelectedDate.ToString(), dept_id, item_id, review_id, dept_name, detect_type));
        }

        private void _export_Click(object sender, RoutedEventArgs e)
        {
            DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_warning_report_hb('{0}','{1}','{2}','{3}','{4}','{5}','{6}',{7},{8})",
                             (Application.Current.Resources["User"] as UserInfo).ID, reportDate_kssj.SelectedDate, reportDate_jssj.SelectedDate,
                              _detect_dept.SelectedIndex < 1 ? "" : (_detect_dept.SelectedItem as Label).Tag,
                              _detect_item.SelectedIndex < 1 ? "" : (_detect_item.SelectedItem as Label).Tag,
                              _review_flag.SelectedIndex < 1 ? "" : (_review_flag.SelectedItem as Label).Tag,
                              _detect_type.SelectedIndex < 1 ? "" : (_detect_type.SelectedItem as Label).Tag,
                             0,
                             _tableview.RowTotal)).Tables[0];

            _tableview.ExportExcel(table);
        }
    }
}
