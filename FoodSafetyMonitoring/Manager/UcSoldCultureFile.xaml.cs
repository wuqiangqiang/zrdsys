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
    /// UcSoldCultureFile.xaml 的交互逻辑
    /// </summary>
    public partial class UcSoldCultureFile : UserControl
    {
        public IDBOperation dbOperation = null;
        private Dictionary<string, MyColumn> MyColumns = new Dictionary<string, MyColumn>();

        string userId = (Application.Current.Resources["User"] as UserInfo).ID;
        public UcSoldCultureFile(IDBOperation dbOperation)
        {
            InitializeComponent();
            this.dbOperation = dbOperation;

            //画面初始化-养殖档案列表画面
            dtpStartDate.SelectedDate = DateTime.Now.AddDays(-1);
            dtpEndDate.SelectedDate = DateTime.Now;
            ComboboxTool.InitComboboxSource(_colony_batch, string.Format("call p_user_colony_batch('{0}')", userId), "cxtj");
            ComboboxTool.InitComboboxSource(_colony_no, string.Format("call p_user_colony('{0}')", userId), "cxtj");

            SetColumns();
        }

        private void SetColumns()
        {
            MyColumns.Add("createdate", new MyColumn("createdate", "建档时间") { BShow = true, Width = 18 });
            MyColumns.Add("culturecompany", new MyColumn("culturecompany", "养殖企业名称") { BShow = true, Width = 18 });
            MyColumns.Add("colonyhouse", new MyColumn("colonyhouse", "圈舍号") { BShow = true, Width = 5 });
            MyColumns.Add("colonybatch", new MyColumn("colonybatch", "圈舍批次") { BShow = true, Width = 15 });
            MyColumns.Add("objecttype", new MyColumn("objecttype", "养殖品种") { BShow = true, Width = 15 });
            MyColumns.Add("createuser", new MyColumn("createuser", "建档人") { BShow = true, Width = 10 });
            MyColumns.Add("sum_num", new MyColumn("sum_num", "总行数") { BShow = false });

            _tableview.MyColumns = MyColumns;
            _tableview.BShowSold = true;
            _tableview.BShowDetails = true;

            _tableview.DetailsRowEnvent += new UcTableOperableView_NoTitle.DetailsRowEventHandler(_tableview_DetailsRowEnvent);
            _tableview.SoldRowEnvent += new UcTableOperableView_NoTitle.SoldRowEventHandler(_tableview_SoldRowEnvent);
        }

        void _tableview_DetailsRowEnvent(string id)
        {
            grid_table.Children.Add(new UcSoldCultureFileDetails(dbOperation, id, (Application.Current.Resources["User"] as UserInfo).DepartmentID));       
        }

        void _tableview_SoldRowEnvent(string id)
        {
            if (Toolkit.MessageBox.Show("确定要对该批次进行出栏操作吗？", "系统询问", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    int result = dbOperation.GetDbHelper().ExecuteSql(string.Format("update t_culture_file set soldflag = '1' where colonybatch ='{0}'", id));
                    if (result > 0)
                    {
                        Toolkit.MessageBox.Show("出栏成功！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        GetData();
                    }
                    else
                    {
                        Toolkit.MessageBox.Show("出栏失败！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                }
                catch
                {
                    Toolkit.MessageBox.Show("出栏失败2！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
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
            //_tableview.Title = string.Format("数据统计时间:{0}年{1}月{2}日到{3}年{4}月{5}日 合计{6}条数据", dtpStartDate.SelectedDate.Value.Year, dtpStartDate.SelectedDate.Value.Month, dtpStartDate.SelectedDate.Value.Day,
            //              dtpEndDate.SelectedDate.Value.Year, dtpEndDate.SelectedDate.Value.Month, dtpEndDate.SelectedDate.Value.Day, _tableview.RowTotal);
            _title.Text = string.Format("合计{0}条数据", _tableview.RowTotal);
            _tableview.PageIndex = 1;
        }

        private void GetData()
        {
            DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_query_sold_culture_file({0},'{1}','{2}','{3}','{4}',{5},{6})",
                   userId,
                   ((DateTime)dtpStartDate.SelectedDate).ToShortDateString(),
                   ((DateTime)dtpEndDate.SelectedDate).ToShortDateString(),
                   _colony_batch.SelectedIndex < 1 ? "" : (_colony_batch.SelectedItem as Label).Tag,
                   _colony_no.SelectedIndex < 1 ? "" : (_colony_no.SelectedItem as Label).Tag,
                   (_tableview.PageIndex - 1) * _tableview.RowMax,
                   _tableview.RowMax)).Tables[0];

            _tableview.Table = table;
        }

        void _tableview_GetDataByPageNumberEvent()
        {
            GetData();
        }

        private void _export_Click(object sender, RoutedEventArgs e)
        {
            DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_query_sold_culture_file({0},'{1}','{2}','{3}','{4}',{5},{6})",
                   userId,
                   ((DateTime)dtpStartDate.SelectedDate).ToShortDateString(),
                   ((DateTime)dtpEndDate.SelectedDate).ToShortDateString(),
                   _colony_batch.SelectedIndex < 1 ? "" : (_colony_batch.SelectedItem as Label).Tag,
                   _colony_no.SelectedIndex < 1 ? "" : (_colony_no.SelectedItem as Label).Tag,
                  0,
                  _tableview.RowTotal)).Tables[0];

            _tableview.ExportExcel(table);
        }
    }
}
