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
    /// UcUserCheckDetails.xaml 的交互逻辑
    /// </summary>
    public partial class UcUserCheckDetails : UserControl
    {
        private IDBOperation dbOperation;
        private Dictionary<string, MyColumn> MyColumns = new Dictionary<string, MyColumn>();
        public string UserId { get; set; }
        public string Kssj { get; set; }
        public string Jssj { get; set; }
        public UcUserCheckDetails(IDBOperation dbOperation, string userid,string kssj,string jssj)
        {
            InitializeComponent();

            this.dbOperation = dbOperation;
            this.UserId = userid;
            this.Kssj = kssj;
            this.Jssj = jssj;

            MyColumns.Add("username", new MyColumn("username", "检测师名称") { BShow = true, Width = 10 });
            MyColumns.Add("checkdate", new MyColumn("checkdate", "签到时间") { BShow = true, Width = 18 });
            MyColumns.Add("checkaddress", new MyColumn("checkaddress", "签到地点") { BShow = true, Width = 18 });
            MyColumns.Add("sum_num", new MyColumn("sum_num", "总行数") { BShow = false });

            _tableview.MyColumns = MyColumns;
            _tableview.GetDataByPageNumberEvent += new UcTableOperableView_NoTitle.GetDataByPageNumberEventHandler(_tableview_GetDataByPageNumberEvent);
            _tableview.PageIndex = 1;
            GetData();
        }

        private void GetData()
        {
            DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_user_check_details('{0}','{1}','{2}',{3},{4})",
                                Kssj, Jssj, UserId,
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
