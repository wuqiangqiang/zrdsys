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
using System.Data;
using FoodSafetyMonitoring.Common;
using Toolkit = Microsoft.Windows.Controls;
using FoodSafetyMonitoring.Manager.UserControls;

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// UcCreateCertificatequery.xaml 的交互逻辑
    /// </summary>
    public partial class UcCreateCertificatequery : UserControl
    {
        DataTable ProvinceCityTable;
        public IDBOperation dbOperation = null;
        private Dictionary<string, MyColumn> MyColumns = new Dictionary<string, MyColumn>();

        string userId = (Application.Current.Resources["User"] as UserInfo).ID;

        public UcCreateCertificatequery(IDBOperation dbOperation)
        {
            InitializeComponent();
            this.dbOperation = dbOperation;
            ProvinceCityTable = Application.Current.Resources["省市表"] as DataTable;
            DataRow[] rows = ProvinceCityTable.Select("pid = '0001'");

            dtpStartDate.SelectedDate = DateTime.Now.AddDays(-1);
            dtpEndDate.SelectedDate = DateTime.Now;

            ComboboxTool.InitComboboxSource(_province, rows, "lr");
            _province.SelectionChanged += new SelectionChangedEventHandler(_province_SelectionChanged);
            //ComboboxTool.InitComboboxSource(_source_company1, string.Format(" call p_provice_dept_hb('{0}','yz') ", userId), "cxtj");
            ComboboxTool.InitComboboxSource(_source_company, "SELECT COMPANYID,COMPANYNAME FROM t_company", "cxtj");
            ComboboxTool.InitComboboxSource(_certificate_station, string.Format("call p_user_dept_hb('{0}','cz')", userId), "cxtj");
            _certificate_station.SelectionChanged += new SelectionChangedEventHandler(_certificate_station_SelectionChanged);
            ComboboxTool.InitComboboxSource(_detect_person1, string.Format("call p_user_detuser_hb('{0}','cz')", userId), "cxtj");

            //SetColumns();
        }

        //private void SetColumns()
        //{
        //    MyColumns.Add("cardid", new MyColumn("cardid", "检疫证号") { BShow = true, Width = 12 });
        //    MyColumns.Add("cdate", new MyColumn("cdate", "出证时间") { BShow = true, Width = 16 });
        //    MyColumns.Add("companyname", new MyColumn("companyname", "来源单位名称") { BShow = true, Width = 15 });
        //    MyColumns.Add("objectcount", new MyColumn("objectcount", "批次头数") { BShow = true, Width = 8 });
        //    MyColumns.Add("objectlable", new MyColumn("objectlable", "耳标号") { BShow = true, Width = 16 });
        //    MyColumns.Add("deptname", new MyColumn("deptname", "出证站点") { BShow = true, Width = 16 });
        //    MyColumns.Add("username", new MyColumn("username", "出证人") { BShow = true, Width = 16 });
        //    MyColumns.Add("sum_num", new MyColumn("sum_num", "总行数") { BShow = false });

        //    _tableview.MyColumns = MyColumns;
        //    _tableview.BShowModify = false;
        //    _tableview.BShowDetails = true;
        //    _tableview.BShowDelete = false;

        //    _tableview.DetailsRowEnvent += new UcTableOperableView_NoTitle.DetailsRowEventHandler(_tableview_DetailsRowEnvent);
        //}


        void _province_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_province.SelectedIndex > 0)
            {
                DataRow[] rows = ProvinceCityTable.Select("pid = '" + (_province.SelectedItem as Label).Tag.ToString() + "'");
                ComboboxTool.InitComboboxSource(_city, rows, "cxtj");
                //20150707来源单位改为连动（受来源区域影响）
                //ComboboxTool.InitComboboxSource(_source_company1, string.Format(" call p_user_company('{0}','{1}') ", userId, (_province1.SelectedItem as Label).Tag.ToString()), "cxtj");
                ComboboxTool.InitComboboxSource(_source_company, "SELECT COMPANYID,COMPANYNAME FROM t_company where AREAID like '" + (_province.SelectedItem as Label).Tag + "%'", "cxtj");
                _city.SelectionChanged += new SelectionChangedEventHandler(_city_SelectionChanged);
            }
        }


        void _city_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_city.SelectedIndex > 0)
            {
                DataRow[] rows = ProvinceCityTable.Select("pid = '" + (_city.SelectedItem as Label).Tag.ToString() + "'");
                ComboboxTool.InitComboboxSource(_region, rows, "cxtj");
                //20150707来源单位改为连动（受来源区域影响）
                //ComboboxTool.InitComboboxSource(_source_company1, string.Format(" call p_user_company('{0}','{1}') ", userId, (_city1.SelectedItem as Label).Tag.ToString()), "cxtj");
                ComboboxTool.InitComboboxSource(_source_company, "SELECT COMPANYID,COMPANYNAME FROM t_company where AREAID like '" + (_city.SelectedItem as Label).Tag + "%'", "cxtj");
                _region.SelectionChanged += new SelectionChangedEventHandler(_region_SelectionChanged);
            }
        }

        //20150707来源单位改为连动（受来源区域影响）
        void _region_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_region.SelectedIndex > 0)
            {
                //ComboboxTool.InitComboboxSource(_source_company1, string.Format("call p_user_company('{0}','{1}')", userId, (_region1.SelectedItem as Label).Tag.ToString()), "cxtj");
                ComboboxTool.InitComboboxSource(_source_company, "SELECT COMPANYID,COMPANYNAME FROM t_company where AREAID =" + (_region.SelectedItem as Label).Tag.ToString(), "cxtj");
            }
        }
        void _certificate_station_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_certificate_station.SelectedIndex > 0)
            {
                ComboboxTool.InitComboboxSource(_detect_person1, string.Format("SELECT RECO_PKID,INFO_USER,NUMB_USER FROM sys_client_user where fk_dept = '{0}'", (_certificate_station.SelectedItem as Label).Tag.ToString()), "cxtj");
            }
            else if (_certificate_station.SelectedIndex == 0)
            {
                ComboboxTool.InitComboboxSource(_detect_person1, string.Format("call p_user_detuser_hb('{0}','cz')", userId), "cxtj");
            }
        }

        private void _query_Click(object sender, RoutedEventArgs e)
        {
            if (dtpStartDate.SelectedDate.Value.Date > dtpEndDate.SelectedDate.Value.Date)
            {
                Toolkit.MessageBox.Show("开始时间大于结束时间，请重新选择！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            //清空列表
            lvlist.DataContext = null;

            DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_query_certificate({0},'{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')",
                   (Application.Current.Resources["User"] as UserInfo).ID,
                   ((DateTime)dtpStartDate.SelectedDate).ToShortDateString(),
                   ((DateTime)dtpEndDate.SelectedDate).ToShortDateString(),
                   _province.SelectedIndex < 1 ? "" : (_province.SelectedItem as Label).Tag,
                   _city.SelectedIndex < 1 ? "" : (_city.SelectedItem as Label).Tag,
                   _region.SelectedIndex < 1 ? "" : (_region.SelectedItem as Label).Tag,
                   _source_company.SelectedIndex < 1 ? "" : (_source_company.SelectedItem as Label).Tag,
                    _certificate_station.SelectedIndex < 1 ? "" : (_certificate_station.SelectedItem as Label).Tag,
                   _detect_person1.SelectedIndex < 1 ? "" : (_detect_person1.SelectedItem as Label).Tag)).Tables[0];

            lvlist.DataContext = table;

            _title.Text = string.Format("合计{0}条数据", table.Rows.Count);

        }

        private void _btn_details_Click(object sender, RoutedEventArgs e)
        {
            string batch_no = (sender as Button).Tag.ToString();

            grid_info.Children.Add(new UcCreateCertificatedetails(dbOperation, batch_no));
        }

        private void _btn_card_Click(object sender, RoutedEventArgs e)
        {

        }

        //private void GetData()
        //{
        //    DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_query_certificate({0},'{1}','{2}','{3}','{4}','{5}',{6},{7})",
        //           (Application.Current.Resources["User"] as UserInfo).ID,
        //           ((DateTime)dtpStartDate.SelectedDate).ToShortDateString(),
        //           ((DateTime)dtpEndDate.SelectedDate).ToShortDateString(),
        //           _source_company1.SelectedIndex < 1 ? "" : (_source_company1.SelectedItem as Label).Tag,
        //            _certificate_station.SelectedIndex < 1 ? "" : (_certificate_station.SelectedItem as Label).Tag,
        //           _detect_person1.SelectedIndex < 1 ? "" : (_detect_person1.SelectedItem as Label).Tag,
        //           (_tableview.PageIndex - 1) * _tableview.RowMax,
        //           _tableview.RowMax)).Tables[0];

        //    _tableview.Table = table;
        //}

        //void _tableview_GetDataByPageNumberEvent()
        //{
        //    GetData();
        //}

        //void _tableview_DetailsRowEnvent(string id)
        //{
        //    string cardid = id;
        //    grid_table.Children.Add(new UcCreateCertificateQueryDetails(dbOperation, cardid));
        //}

        //private void _export_Click(object sender, RoutedEventArgs e)
        //{
        //    DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_query_certificate({0},'{1}','{2}','{3}','{4}','{5}',{6},{7})",
        //           (Application.Current.Resources["User"] as UserInfo).ID,
        //           ((DateTime)dtpStartDate.SelectedDate).ToShortDateString(),
        //           ((DateTime)dtpEndDate.SelectedDate).ToShortDateString(),
        //           _source_company1.SelectedIndex < 1 ? "" : (_source_company1.SelectedItem as Label).Tag,
        //            _certificate_station.SelectedIndex < 1 ? "" : (_certificate_station.SelectedItem as Label).Tag,
        //           _detect_person1.SelectedIndex < 1 ? "" : (_detect_person1.SelectedItem as Label).Tag,
        //          0,
        //          _tableview.RowTotal)).Tables[0];

        //    _tableview.ExportExcel(table);
        //}

    }
}
