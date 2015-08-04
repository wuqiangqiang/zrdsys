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

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// UcCultureFileReportDetails.xaml 的交互逻辑
    /// </summary>
    public partial class UcCultureFileReportDetails : UserControl
    {
        private IDBOperation dbOperation;

        private Dictionary<string, MyColumn> MyColumns = new Dictionary<string, MyColumn>();
        public string Kssj { get; set; }
        public string Jssj { get; set; }
        public string DeptId { get; set; }
        public UcCultureFileReportDetails(IDBOperation dbOperation, string kssj, string jssj, string dept_id)
        {
            InitializeComponent();
            this.dbOperation = dbOperation;
            this.Kssj = kssj;
            this.Jssj = jssj;
            this.DeptId = dept_id;

            MyColumns.Add("createdate", new MyColumn("createdate", "建档时间") { BShow = true, Width = 18 });
            MyColumns.Add("culturecompany", new MyColumn("culturecompany", "养殖企业名称") { BShow = true, Width = 18 });
            MyColumns.Add("colonyhouse", new MyColumn("colonyhouse", "圈舍号") { BShow = true, Width = 5 });
            MyColumns.Add("fileno", new MyColumn("fileno", "档案编号") { BShow = true, Width = 15 });
            MyColumns.Add("objecttype", new MyColumn("objecttype", "养殖品种") { BShow = true, Width = 15 });
            MyColumns.Add("createuser", new MyColumn("createuser", "建档人") { BShow = true, Width = 10 });
            MyColumns.Add("colonybatch", new MyColumn("colonybatch", "圈舍批次") { BShow = true, Width = 12 });
            MyColumns.Add("solddate", new MyColumn("solddate", "出栏时间") { BShow = true, Width = 18 });
            MyColumns.Add("solduserid", new MyColumn("solduserid", "出栏操作人") { BShow = true, Width = 10 });
            MyColumns.Add("soldflag", new MyColumn("soldflag", "出栏状态") { BShow = true, Width = 8 });
            MyColumns.Add("sum_num", new MyColumn("sum_num", "总行数") { BShow = false });

            _tableview.MyColumns = MyColumns;
            _tableview.GetDataByPageNumberEvent += new UcTableOperableView_NoTitle.GetDataByPageNumberEventHandler(_tableview_GetDataByPageNumberEvent);
            _tableview.PageIndex = 1;
            GetData();
        }

        private void GetData()
        {
            DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_report_culture_file_details('{0}','{1}','{2}',{3},{4})",
                                Kssj,Jssj, DeptId,
                              (_tableview.PageIndex - 1) * _tableview.RowMax,
                              _tableview.RowMax)).Tables[0];

            _tableview.Table = table;
        }

        void _tableview_GetDataByPageNumberEvent()
        {
            GetData();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

    }
}