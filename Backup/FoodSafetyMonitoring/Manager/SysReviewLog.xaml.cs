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
using System.Data;
using FoodSafetyMonitoring.dao;
using FoodSafetyMonitoring.Manager.UserControls;
using FoodSafetyMonitoring.Common;

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// SysReviewLog.xaml 的交互逻辑
    /// </summary>
    public partial class SysReviewLog : UserControl
    {
        private IDBOperation dbOperation;
        private Dictionary<string, MyColumn> MyColumns = new Dictionary<string, MyColumn>();

        public SysReviewLog(IDBOperation dbOperation)
        {
            InitializeComponent();

            this.dbOperation = dbOperation;

            //初始化查询条件
            reportDate_kssj.Value = DateTime.Now.AddDays(-1);
            reportDate_jssj.Value = DateTime.Now;
            //检测项目
            ComboboxTool.InitComboboxSource(_detect_item, "SELECT ItemID,ItemNAME FROM t_det_item WHERE  (tradeId ='1'or tradeId ='2' or tradeId ='3' or ifnull(tradeId,'') = '') and OPENFLAG = '1' order by orderId", "cxtj");

            MyColumns.Add("orderid", new MyColumn("orderid", "检测单编号") { BShow = true, Width = 10 });
            MyColumns.Add("itemname", new MyColumn("itemname", "检测项目") { BShow = true, Width = 14 });
            MyColumns.Add("reviewdate", new MyColumn("reviewdate", "复核检测时间") { BShow = true, Width = 18 });
            MyColumns.Add("reviewreagentname", new MyColumn("reviewreagentname", "复核检测方法") { BShow = true, Width = 14 });
            MyColumns.Add("reviewresultname", new MyColumn("reviewresultname", "复核检测结果") { BShow = true, Width = 14 });
            MyColumns.Add("reviewusername", new MyColumn("reviewusername", "复核检测师") { BShow = true, Width = 14 });
            MyColumns.Add("reviewreason", new MyColumn("reviewreason", "复核原因说明") { BShow = true, Width = 22 });
            MyColumns.Add("sum_num", new MyColumn("sum_num", "总行数") { BShow = false });

            _tableview.MyColumns = MyColumns;
            _tableview.BShowDetails = true;
            _tableview.DetailsRowEnvent += new UcTableOperableView.DetailsRowEventHandler(_tableview_DetailsRowEnvent);
        }

        private void _query_Click(object sender, RoutedEventArgs e)
        {
            _tableview.GetDataByPageNumberEvent += new UcTableOperableView.GetDataByPageNumberEventHandler(_tableview_GetDataByPageNumberEvent);
            GetData();
            _tableview.Title = string.Format("数据统计时间:{0}年{1}月{2}日到{3}年{4}月{5}日", reportDate_kssj.Value.Value.Year, reportDate_kssj.Value.Value.Month, reportDate_kssj.Value.Value.Day,
                          reportDate_jssj.Value.Value.Year, reportDate_jssj.Value.Value.Month, reportDate_jssj.Value.Value.Day);
            _tableview.PageIndex = 1;
        }

        private void GetData()
        {
            DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_review_log('{0}','{1}','{2}','{3}','{4}','{5}')",
                              (Application.Current.Resources["User"] as UserInfo).ID, reportDate_kssj.Value, reportDate_jssj.Value,
                               _detect_item.SelectedIndex < 1 ? "" : (_detect_item.SelectedItem as Label).Tag,
                              (_tableview.PageIndex - 1) * _tableview.RowMax,
                              _tableview.RowMax)).Tables[0];
            _tableview.Table = table;
        }

        void _tableview_GetDataByPageNumberEvent()
        {
            GetData();
        }

        void _tableview_DetailsRowEnvent(string id)
        {
            int orderid = int.Parse(id);
            detectDetailsReview det = new detectDetailsReview(dbOperation, orderid);
            det.ShowDialog();
        }

        private void _export_Click(object sender, RoutedEventArgs e)
        {
            _tableview.ExportExcel();
        }

    }
}

