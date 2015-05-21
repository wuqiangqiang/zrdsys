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
            reportDate_kssj.Value = DateTime.Now.AddDays(-1);
            reportDate_jssj.Value = DateTime.Now;
            //检测站点
            switch (user_flag_tier)
            {
                case "0": _dept_name.Text = "省:";
                    dept_name = "省名称";
                    break;
                case "1": _dept_name.Text = "地市:";
                    dept_name = "地市名称";
                    break;
                case "2": _dept_name.Text = "区县:";
                    dept_name = "区县名称";
                    break;
                case "3": _dept_name.Text = "检测站点:";
                    dept_name = "检测站点名称";
                    break;
                case "4": _dept_name.Text = "检测站点:";
                    dept_name = "检测站点名称";
                    break;
                default: break;
            }
            ComboboxTool.InitComboboxSource(_detect_dept, "call p_dept_cxtj(" + (Application.Current.Resources["User"] as UserInfo).ID + ")", "cxtj");
            //检测项目
            ComboboxTool.InitComboboxSource(_detect_item, "SELECT ItemID,ItemNAME FROM t_det_item WHERE  (tradeId ='1'or tradeId ='2' or tradeId ='3' or ifnull(tradeId,'') = '') and OPENFLAG = '1' order by orderId", "cxtj");
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

        private void _query_Click(object sender, RoutedEventArgs e)
        {
            _tableview.GetDataByPageNumberEvent += new UcTableOperableView_NoTitle.GetDataByPageNumberEventHandler(_tableview_GetDataByPageNumberEvent);
            GetData();
            _tableview.Title = string.Format("数据统计时间:{0}年{1}月{2}日到{3}年{4}月{5}日", reportDate_kssj.Value.Value.Year, reportDate_kssj.Value.Value.Month, reportDate_kssj.Value.Value.Day,
                          reportDate_jssj.Value.Value.Year, reportDate_jssj.Value.Value.Month, reportDate_jssj.Value.Value.Day);
            _title.Text = string.Format("▪ 数据统计时间:{0}年{1}月{2}日到{3}年{4}月{5}日", reportDate_kssj.Value.Value.Year, reportDate_kssj.Value.Value.Month, reportDate_kssj.Value.Value.Day,
                          reportDate_jssj.Value.Value.Year, reportDate_jssj.Value.Value.Month, reportDate_jssj.Value.Value.Day);
            _tableview.PageIndex = 1;
        }

        private void GetData()
        {
            DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_warning_report('{0}','{1}','{2}','{3}','{4}','{5}',{6},{7})",
                             (Application.Current.Resources["User"] as UserInfo).ID, reportDate_kssj.Value, reportDate_jssj.Value,
                              _detect_dept.SelectedIndex < 1 ? "" : (_detect_dept.SelectedItem as Label).Tag,
                              _detect_item.SelectedIndex < 1 ? "" : (_detect_item.SelectedItem as Label).Tag,
                              _review_flag.SelectedIndex < 1 ? "" : (_review_flag.SelectedItem as Label).Tag,
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

            dept_id = id;
            item_id = _detect_item.SelectedIndex < 1 ? "" : (_detect_dept.SelectedItem as Label).Tag.ToString();
            review_id = _review_flag.SelectedIndex < 1 ? "" : (_review_flag.SelectedItem as Label).Tag.ToString();

            grid_info.Children.Add(new UcWarningReportDetails(dbOperation, reportDate_kssj.Value.ToString(), reportDate_jssj.Value.ToString(), dept_id, item_id, review_id, dept_name));
        }

        private void _export_Click(object sender, RoutedEventArgs e)
        {
            _tableview.ExportExcel();
        }
    }
}
