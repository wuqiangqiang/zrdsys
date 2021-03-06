﻿using System;
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
    /// UcQueryCultureFile.xaml 的交互逻辑
    /// </summary>
    public partial class UcQueryCultureFile : UserControl
    {
        public IDBOperation dbOperation = null;
        private Dictionary<string, MyColumn> MyColumns = new Dictionary<string, MyColumn>();

        string userId = (Application.Current.Resources["User"] as UserInfo).ID;

        public UcQueryCultureFile(IDBOperation dbOperation)
        {
            InitializeComponent();
            this.dbOperation = dbOperation;

            //画面初始化-养殖档案列表画面
            dtpStartDate.SelectedDate = DateTime.Now.AddDays(-1);
            dtpEndDate.SelectedDate = DateTime.Now;
            ComboboxTool.InitComboboxSource(_colony_no, string.Format("call p_user_colony('{0}')", userId), "cxtj");
            ComboboxTool.InitComboboxSource(_object_type, "SELECT ObjectTypeId,ObjectTypeName FROM t_culture_type where OpenFlag = '1'", "cxtj");
            ComboboxTool.InitComboboxSource(_culture_company, string.Format("call p_user_dept_hb('{0}','yz')", userId), "cxtj");
            ComboboxTool.InitComboboxSource(_file_cuserid, string.Format("call p_user_detuser_hb('{0}','yz')", userId), "cxtj");
            _culture_company.SelectionChanged += new SelectionChangedEventHandler(_culture_company_SelectionChanged);

            SetColumns();
        }

        private void SetColumns()
        {
            MyColumns.Add("createdate", new MyColumn("createdate", "建档时间") { BShow = true, Width = 14 });
            MyColumns.Add("culturecompany", new MyColumn("culturecompany", "养殖企业名称") { BShow = true, Width = 18 });
            MyColumns.Add("colonyhouse", new MyColumn("colonyhouse", "圈舍号") { BShow = true, Width = 4 });
            MyColumns.Add("fileno", new MyColumn("fileno", "档案编号") { BShow = true, Width = 14 });
            MyColumns.Add("objecttype", new MyColumn("objecttype", "养殖品种") { BShow = true, Width = 10 });
            MyColumns.Add("createuser", new MyColumn("createuser", "建档人") { BShow = true, Width = 10 });
            MyColumns.Add("colonybatch", new MyColumn("colonybatch", "圈舍批次") { BShow = true, Width = 12 });
            MyColumns.Add("solddate", new MyColumn("solddate", "出栏时间") { BShow = true, Width = 14 });
            MyColumns.Add("solduserid", new MyColumn("solduserid", "出栏操作人") { BShow = true, Width = 8 });
            MyColumns.Add("soldflag", new MyColumn("soldflag", "出栏状态") { BShow = true, Width = 8 });
            MyColumns.Add("sum_num", new MyColumn("sum_num", "总行数") { BShow = false });

            _tableview.MyColumns = MyColumns;
            _tableview.BShowModify = false;
            _tableview.BShowDetails = false;

            if ((Application.Current.Resources["User"] as UserInfo).FlagTier == "0")
            {
                _tableview.BShowDelete = true;
            }
            else
            {
                _tableview.BShowDelete = false;
            }

            _tableview.DeleteRowEnvent += new UcTableOperableView_NoTitle.DeleteRowEventHandler(_tableview_DeleteRowEnvent);
        }

        void _culture_company_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_culture_company.SelectedIndex > 0)
            {
                ComboboxTool.InitComboboxSource(_file_cuserid, string.Format("SELECT RECO_PKID,INFO_USER,NUMB_USER FROM sys_client_user where fk_dept = '{0}'", (_culture_company.SelectedItem as Label).Tag.ToString()), "cxtj");
            }
            else if (_culture_company.SelectedIndex == 0)
            {
                ComboboxTool.InitComboboxSource(_file_cuserid, string.Format("call p_user_detuser_hb('{0}','yz')", userId), "cxtj");
            }
        }

        void _tableview_DeleteRowEnvent(string id)
        {
            if (Toolkit.MessageBox.Show("确定要删除该条养殖档案记录吗？", "系统询问", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    int result = dbOperation.GetDbHelper().ExecuteSql(string.Format("delete from t_culture_file where FileNo ='{0}'", id));
                    if (result > 0)
                    {
                        Toolkit.MessageBox.Show("删除成功！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        GetData();
                    }
                    else
                    {
                        Toolkit.MessageBox.Show("删除失败！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                }
                catch
                {
                    Toolkit.MessageBox.Show("删除失败2！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
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
            DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_query_culture_file({0},'{1}','{2}','{3}','{4}','{5}','{6}',{7},{8})",
                   userId,
                   ((DateTime)dtpStartDate.SelectedDate).ToShortDateString(),
                   ((DateTime)dtpEndDate.SelectedDate).ToShortDateString(),
                   _object_type.SelectedIndex < 1 ? "" : (_object_type.SelectedItem as Label).Tag,
                   _colony_no.SelectedIndex < 1 ? "" : (_colony_no.SelectedItem as Label).Tag,
                   _culture_company.SelectedIndex < 1 ? "" : (_culture_company.SelectedItem as Label).Tag,
                   _file_cuserid.SelectedIndex < 1 ? "" : (_file_cuserid.SelectedItem as Label).Tag,
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
            DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_query_culture_file({0},'{1}','{2}','{3}','{4}','{5}','{6}',{7},{8})",
                   userId,
                   ((DateTime)dtpStartDate.SelectedDate).ToShortDateString(),
                   ((DateTime)dtpEndDate.SelectedDate).ToShortDateString(),
                   _object_type.SelectedIndex < 1 ? "" : (_object_type.SelectedItem as Label).Tag,
                   _colony_no.SelectedIndex < 1 ? "" : (_colony_no.SelectedItem as Label).Tag,
                   _culture_company.SelectedIndex < 1 ? "" : (_culture_company.SelectedItem as Label).Tag,
                   _file_cuserid.SelectedIndex < 1 ? "" : (_file_cuserid.SelectedItem as Label).Tag,
                  0,
                  _tableview.RowTotal)).Tables[0];

            _tableview.ExportExcel(table);
        }
    }
}
