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
using FoodSafetyMonitoring.Manager.UserControls;

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// SysTaskCheck.xaml 的交互逻辑
    /// </summary>
    public partial class SysTaskCheck : UserControl
    {
        private IDBOperation dbOperation;
        private DataTable currenttable;
        private Dictionary<string, MyColumn> MyColumns = new Dictionary<string, MyColumn>();
        private string user_flag_tier;
        private string dept_name;

        private readonly List<string> year = new List<string>() { "2010",
            "2011", 
            "2012",
            "2013",
            "2014",
            "2015",
            "2016",
            "2017"};//初始化变量

        private readonly List<string> month = new List<string>() { "01",
            "02", 
            "03",
            "04",
            "05",
            "06",
            "07",
            "08",
            "09",
            "10",
            "11",
            "12"};//初始化变量

        public SysTaskCheck(IDBOperation dbOperation)
        {
            this.dbOperation = dbOperation;

            InitializeComponent();

            user_flag_tier = (Application.Current.Resources["User"] as UserInfo).FlagTier;

            _year.ItemsSource = year;
            _year.SelectedIndex = 5;

            _month.ItemsSource = month;
            _month.SelectedIndex = 4;

            loadTaskGrade();

            switch (user_flag_tier)
            {
                case "0": dept_name = "省名称";
                    break;
                case "1": dept_name = "地市名称";
                    break;
                case "2": dept_name = "区县名称";
                    break;
                case "3": dept_name = "检测站点名称";
                    break;
                case "4": dept_name = "检测站点名称";
                    break;
                default: break;
            }

            MyColumns.Add("part_id", new MyColumn("part_id", "检测站点id") { BShow = false });
            MyColumns.Add("part_name", new MyColumn("part_name", dept_name) { BShow = true, Width = 16 });
            MyColumns.Add("task_theory", new MyColumn("task_theory", "月度任务量") { BShow = true, Width = 14 });
            MyColumns.Add("task_actual", new MyColumn("task_actual", "月度实际完成量") { BShow = true, Width = 14 });
            MyColumns.Add("task_percent", new MyColumn("task_percent", "月度任务完成率") { BShow = true, Width = 14 });
            MyColumns.Add("task_gradename", new MyColumn("task_gradename", "评级") { BShow = true, Width = 10 });
            MyColumns.Add("sum_num", new MyColumn("sum_num", "总行数") { BShow = false });

            _tableview.MyColumns = MyColumns;
            //_tableview.BShowDetails = true;
            _tableview.DetailsRowEnvent += new UcTableOperableView.DetailsRowEventHandler(_tableview_DetailsRowEnvent);

        }

        private void _query_Click(object sender, RoutedEventArgs e)
        {
            _tableview.GetDataByPageNumberEvent += new UcTableOperableView.GetDataByPageNumberEventHandler(_tableview_GetDataByPageNumberEvent);
            
            GetData();
            _tableview.Title = _year.Text + "." + _month.Text + "月 检测任务执行绩效考评结果";
        }

        private void GetData()
        {
            DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_task_check('{0}','{1}','{2}',{3},{4})",
                              (Application.Current.Resources["User"] as UserInfo).ID, _year.Text, _month.Text,
                              (_tableview.PageIndex - 1) * _tableview.RowMax,
                              _tableview.RowMax)).Tables[0];

            _tableview.Table = table;
            currenttable = table;
        }

        void _tableview_GetDataByPageNumberEvent()
        {
            GetData();
        }

        void _tableview_DetailsRowEnvent(string id)
        {
        
        }

        private void _export_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            string grade_id = (sender as Button).Tag.ToString();
            SetTaskGrade setgrade = new SetTaskGrade(dbOperation, grade_id, (Application.Current.Resources["User"] as UserInfo).DepartmentID, currenttable,this);
            setgrade.ShowDialog();
        }

        //加载考评参数设置值
        public void loadTaskGrade()
        {
            DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_task_grade('{0}')",
                             (Application.Current.Resources["User"] as UserInfo).ID)).Tables[0];

            lvlist2.DataContext = null;
            lvlist2.DataContext = table;

            currenttable = table;
        }

        private void _tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
