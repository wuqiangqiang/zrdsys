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
using FoodSafetyMonitoring.Common;
using FoodSafetyMonitoring.dao;
using DBUtility;
using System.Data;
using Toolkit = Microsoft.Windows.Controls;
using FoodSafetyMonitoring.Manager.UserControls;
using System.Windows.Threading;

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// UcTaskAllocation.xaml 的交互逻辑
    /// </summary>
    public partial class UcTaskAllocation : UserControl
    {
        DbHelperMySQL dbOperation;
        private string user_flag_tier;
        private string supplierId;
        private string depttype;
        private string dept_id;

        public UcTaskAllocation(string dept_type)
        {
            InitializeComponent();
            dbOperation = DBUtility.DbHelperMySQL.CreateDbHelper();
            this.depttype = dept_type;
            btnSave.Tag = "新增";
            user_flag_tier = (Application.Current.Resources["User"] as UserInfo).FlagTier;
            supplierId = (Application.Current.Resources["User"] as UserInfo).SupplierId;
            dept_id = (Application.Current.Resources["User"] as UserInfo).DepartmentID;

            bool exit_flag = dbOperation.Exists(string.Format("select count(nian) from t_task_assign_hb where nian = '{0}' and did = '{1}'", DateTime.Now.Year,dept_id));

            if (exit_flag)
            {
                _grid_detail.Visibility = Visibility.Hidden;
                btnSave.Visibility = Visibility.Hidden;
                _tabControl.SelectedIndex = 1;
            }


            //switch (user_flag_tier)
            //{
            //    case "0": _dept_name.Text = "选择省:";
            //        dept_name = "省名称";
            //        break;
            //    case "1": _dept_name.Text = "选择市(州):";
            //        dept_name = "市(州)名称";
            //        break;
            //    case "2": _dept_name.Text = "选择区县:";
            //        dept_name = "区县名称";
            //        break;
            //    case "3": _dept_name.Text = "选择检测站点:";
            //        dept_name = "检测站点名称";
            //        break;
            //    case "4": _dept_name.Text = "选择检测站点:";
            //        dept_name = "检测站点名称";
            //        break;
            //    default: break;
            //}
            
            _tableview.ModifyRowEnvent += new UcTableOperableView_NoTitle.ModifyRowEventHandler(_tableview_ModifyRowEnvent);
            _tableview.DeleteRowEnvent += new UcTableOperableView_NoTitle.DeleteRowEventHandler(_tableview_DeleteRowEnvent);

        }

        List<string> tableHeaders = new List<string>() { "检测项目", DateTime.Now.Year + "年"};
        Dictionary<string, TextBox> textBoxs = new Dictionary<string, TextBox>();

        //void _detect_station_SelectionChanged(object sender, SelectionChangedEventArgs e)
        private void createTask()
        {
            //清空控件区域
            lastTextboxValues.Clear();
            _grid_detail.Children.Clear();
            _grid_detail.RowDefinitions.Clear();
            _grid_detail.ColumnDefinitions.Clear();
            textBoxs.Clear();

            //根据列个数，插入列
            for (int i = 0; i < tableHeaders.Count; i++)
            {
                _grid_detail.ColumnDefinitions.Add(new ColumnDefinition());
                _grid_detail.ColumnDefinitions[i].Width = new GridLength(200, GridUnitType.Pixel);
            }

            //画出标题
            _grid_detail.RowDefinitions.Add(new RowDefinition());
            for (int i = 0; i < tableHeaders.Count; i++)
            {
                Border border = new Border();
                border.BorderBrush = new SolidColorBrush(Color.FromRgb(228, 227, 225));
                border.HorizontalAlignment = HorizontalAlignment.Center;
                border.Width = 200;
                border.Background = new SolidColorBrush(Color.FromRgb(242, 247, 251));
                if (i == tableHeaders.Count - 1)
                {
                    border.BorderThickness = new Thickness(1,1,1,0);
                }
                else
                {
                    border.BorderThickness = new Thickness(1,1,0,0);
                }
                TextBox textBox = new TextBox();
                textBox.Margin = new Thickness(0);
                textBox.IsReadOnly = true;
                textBox.Text = tableHeaders[i];
                textBox.HorizontalAlignment = HorizontalAlignment.Center;
                textBox.HorizontalContentAlignment = HorizontalAlignment.Center;
                border.Child = textBox;
                border.SetValue(Grid.RowProperty, 0);
                border.SetValue(Grid.ColumnProperty, i);
                _grid_detail.Children.Add(border);
            }

            //string sql = string.Format("SELECT ItemID,ItemNAME FROM t_det_item WHERE  (tradeId ='{0}'or ifnull(tradeId,'') = '') and OPENFLAG = '1' order by orderId", (_detect_trade.SelectedItem as Label).Tag);
            string sql = "SELECT ItemID,ItemNAME FROM t_det_item_hb WHERE  (tradeId ='1' or ifnull(tradeId,'') = '') and OPENFLAG = '1'";
            
            DataTable table = dbOperation.GetDataSet(sql).Tables[0];
            _grid_detail.Tag = table;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                _grid_detail.RowDefinitions.Add(new RowDefinition());
                for (int j = 0; j < tableHeaders.Count; j++)
                {
                    Border border = new Border();
                    border.BorderBrush = new SolidColorBrush(Color.FromRgb(228, 227, 225));
                    border.HorizontalAlignment = HorizontalAlignment.Center;
                    border.Width = 200;
                    
                    if (j == tableHeaders.Count - 1)
                    {
                        if (i == table.Rows.Count - 1 )
                        {
                            border.BorderThickness = new Thickness(1, 1, 1, 1);
                        }
                        else
                        {
                            border.BorderThickness = new Thickness(1, 1, 1, 0);
                        }
                        
                    }
                    else
                    {
                        if (i == table.Rows.Count - 1)
                        {
                            border.BorderThickness = new Thickness(1, 1, 0, 1);
                        }
                        else
                        {
                            border.BorderThickness = new Thickness(1, 1, 0, 0);
                        }      
                    }
                    TextBox textBox = new TextBox();
                    textBox.Margin = new Thickness(0);
                    if (j == 0)
                    {
                        textBox.IsReadOnly = true;
                        textBox.Text = table.Rows[i][1].ToString();
                        textBox.Tag = table.Rows[i][0].ToString();
                    }
                    else
                    {
                        textBox.Text = "0";
                        textBox.MaxLength = 3;
                        textBox.TextChanged += new TextChangedEventHandler(textBox_TextChanged);
                        textBox.Tag = (i + 1) + "," + j;
                        textBoxs.Add((i + 1) + "," + j, textBox);
                    }
                    textBox.HorizontalAlignment = HorizontalAlignment.Center;
                    textBox.HorizontalContentAlignment = HorizontalAlignment.Center;
                    border.Child = textBox;
                    border.SetValue(Grid.RowProperty, i + 1);
                    border.SetValue(Grid.ColumnProperty, j);
                    _grid_detail.Children.Add(border);
                }
            }

            //_grid_detail.RowDefinitions.Add(new RowDefinition());
            //for (int j = 0; j < tableHeaders.Count; j++)
            //{
            //    Border border = new Border();
            //    border.BorderBrush = new SolidColorBrush(Color.FromRgb(228, 227, 225));
            //    border.HorizontalAlignment = HorizontalAlignment.Center;
            //    border.Width = 200;
            //    if (j == tableHeaders.Count - 1)
            //    {
            //        border.BorderThickness = new Thickness(1, 1, 1, 1);
            //    }
            //    else
            //    {
            //        border.BorderThickness = new Thickness(1, 1, 0, 1);
            //    }
            //    TextBox textBox = new TextBox();
            //    textBox.Margin = new Thickness(0);
            //    textBox.IsReadOnly = true;
            //    if (j == 0)
            //    {
            //        textBox.Text = "合计";
            //    }
            //    else
            //    {
            //        textBox.Text = "0";
            //        textBoxs.Add("列合计" + j, textBox);
            //    }
            //    textBox.Foreground = Brushes.Blue;
            //    textBox.HorizontalAlignment = HorizontalAlignment.Center;
            //    textBox.HorizontalContentAlignment = HorizontalAlignment.Center;
            //    border.Child = textBox;
            //    border.SetValue(Grid.RowProperty, _grid_detail.RowDefinitions.Count - 1);
            //    border.SetValue(Grid.ColumnProperty, j);
            //    _grid_detail.Children.Add(border);
            //}

            //btnSave.Visibility = Visibility.Visible;

        }


        private Dictionary<string, string> lastTextboxValues = new Dictionary<string, string>();


        void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int value = 0;
            try
            {
                if ((sender as TextBox).Text == "")
                {
                    value = 0 ;
                }
                else
                {
                    value = Convert.ToInt32((sender as TextBox).Text);
                }
                
                int lastValue = 0;
                if (lastTextboxValues.ContainsKey((sender as TextBox).Tag.ToString()))
                {
                    if (lastTextboxValues[(sender as TextBox).Tag.ToString()] != "")
                    {
                        lastValue = Convert.ToInt32(lastTextboxValues[(sender as TextBox).Tag.ToString()]);
                    } 
                    else
                    {
                        lastValue = 0;
                    }
                    lastTextboxValues[(sender as TextBox).Tag.ToString()] = (sender as TextBox).Text;
                }
                else
                {
                    if ((sender as TextBox).Text != "")
                    {
                        lastTextboxValues.Add((sender as TextBox).Tag.ToString(), (sender as TextBox).Text);
                    }
                }

                string[] s = (sender as TextBox).Tag.ToString().Split(new char[] { ',' });
                //textBoxs["行合计" + s[0]].Text = (Convert.ToInt32(textBoxs["行合计" + s[0]].Text) - lastValue + value).ToString();
                //textBoxs["列合计" + s[1]].Text = (Convert.ToInt32(textBoxs["列合计" + s[1]].Text) - lastValue + value).ToString();
                //textBoxs["列合计" + (tableHeaders.Count - 1)].Text = (Convert.ToInt32(textBoxs["列合计" + (tableHeaders.Count - 1)].Text) - lastValue + value).ToString();

            }
            catch
            {
                Toolkit.MessageBox.Show("输入的必须是数字！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                if (lastTextboxValues.ContainsKey((sender as TextBox).Tag.ToString()))
                {
                    (sender as TextBox).Text = lastTextboxValues[(sender as TextBox).Tag.ToString()];
                }
                else
                {
                    (sender as TextBox).Text = "0";
                }

                return;
            }

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //if (btnSave.Tag.ToString() != "修改")
            //{
            //    if (_detect_station.SelectedIndex == 0)
            //    {
            //        Toolkit.MessageBox.Show("请选择"+ dept_name +"！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
            //        return;
            //    }
            //}

            StringBuilder sb = new StringBuilder();
            DataTable table = _grid_detail.Tag as DataTable;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                sb.Append(table.Rows[i][0].ToString() + ",");
                for (int j = 1; j < tableHeaders.Count; j++)
                {
                    sb.Append(textBoxs[(i + 1) + "," + j].Text);
                    if (j != tableHeaders.Count - 1)
                    {
                        sb.Append(",");
                    }
                }
                if (i != table.Rows.Count - 1)
                {
                    sb.Append("#");
                }
            }
            string tipInfo_success = "";
            string tipInfo_fail = "";
            try
            {

                string function = "";

                if (btnSave.Tag.ToString() == "新增")
                {
                    function = "p_insert_task_hb";
                    tipInfo_success = "添加成功!";
                    tipInfo_fail = "添加失败!";
                }
                if (btnSave.Tag.ToString() == "修改")
                {
                    function = "p_update_task_hb";
                    btnSave.Tag = "新增";
                    tipInfo_success = "修改成功!";
                    tipInfo_fail = "修改失败!";
                }
                if (function == "")
                {
                    return;
                }
                int result = dbOperation.ExecuteSql(string.Format("call " + function + " ('{0}','{1}','{2}','{3}')",
                                                    dept_id, DateTime.Now.Year, sb.ToString(),
                                                     depttype));

                if (result == 1)
                {
                    Toolkit.MessageBox.Show(tipInfo_success, "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    _grid_detail.Children.Clear();
                    _grid_detail.RowDefinitions.Clear();
                    _grid_detail.ColumnDefinitions.Clear();
                    textBoxs.Clear();
                    if (btnSave.Tag.ToString() == "修改")
                    {
                        btnSave.Tag = "新增";
                    }
                    _grid_detail.Visibility = Visibility.Hidden;
                    btnSave.Visibility = Visibility.Hidden;
                    _tabControl.SelectedIndex = 1;
                    loadTableView();
                }
                else
                {
                    Toolkit.MessageBox.Show(tipInfo_fail, "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch
            {
                Toolkit.MessageBox.Show(tipInfo_fail, "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            clear();

        }


        private void clear()
        {
            foreach (TextBox textBox in textBoxs.Values)
            {
                textBox.Text = "0";
            }
        }



        class DetectTask
        {
            //public string Deptment_id { get; set; }
            //public string Department_name { get; set; }
            //public Dictionary<string, int> Detect_Items = new Dictionary<string, int>();

            //public DetectTask(string Deptment_id, string Department_name, string Detect_item, int Detect_count)
            //{
            //    this.Deptment_id = Deptment_id;
            //    this.Department_name = Department_name;
            //    Detect_Items = new Dictionary<string, int>() { { Detect_item, Detect_count } };
            //}

            public string Nian { get; set; }
            public Dictionary<string, string> Detect_Items = new Dictionary<string, string>();

            public DetectTask(string nian, string Detect_item, string Detect_count)
            {
                this.Nian = nian;
                Detect_Items = new Dictionary<string,string>() { { Detect_item, Detect_count } };
            }
        }



        private void _tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if ((_tabControl.SelectedItem as TabItem).Header.ToString() == "抽检率统计表" && _tabControl.Tag.ToString() != "抽检率统计表")
            {
                _tabControl.Tag = "抽检率统计表";
                loadTableView();
            }
            else if ((_tabControl.SelectedItem as TabItem).Header.ToString() == "抽检率设置" && _tabControl.Tag.ToString() != "抽检率设置" && btnSave.Tag.ToString() != "修改")
            {
                _tabControl.Tag = "抽检率设置";

                createTask();
                //_detect_trade.SelectionChanged -= new SelectionChangedEventHandler(_detect_trade_SelectionChanged);
                //if (supplierId == "nkrx")
                //{
                //    ComboboxTool.InitComboboxSource(_detect_trade, "select tradeId,tradeName from t_trade where openFlag = '1' order by orderId", "lr");
                //}
                //else
                //{
                //    ComboboxTool.InitComboboxSource(_detect_trade, "select tradeId,tradeName from t_trade where openFlag = '1'", "lr");
                //}
                //_detect_trade.SelectedIndex = 1;
                //_detect_trade.SelectionChanged += new SelectionChangedEventHandler(_detect_trade_SelectionChanged);

                //_detect_station.SelectionChanged -= new SelectionChangedEventHandler(_detect_station_SelectionChanged);
                //ComboboxTool.InitComboboxSource(_detect_station, string.Format("call p_user_dept_task_hb('{0}','{1}','{2}')", (Application.Current.Resources["User"] as UserInfo).ID, DateTime.Now.Year, depttype), "lr");
                //_detect_station.SelectionChanged += new SelectionChangedEventHandler(_detect_station_SelectionChanged);
            }
        }

        private void loadTableView()
        {
            DataTable table = dbOperation.GetDataSet(string.Format("call p_query_task_hb({0},'{1}','{2}')", (Application.Current.Resources["User"] as UserInfo).ID, DateTime.Now.Year, depttype)).Tables[0];
            Dictionary<string, DetectTask> DetectTasks = new Dictionary<string, DetectTask>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                //string deptment_id = table.Rows[i]["dept_no"].ToString();
                //string deptment_name = table.Rows[i]["dept_name"].ToString();
                string nian = table.Rows[i]["nian"].ToString();
                string detect_item = table.Rows[i]["item_name"].ToString();
                string detect_count = table.Rows[i]["task_sum"].ToString();

                if (DetectTasks.ContainsKey(nian))
                {
                    if (!DetectTasks[nian].Detect_Items.ContainsKey(detect_item))
                    {
                        DetectTasks[nian].Detect_Items.Add(detect_item, detect_count);
                    }
                }
                else
                {
                    DetectTasks.Add(nian, new DetectTask(nian, detect_item, detect_count));
                }
            }

            DataTable show_table = new DataTable();
            //show_table.Columns.Add("ID", Type.GetType("System.String"));
            show_table.Columns.Add("年", Type.GetType("System.String"));
            //show_table.Columns.Add("合计", Type.GetType("System.String"));

            Dictionary<string, MyColumn> MyColumns = new Dictionary<string, MyColumn>();
            //MyColumns.Add("id", new MyColumn("id", "id") { BShow = false });
            MyColumns.Add("年", new MyColumn("年", "年份") { BShow = true, Width = 10 });
            //MyColumns.Add("合计", new MyColumn("合计", "合计") { BShow = true, Width = 8 });
            _tableview.BShowModify = true;
            _tableview.BShowDelete = true;

            foreach (DetectTask detectTask in DetectTasks.Values)
            { 
                foreach (string item_name in detectTask.Detect_Items.Keys)
                {
                    if (!show_table.Columns.Contains(item_name))
                    {
                        show_table.Columns.Add(item_name, Type.GetType("System.String"));
                        show_table.Columns[item_name].DefaultValue = "0";
                    } 
                } 
            }


            foreach (DetectTask detectTask in DetectTasks.Values)
            {
                DataRow row = show_table.NewRow();
                //row["ID"] = detectTask.Deptment_id;
                row["年"] = detectTask.Nian;
                //int sum = 0;
                string item_id;
                foreach (string item_name in detectTask.Detect_Items.Keys)
                {  
                    row[item_name] = detectTask.Detect_Items[item_name];
                    //sum += detectTask.Detect_Items[item_name];
                    if (item_name == "黄曲霉M1")
                    {
                        item_id = "黄曲霉m1";
                    }
                    else
                    {
                        item_id = item_name;
                    }
                    if (!MyColumns.ContainsKey(item_id))
                    {
                        MyColumns.Add(item_id, new MyColumn(item_id, item_name) { BShow = true, Width = 12 });
                    }
                }
                //row["合计"] = sum;
                show_table.Rows.Add(row);
            }
            //show_table.Columns["合计"].SetOrdinal(show_table.Columns.Count - 1);
            _tableview.MyColumns = MyColumns;
            _tableview.Table = show_table;
            //计算报表总条数
            int row_count = 0 ;
            if (show_table.Rows.Count != 0)
            {
                row_count =  show_table.Rows.Count ;
            }
            else
            {
                row_count = 0 ;
            }
            _title.Text = "  合计" + row_count + "条数据";
        }

        void _tableview_DeleteRowEnvent(string id)
        {
            if (Toolkit.MessageBox.Show("确定要删除该条数据吗？", "系统询问", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    int result = dbOperation.ExecuteSql(string.Format("delete from t_task_assign_hb where did ='{0}' and nian ='{1}' ", dept_id, id));
                    if (result > 0)
                    {
                        Toolkit.MessageBox.Show("删除成功！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        loadTableView();
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


        void _tableview_ModifyRowEnvent(string id)
        {
            btnSave.Tag = "修改";
            btnSave.Visibility = Visibility.Visible;
            _grid_detail.Visibility = Visibility.Visible;
            //_detect_trade_name.Visibility = Visibility.Hidden;
            //_detect_trade.Visibility = Visibility.Hidden;
            //_dept_name.Text = dept_name;
            //_detect_station.SelectionChanged -= new SelectionChangedEventHandler(_detect_station_SelectionChanged);
            _tabControl.SelectedIndex = 0;
            //_detect_station.IsEnabled = false;
            string sql = string.Format("select iid,ItemNAME,task" +
                                        " from t_task_assign_hb ,t_det_item_hb" +
                                        " where t_task_assign_hb.iid = t_det_item_hb.ItemID" +
                                        " and  did = '{0}' and nian='{1}' and depttype = '{2}'", dept_id, id, depttype);
            //string deptment_name = "";
            //for (int i = 0; i < _tableview.Table.Rows.Count; i++)
            //{
            //    if (_tableview.Table.Rows[i][0].ToString() == id)
            //    {
            //        deptment_name = _tableview.Table.Rows[i][1].ToString();
            //    }
            //}

            //_detect_station.Items.Clear();
            //Label label = new Label();
            //label.Content = deptment_name;
            //label.Tag = id;
            //_detect_station.Items.Add(label);
            //_detect_station.SelectedIndex = 0;


            _grid_detail.Children.Clear();
            _grid_detail.RowDefinitions.Clear();
            _grid_detail.ColumnDefinitions.Clear();
            textBoxs.Clear();

            for (int i = 0; i < tableHeaders.Count; i++)
            {

                _grid_detail.ColumnDefinitions.Add(new ColumnDefinition());
                _grid_detail.ColumnDefinitions[i].Width = new GridLength(200, GridUnitType.Pixel);
            }
            _grid_detail.RowDefinitions.Add(new RowDefinition());
            for (int i = 0; i < tableHeaders.Count; i++)
            {
                Border border = new Border();
                border.BorderBrush = new SolidColorBrush(Color.FromRgb(228, 227, 225));
                border.HorizontalAlignment = HorizontalAlignment.Center;
                border.Width = 200;
                border.Background = new SolidColorBrush(Color.FromRgb(242, 247, 251));
                if (i == tableHeaders.Count - 1)
                {
                    border.BorderThickness = new Thickness(1, 1, 1, 0);
                }
                else
                {
                    border.BorderThickness = new Thickness(1, 1, 0, 0);
                }
                TextBox textBox = new TextBox();
                textBox.Margin = new Thickness(0);
                textBox.IsReadOnly = true;
                
                if (i == tableHeaders.Count - 1)
                {
                    textBox.Text = id + "年";
                }
                else
                {
                    textBox.Text = tableHeaders[i];
                }
                textBox.HorizontalAlignment = HorizontalAlignment.Center;
                textBox.HorizontalContentAlignment = HorizontalAlignment.Center;
                border.Child = textBox;
                border.SetValue(Grid.RowProperty, 0);
                border.SetValue(Grid.ColumnProperty, i);
                _grid_detail.Children.Add(border);
            }

            DataTable table = dbOperation.GetDataSet(sql).Tables[0];
            _grid_detail.Tag = table;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                _grid_detail.RowDefinitions.Add(new RowDefinition());
                for (int j = 0; j < tableHeaders.Count; j++)
                {
                    Border border = new Border();
                    border.BorderBrush = new SolidColorBrush(Color.FromRgb(228, 227, 225));
                    border.HorizontalAlignment = HorizontalAlignment.Center;
                    border.Width = 200;
                    if (j == tableHeaders.Count - 1)
                    {
                        if (i == table.Rows.Count - 1)
                        {
                            border.BorderThickness = new Thickness(1, 1, 1, 1);
                        }
                        else
                        {
                            border.BorderThickness = new Thickness(1, 1, 1, 0);
                        }

                    }
                    else
                    {
                        if (i == table.Rows.Count - 1)
                        {
                            border.BorderThickness = new Thickness(1, 1, 0, 1);
                        }
                        else
                        {
                            border.BorderThickness = new Thickness(1, 1, 0, 0);
                        }
                    }
                    TextBox textBox = new TextBox();
                    textBox.Margin = new Thickness(0);
                    if (j == 0)
                    {
                        textBox.IsReadOnly = true;
                        textBox.Text = table.Rows[i][1].ToString();
                        textBox.Tag = table.Rows[i][0].ToString();
                    }
                    else
                    {
                        textBox.Text = "0";
                        textBox.MaxLength = 3;
                        textBox.TextChanged += new TextChangedEventHandler(textBox_TextChanged);
                        textBox.Tag = (i + 1) + "," + j;
                        textBoxs.Add((i + 1) + "," + j, textBox);
                    }
                    textBox.HorizontalAlignment = HorizontalAlignment.Center;
                    textBox.HorizontalContentAlignment = HorizontalAlignment.Center;
                    border.Child = textBox;
                    border.SetValue(Grid.RowProperty, i + 1);
                    border.SetValue(Grid.ColumnProperty, j);
                    _grid_detail.Children.Add(border);
                }
            }

            //_grid_detail.RowDefinitions.Add(new RowDefinition());
            //for (int j = 0; j < tableHeaders.Count; j++)
            //{
            //    Border border = new Border();
            //    border.BorderBrush = new SolidColorBrush(Color.FromRgb(228, 227, 225));
            //    border.HorizontalAlignment = HorizontalAlignment.Center;
            //    border.Width = 200;
            //    if (j == tableHeaders.Count - 1)
            //    {
            //        border.BorderThickness = new Thickness(1, 1, 1, 1);
            //    }
            //    else
            //    {
            //        border.BorderThickness = new Thickness(1, 1, 0, 1);
            //    }
            //    TextBox textBox = new TextBox();
            //    textBox.Margin = new Thickness(0);
            //    textBox.IsReadOnly = true;
            //    if (j == 0)
            //    {
            //        textBox.Text = "合计";
            //    }
            //    else
            //    {
            //        textBox.Text = "0";
            //        textBoxs.Add("列合计" + j, textBox);
            //    }
            //    textBox.Foreground = Brushes.Black;
            //    textBox.HorizontalAlignment = HorizontalAlignment.Center;
            //    textBox.HorizontalContentAlignment = HorizontalAlignment.Center;
            //    border.Child = textBox;
            //    border.SetValue(Grid.RowProperty, _grid_detail.RowDefinitions.Count - 1);
            //    border.SetValue(Grid.ColumnProperty, j);
            //    _grid_detail.Children.Add(border);
            //}
            lastTextboxValues.Clear();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                for (int j = 2; j < table.Columns.Count; j++)
                {
                    if (textBoxs.ContainsKey((i + 1) + "," + (j - 1)))
                    {
                        textBoxs[(i + 1) + "," + (j - 1)].Text = table.Rows[i][j].ToString().Length == 0 ? "0" : table.Rows[i][j].ToString();
                    }
                }
            }    
        }
    }
}
