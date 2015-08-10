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
using FoodSafetyMonitoring.Common;
using FoodSafetyMonitoring.dao;
using System.Data;
using FoodSafetyMonitoring.Manager.UserControls;
using Toolkit = Microsoft.Windows.Controls;

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// SysReviewInfo.xaml 的交互逻辑
    /// </summary>
    public partial class SysReviewInfo : UserControl
    {
        private IDBOperation dbOperation;
        private DataTable current_table;
        private Dictionary<string, MyColumn> MyColumns = new Dictionary<string, MyColumn>();
        private string depttype;
        private string detecttype;

        public SysReviewInfo(IDBOperation dbOperation, string dept_type, string detect_type)
        {
            InitializeComponent();

            this.dbOperation = dbOperation;
            this.depttype = dept_type;
            this.detecttype = detect_type;

            //初始化查询条件
            reportDate_kssj.SelectedDate = DateTime.Now.AddDays(-1);
            reportDate_jssj.SelectedDate = DateTime.Now;
            //检测站点
            ComboboxTool.InitComboboxSource(_detect_dept, string.Format("call p_user_dept_hb({0},'{1}')", (Application.Current.Resources["User"] as UserInfo).ID, depttype), "cxtj");
            //检测项目
            ComboboxTool.InitComboboxSource(_detect_item, "SELECT ItemID,ItemNAME FROM t_det_item WHERE  (tradeId ='1'or tradeId ='2' or tradeId ='3' or ifnull(tradeId,'') = '') and OPENFLAG = '1' order by orderId", "cxtj");
            ////检测结果
            //DataTable table_detect_result = new DataTable();
            //table_detect_result.Columns.Add("id", Type.GetType("System.String"));
            //table_detect_result.Columns.Add("name", Type.GetType("System.String"));
            //table_detect_result.Rows.Add(new object[] { "0", "未复核" });
            //table_detect_result.Rows.Add(new object[] { "1", "已复核" });
            //ComboboxTool.InitComboboxSource(_detect_result, table_detect_result,"cxtj");

            //MyColumns.Add("zj", new MyColumn("zj", "主键") { BShow = false });
            //MyColumns.Add("partid", new MyColumn("partid", "检测站点id") { BShow = false });
            //MyColumns.Add("partname", new MyColumn("partname", "检测站点") { BShow = true, Width = 12 });
            //MyColumns.Add("itemid", new MyColumn("itemid", "检测项目id") { BShow = false });
            //MyColumns.Add("itemname", new MyColumn("itemname", "检测项目") { BShow = true, Width = 12 });
            //MyColumns.Add("review_yes", new MyColumn("review_yes", "已复核") { BShow = true, Width = 10 });
            //MyColumns.Add("review_no", new MyColumn("review_no", "未复核") { BShow = true, Width = 10 });
            //MyColumns.Add("count", new MyColumn("count", "合计数量") { BShow = true, Width = 10 });
            //MyColumns.Add("sum_num", new MyColumn("sum_num", "总行数") { BShow = false });
            //_tableview.MyColumns = MyColumns;
            //_tableview.BShowDetails = true;

            //_tableview.DetailsRowEnvent += new UcTableOperableView_NoTitle.DetailsRowEventHandler(_tableview_DetailsRowEnvent);

            MyColumns.Add("orderid", new MyColumn("orderid", "检测单编号") { BShow = true, Width = 8 });
            MyColumns.Add("detecttypename", new MyColumn("detecttypename", "信息来源") { BShow = true, Width = 8 });
            MyColumns.Add("detectdate", new MyColumn("detectdate", "检测时间") { BShow = true, Width = 18 });
            MyColumns.Add("partname", new MyColumn("partname", "检测单位") { BShow = true, Width = 16 });
            MyColumns.Add("itemname", new MyColumn("itemname", "检测项目") { BShow = true, Width = 10 });
            MyColumns.Add("objectname", new MyColumn("objectname", "检测对象") { BShow = true, Width = 8 });
            MyColumns.Add("samplename", new MyColumn("samplename", "检测样本") { BShow = true, Width = 8 });
            MyColumns.Add("sensitivityname", new MyColumn("sensitivityname", "检测灵敏度") { BShow = true, Width = 8 });
            MyColumns.Add("reagentname", new MyColumn("reagentname", "检测方法") { BShow = true, Width = 10 });
            MyColumns.Add("resultname", new MyColumn("resultname", "检测结果") { BShow = true, Width = 8 });
            MyColumns.Add("detectusername", new MyColumn("detectusername", "检测师") { BShow = true, Width = 8 });
            MyColumns.Add("areaname", new MyColumn("areaname", "来源产地") { BShow = false });
            MyColumns.Add("companyname", new MyColumn("companyname", "被检单位") { BShow = true, Width = 16 });
            MyColumns.Add("reviewflagname", new MyColumn("reviewflagname", "是否复核") { BShow = false });
            MyColumns.Add("sum_num", new MyColumn("sum_num", "总行数") { BShow = false });

            _tableview.MyColumns = MyColumns;
            _tableview.BShowState = true;
            _tableview.StateRowEnvent += new UcTableOperableView_NoTitle.StateRowEventHandler(_tableview_StateRowEnvent);            
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
            //_tableview.Title = string.Format("数据统计时间:{0}年{1}月{2}日到{3}年{4}月{5}日  合计{6}条数据", reportDate_kssj.Value.Value.Year, reportDate_kssj.Value.Value.Month, reportDate_kssj.Value.Value.Day,
            //              reportDate_jssj.Value.Value.Year, reportDate_jssj.Value.Value.Month, reportDate_jssj.Value.Value.Day, _tableview.RowTotal);
            _title.Text = string.Format("合计{0}条数据", _tableview.RowTotal);
            _tableview.PageIndex = 1;

        }

        public void GetData()
        {
            //DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_review_info('{0}','{1}','{2}','{3}','{4}','{5}',{6},{7})",
            //                  (Application.Current.Resources["User"] as UserInfo).ID, reportDate_kssj.Value, reportDate_jssj.Value,
            //                  _detect_dept.SelectedIndex < 1 ? "" : (_detect_dept.SelectedItem as Label).Tag,
            //                  _detect_item.SelectedIndex < 1 ? "" : (_detect_item.SelectedItem as Label).Tag,
            //                  _detect_result.SelectedIndex < 1 ? "" : (_detect_result.SelectedItem as Label).Tag,
            //                  (_tableview.PageIndex - 1) * _tableview.RowMax,
            //                  _tableview.RowMax)).Tables[0];

            DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_review_details_hb('{0}','{1}','{2}','{3}','{4}','{5}','{6}',{7},{8})",
                               (Application.Current.Resources["User"] as UserInfo).ID, reportDate_kssj.SelectedDate, reportDate_jssj.SelectedDate,
                               _detect_dept.SelectedIndex < 1 ? "" : (_detect_dept.SelectedItem as Label).Tag,
                               _detect_item.SelectedIndex < 1 ? "" : (_detect_item.SelectedItem as Label).Tag,
                               depttype,detecttype,
                              (_tableview.PageIndex - 1) * _tableview.RowMax,
                              _tableview.RowMax)).Tables[0];

            _tableview.Table = table;
            current_table = table;
        }

        void _tableview_GetDataByPageNumberEvent()
        {
            GetData();
        }

        //void _tableview_DetailsRowEnvent(string id)
        //{
        //    string dept_id;
        //    string item_id;

        //    int selectrow = int.Parse(id);

        //    dept_id = current_table.Rows[selectrow - 1][1].ToString();
        //    item_id = current_table.Rows[selectrow - 1][3].ToString();

        //    grid_info.Children.Add(new UcReviewdetails(dbOperation, reportDate_kssj.Value.ToString(), reportDate_jssj.Value.ToString(), dept_id, item_id, _detect_result.SelectedIndex < 1 ? "" : (_detect_result.SelectedItem as Label).Tag.ToString()));
        //}

        void _tableview_StateRowEnvent(string id)
        {
            int orderid = int.Parse(id);
            if (detecttype == "0")
            {
                Culture_AddReviewDetails det = new Culture_AddReviewDetails(dbOperation, orderid,this);
                det.ShowDialog();
            }
            else if (detecttype == "1")
            {
                Certificate_AddReviewDetails det = new Certificate_AddReviewDetails(dbOperation, orderid,this);
                det.ShowDialog();
            }
            else if (detecttype == "2")
            {
                Slaughter_AddReviewDetails det = new Slaughter_AddReviewDetails(dbOperation, orderid, this);
                det.ShowDialog();
            }
            else if (detecttype == "3")
            {
                Feed_AddReviewDetails det = new Feed_AddReviewDetails(dbOperation, orderid, this);
                det.ShowDialog();
            }
            else if (detecttype == "")
            {
                AddReviewDetails det = new AddReviewDetails(dbOperation, orderid, this);
                det.ShowDialog();
            }       

        }

        private void _export_Click(object sender, RoutedEventArgs e)
        {
            DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_review_details_hb('{0}','{1}','{2}','{3}','{4}','{5}','{6}',{7},{8})",
                               (Application.Current.Resources["User"] as UserInfo).ID, reportDate_kssj.SelectedDate, reportDate_jssj.SelectedDate,
                               _detect_dept.SelectedIndex < 1 ? "" : (_detect_dept.SelectedItem as Label).Tag,
                               _detect_item.SelectedIndex < 1 ? "" : (_detect_item.SelectedItem as Label).Tag,
                               depttype, detecttype,
                               0,
                              _tableview.RowTotal)).Tables[0];

            _tableview.ExportExcel(table);
        }

        //private void btnDetails_Click(object sender, RoutedEventArgs e)
        //{
        //    string dept_id;
        //    string item_id;

        //    int selectrow = int.Parse((sender as Button).Tag.ToString());

        //    dept_id = current_table.Rows[selectrow - 1][0].ToString();
        //    item_id = current_table.Rows[selectrow - 1][2].ToString();

        //    grid_info.Children.Add(new UcReviewdetails(dbOperation, reportDate_kssj.Value.ToString(), reportDate_jssj.Value.ToString(), dept_id, item_id, _detect_result.SelectedIndex < 1 ? "" : (_detect_result.SelectedItem as Label).Tag.ToString()));
            
        //}
    }
}
