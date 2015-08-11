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
    /// SysDetectUserManager.xaml 的交互逻辑
    /// </summary>
    public partial class SysDetectUserManager : UserControl
    {
        public IDBOperation dbOperation = null;
        private Dictionary<string, MyColumn> MyColumns = new Dictionary<string, MyColumn>();

        string userId = (Application.Current.Resources["User"] as UserInfo).ID;
        string user_flag_tier = (Application.Current.Resources["User"] as UserInfo).FlagTier;
        private string dept_name;
        public SysDetectUserManager(IDBOperation dbOperation)
        {
            InitializeComponent();

            this.dbOperation = dbOperation;

            //画面初始化-检测单列表画面
            dtpStartDate.SelectedDate = DateTime.Now.AddDays(-1);
            dtpEndDate.SelectedDate = DateTime.Now;

            switch (user_flag_tier)
            {
                case "0": _dept_name.Text = "选择省:";
                    dept_name = "省名称";
                    break;
                case "1": _dept_name.Text = "选择市州:";
                    dept_name = "市州名称";
                    break;
                case "2": _dept_name.Text = "选择区县:";
                    dept_name = "区县名称";
                    break;
                case "3": _dept_name.Text = "选择检测站点:";
                    dept_name = "检测站点名称";
                    break;
                case "4": _dept_name.Text = "选择检测站点:";
                    dept_name = "检测站点名称";
                    break;
                default: break;
            }

            //检测站点
            ComboboxTool.InitComboboxSource(_detect_station, string.Format("call p_dept_cxtj({0})", userId), "cxtj");
            _detect_station.SelectionChanged += new SelectionChangedEventHandler(_detect_station_SelectionChanged);
            ComboboxTool.InitComboboxSource(_detect_person1, string.Format("call p_user_detuser('{0}')", userId), "cxtj");

            SetColumns();
        }

        private void SetColumns()
        {
            MyColumns.Add("userid", new MyColumn("userid", "用户id") { BShow = false });
            MyColumns.Add("part_name", new MyColumn("part_name", dept_name) { BShow = true, Width = 15 });
            MyColumns.Add("username", new MyColumn("username", "检测师名称") { BShow = true, Width = 10 });
            MyColumns.Add("check_count", new MyColumn("check_count", "签到次数") { BShow = true, Width = 10 });
            MyColumns.Add("sum_num", new MyColumn("sum_num", "总行数") { BShow = false });

            _tableview.MyColumns = MyColumns;
            _tableview.BShowDetails = true;

            _tableview.DetailsRowEnvent += new UcTableOperableView_NoTitle.DetailsRowEventHandler(_tableview_DetailsRowEnvent);
        }

        //20150707检测师改为连动（受检测站点影响）
        void _detect_station_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_detect_station.SelectedIndex > 0)
            {
                ComboboxTool.InitComboboxSource(_detect_person1, string.Format("SELECT RECO_PKID,INFO_USER,NUMB_USER FROM sys_client_user where fk_dept like '{0}%' ", (_detect_station.SelectedItem as Label).Tag.ToString()), "cxtj");
            }
            else if (_detect_station.SelectedIndex == 0)
            {
                ComboboxTool.InitComboboxSource(_detect_person1, string.Format("call p_user_detuser('{0}')", userId), "cxtj");
            }
        }

        private void _query_Click(object sender, RoutedEventArgs e)
        {
            if (dtpStartDate.SelectedDate.Value.Date > dtpEndDate.SelectedDate.Value.Date)
            {
                Toolkit.MessageBox.Show("开始时间大于结束时间，请重新选择！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            _tableview.GetDataByPageNumberEvent += new UcTableOperableView_NoTitle.GetDataByPageNumberEventHandler(_tableview_GetDataByPageNumberEvent);
            GetData();
            _title.Text = string.Format("合计{0}条数据", _tableview.RowTotal);
            _tableview.PageIndex = 1;
        }

        private void GetData()
        {
            DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_user_check({0},'{1}','{2}','{3}','{4}',{5},{6})",
                  (Application.Current.Resources["User"] as UserInfo).ID,
                  ((DateTime)dtpStartDate.SelectedDate).ToShortDateString(),
                  ((DateTime)dtpEndDate.SelectedDate).ToShortDateString(),
                   _detect_station.SelectedIndex < 1 ? "" : (_detect_station.SelectedItem as Label).Tag,
                  _detect_person1.SelectedIndex < 1 ? "" : (_detect_person1.SelectedItem as Label).Tag,
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
            grid_table.Children.Add(new UcUserCheckDetails(dbOperation, id,((DateTime)dtpStartDate.SelectedDate).ToShortDateString(),((DateTime)dtpEndDate.SelectedDate).ToShortDateString()));
        }

    }
}
