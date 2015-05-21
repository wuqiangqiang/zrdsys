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
        private string dept_name;
        private string supplierId;

        public UcTaskAllocation()
        {
            InitializeComponent();
            dbOperation = DBUtility.DbHelperMySQL.CreateDbHelper();
            btnSave.Tag = "新增";
            user_flag_tier = (Application.Current.Resources["User"] as UserInfo).FlagTier;
            supplierId = (Application.Current.Resources["User"] as UserInfo).SupplierId;

            switch (user_flag_tier)
            {
                case "0": _dept_name.Text = "选择省:";
                    dept_name = "省名称";
                    break;
                case "1": _dept_name.Text = "选择地市:";
                    dept_name = "地市名称";
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
            
            _tableview.ModifyRowEnvent += new UcTableOperableView.ModifyRowEventHandler(_tableview_ModifyRowEnvent);
            _tableview.DeleteRowEnvent += new UcTableOperableView.DeleteRowEventHandler(_tableview_DeleteRowEnvent);

        }

        List<string> tableHeaders = new List<string>() { "检测项目", "1月", "2月", "3月", "4月", "5月", "6月", "7月", "8月", "9月", "10月", "11月", "12月", "合计" };
        Dictionary<string, TextBox> textBoxs = new Dictionary<string, TextBox>();

        void _detect_trade_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _detect_station.SelectedIndex = 0;
            _grid_detail.Children.Clear();
            btnSave.Visibility = Visibility.Hidden;
        }

        void _detect_station_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lastTextboxValues.Clear();
            _grid_detail.Children.Clear();
            _grid_detail.RowDefinitions.Clear();
            _grid_detail.ColumnDefinitions.Clear();
            textBoxs.Clear();

            for (int i = 0; i < tableHeaders.Count; i++)
            {

                _grid_detail.ColumnDefinitions.Add(new ColumnDefinition());
                if (i == 0)
                {
                    _grid_detail.ColumnDefinitions[0].Width = new GridLength(2, GridUnitType.Star);
                }
                else
                {
                    _grid_detail.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
                }
            }
            _grid_detail.RowDefinitions.Add(new RowDefinition());
            for (int i = 0; i < tableHeaders.Count; i++)
            {
                Border border = new Border();
                border.BorderBrush = Brushes.Black;
                border.HorizontalAlignment = HorizontalAlignment.Center;
                border.BorderThickness = new Thickness(0.2);
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

            string sql = string.Format("SELECT ItemID,ItemNAME FROM t_det_item WHERE  (tradeId ='{0}'or ifnull(tradeId,'') = '') and OPENFLAG = '1' order by orderId", (_detect_trade.SelectedItem as Label).Tag);
            DataTable table = dbOperation.GetDataSet(sql).Tables[0];
            _grid_detail.Tag = table;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                _grid_detail.RowDefinitions.Add(new RowDefinition());
                for (int j = 0; j < tableHeaders.Count; j++)
                {
                    Border border = new Border();
                    border.BorderBrush = Brushes.Black;
                    border.HorizontalAlignment = HorizontalAlignment.Center;
                    border.BorderThickness = new Thickness(0.2);
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
                        textBox.MaxLength = 5;
                        if (j == tableHeaders.Count - 1)
                        {
                            textBox.IsReadOnly = true;
                            textBoxs.Add("行合计" + (_grid_detail.RowDefinitions.Count - 1), textBox);
                        }
                        else
                        {
                            textBox.TextChanged += new TextChangedEventHandler(textBox_TextChanged);
                            textBox.Tag = (i + 1) + "," + j;
                            textBoxs.Add((i + 1) + "," + j, textBox);
                        }
                    }
                    textBox.HorizontalAlignment = HorizontalAlignment.Center;
                    textBox.HorizontalContentAlignment = HorizontalAlignment.Center;
                    border.Child = textBox;
                    border.SetValue(Grid.RowProperty, i + 1);
                    border.SetValue(Grid.ColumnProperty, j);
                    _grid_detail.Children.Add(border);
                }
            }

            _grid_detail.RowDefinitions.Add(new RowDefinition());
            for (int j = 0; j < tableHeaders.Count; j++)
            {
                Border border = new Border();
                border.BorderBrush = Brushes.Black;
                border.HorizontalAlignment = HorizontalAlignment.Center;
                border.BorderThickness = new Thickness(0.2);
                TextBox textBox = new TextBox();
                textBox.Margin = new Thickness(0);
                textBox.IsReadOnly = true;
                if (j == 0)
                {
                    textBox.Text = "合计";
                }
                else
                {
                    textBox.Text = "0";
                    textBoxs.Add("列合计" + j, textBox);
                }
                textBox.Foreground = Brushes.Blue;
                textBox.HorizontalAlignment = HorizontalAlignment.Center;
                textBox.HorizontalContentAlignment = HorizontalAlignment.Center;
                border.Child = textBox;
                border.SetValue(Grid.RowProperty, _grid_detail.RowDefinitions.Count - 1);
                border.SetValue(Grid.ColumnProperty, j);
                _grid_detail.Children.Add(border);
            }

            btnSave.Visibility = Visibility.Visible;

        }


        private Dictionary<string, string> lastTextboxValues = new Dictionary<string, string>();


        void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int value = 0;
            try
            {
                value = Convert.ToInt32((sender as TextBox).Text);
                int lastValue = 0;
                if (lastTextboxValues.ContainsKey((sender as TextBox).Tag.ToString()))
                {
                    lastValue = Convert.ToInt32(lastTextboxValues[(sender as TextBox).Tag.ToString()]);
                    lastTextboxValues[(sender as TextBox).Tag.ToString()] = (sender as TextBox).Text;
                }
                else
                {
                    lastTextboxValues.Add((sender as TextBox).Tag.ToString(), (sender as TextBox).Text);
                }

                string[] s = (sender as TextBox).Tag.ToString().Split(new char[] { ',' });
                textBoxs["行合计" + s[0]].Text = (Convert.ToInt32(textBoxs["行合计" + s[0]].Text) - lastValue + value).ToString();
                textBoxs["列合计" + s[1]].Text = (Convert.ToInt32(textBoxs["列合计" + s[1]].Text) - lastValue + value).ToString();
                textBoxs["列合计" + (tableHeaders.Count - 1)].Text = (Convert.ToInt32(textBoxs["列合计" + (tableHeaders.Count - 1)].Text) - lastValue + value).ToString();

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
            if (btnSave.Tag.ToString() != "修改")
            {
                if (_detect_station.SelectedIndex == 0)
                {
                    Toolkit.MessageBox.Show("请选择"+ dept_name +"！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
            }
            StringBuilder sb = new StringBuilder();
            DataTable table = _grid_detail.Tag as DataTable;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                sb.Append(table.Rows[i][0].ToString() + ",");
                for (int j = 1; j < tableHeaders.Count - 1; j++)
                {
                    sb.Append(textBoxs[(i + 1) + "," + j].Text);
                    if (j != tableHeaders.Count - 2)
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
                    function = "p_insert_task";
                    tipInfo_success = "添加成功!";
                    tipInfo_fail = "添加失败!";
                }
                if (btnSave.Tag.ToString() == "修改")
                {
                    function = "p_update_task";
                    btnSave.Tag = "新增";
                    tipInfo_success = "修改成功!";
                    tipInfo_fail = "修改失败!";
                }
                if (function == "")
                {
                    return;
                }
                int result = dbOperation.ExecuteSql(string.Format("call " + function + " ('{0}','{1}','{2}')", (_detect_station.SelectedItem as Label).Tag, DateTime.Now.Year, sb.ToString()));

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
            public string Deptment_id { get; set; }
            public string Department_name { get; set; }
            public Dictionary<string, int> Detect_Items = new Dictionary<string, int>();

            public DetectTask(string Deptment_id, string Department_name, string Detect_item, int Detect_count)
            {
                this.Deptment_id = Deptment_id;
                this.Department_name = Department_name;
                Detect_Items = new Dictionary<string, int>() { { Detect_item, Detect_count } };
            }
        }



        private void _tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if ((_tabControl.SelectedItem as TabItem).Header.ToString() == "任务汇总表" && _tabControl.Tag.ToString() != "任务汇总表")
            {
                _tabControl.Tag = "任务汇总表";
                loadTableView();
            }
            else if ((_tabControl.SelectedItem as TabItem).Header.ToString() == "分配检测任务" && _tabControl.Tag.ToString() != "分配检测任务" && btnSave.Tag.ToString() != "修改")
            {
                _tabControl.Tag = "分配检测任务";
                _detect_trade.SelectionChanged -= new SelectionChangedEventHandler(_detect_trade_SelectionChanged);
                if (supplierId == "nkrx")
                {
                    ComboboxTool.InitComboboxSource(_detect_trade, "select tradeId,tradeName from t_trade where openFlag = '1' order by orderId", "lr");
                }
                else
                {
                    ComboboxTool.InitComboboxSource(_detect_trade, "select tradeId,tradeName from t_trade where openFlag = '1'", "lr");
                }
                _detect_trade.SelectedIndex = 1;
                _detect_trade.SelectionChanged += new SelectionChangedEventHandler(_detect_trade_SelectionChanged);

                _detect_station.SelectionChanged -= new SelectionChangedEventHandler(_detect_station_SelectionChanged);
                ComboboxTool.InitComboboxSource(_detect_station, string.Format("call p_user_dept_task('{0}','{1}')", (Application.Current.Resources["User"] as UserInfo).ID, DateTime.Now.Year), "lr");
                _detect_station.SelectionChanged += new SelectionChangedEventHandler(_detect_station_SelectionChanged);
            }
        }

        private void loadTableView()
        {
            DataTable table = dbOperation.GetDataSet(string.Format("call p_query_task({0},{1})", (Application.Current.Resources["User"] as UserInfo).ID, DateTime.Now.Year)).Tables[0];
            Dictionary<string, DetectTask> DetectTasks = new Dictionary<string, DetectTask>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                string deptment_id = table.Rows[i]["dept_id"].ToString();
                string deptment_name = table.Rows[i]["dept_name"].ToString();
                string detect_item = table.Rows[i]["item_name"].ToString();
                int detect_count = Convert.ToInt32(table.Rows[i]["task_sum"].ToString());

                if (DetectTasks.ContainsKey(deptment_id))
                {
                    if (!DetectTasks[deptment_id].Detect_Items.ContainsKey(detect_item))
                    {
                        DetectTasks[deptment_id].Detect_Items.Add(detect_item, detect_count);
                    }
                }
                else
                {
                    DetectTasks.Add(deptment_id, new DetectTask(deptment_id, deptment_name, detect_item, detect_count));
                }
            }

            DataTable show_table = new DataTable();
            show_table.Columns.Add("ID", Type.GetType("System.String"));
            show_table.Columns.Add("区县名称", Type.GetType("System.String"));
            show_table.Columns.Add("合计", Type.GetType("System.String"));

            Dictionary<string, MyColumn> MyColumns = new Dictionary<string, MyColumn>();
            MyColumns.Add("id", new MyColumn("id", "id") { BShow = false });
            MyColumns.Add("区县名称", new MyColumn("区县名称", dept_name +"名称") { BShow = true, Width = 18 });
            MyColumns.Add("合计", new MyColumn("合计", "合计") { BShow = true, Width = 8 });
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
                row["ID"] = detectTask.Deptment_id;
                row["区县名称"] = detectTask.Department_name;
                int sum = 0;
                string item_id;
                foreach (string item_name in detectTask.Detect_Items.Keys)
                {  
                    row[item_name] = detectTask.Detect_Items[item_name];
                    sum += detectTask.Detect_Items[item_name];
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
                row["合计"] = sum;
                show_table.Rows.Add(row);
            }
            show_table.Columns["合计"].SetOrdinal(show_table.Columns.Count - 1);
            _tableview.MyColumns = MyColumns;
            _tableview.Table = show_table;
            _tableview.Title = DateTime.Now.Year + "年各" + dept_name + "检测任务分配汇总表";
        }

        void _tableview_DeleteRowEnvent(string id)
        {
            if (Toolkit.MessageBox.Show("确定要删除该条数据吗？", "系统询问", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    int result = dbOperation.ExecuteSql(string.Format("delete from t_task_assign where did ='{0}' and nian ='{1}' ", id, DateTime.Now.Year));
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
            _detect_trade_name.Visibility = Visibility.Hidden;
            _detect_trade.Visibility = Visibility.Hidden;
            _dept_name.Text = dept_name;
            _detect_station.SelectionChanged -= new SelectionChangedEventHandler(_detect_station_SelectionChanged);
            _tabControl.SelectedIndex = 0;
            _detect_station.IsEnabled = false;
            string sql = string.Format("select iid,ItemNAME,task_jan,task_feb,task_mar,task_apr,task_may,task_jun,task_jul,task_aug,task_sep,task_oct,task_nov,task_dec" +
                                        " from t_task_assign ,t_det_item" +
                                        " where t_task_assign.iid = t_det_item.ItemID" +
                                        " and  did = '{0}' and nian='{1}'", id, DateTime.Now.Year);
            string deptment_name = "";
            for (int i = 0; i < _tableview.Table.Rows.Count; i++)
            {
                if (_tableview.Table.Rows[i][0].ToString() == id)
                {
                    deptment_name = _tableview.Table.Rows[i][1].ToString();
                }
            }

            _detect_station.Items.Clear();
            Label label = new Label();
            label.Content = deptment_name;
            label.Tag = id;
            _detect_station.Items.Add(label);
            _detect_station.SelectedIndex = 0;


            _grid_detail.Children.Clear();
            _grid_detail.RowDefinitions.Clear();
            _grid_detail.ColumnDefinitions.Clear();
            textBoxs.Clear();

            for (int i = 0; i < tableHeaders.Count; i++)
            {

                _grid_detail.ColumnDefinitions.Add(new ColumnDefinition());
                if (i == 0)
                {
                    _grid_detail.ColumnDefinitions[0].Width = new GridLength(2, GridUnitType.Star);
                }
                else
                {
                    _grid_detail.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
                }
            }
            _grid_detail.RowDefinitions.Add(new RowDefinition());
            for (int i = 0; i < tableHeaders.Count; i++)
            {
                Border border = new Border();
                border.BorderBrush = Brushes.Black;
                border.HorizontalAlignment = HorizontalAlignment.Center;
                border.BorderThickness = new Thickness(0.2);
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

            DataTable table = dbOperation.GetDataSet(sql).Tables[0];
            _grid_detail.Tag = table;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                _grid_detail.RowDefinitions.Add(new RowDefinition());
                for (int j = 0; j < tableHeaders.Count; j++)
                {
                    Border border = new Border();
                    border.BorderBrush = Brushes.Black;
                    border.HorizontalAlignment = HorizontalAlignment.Center;
                    border.BorderThickness = new Thickness(0.2);
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
                        textBox.MaxLength = 5;
                        if (j == tableHeaders.Count - 1)
                        {
                            textBox.IsReadOnly = true;
                            textBoxs.Add("行合计" + (_grid_detail.RowDefinitions.Count - 1), textBox);
                        }
                        else
                        {
                            textBox.TextChanged += new TextChangedEventHandler(textBox_TextChanged);
                            textBox.Tag = (i + 1) + "," + j;
                            textBoxs.Add((i + 1) + "," + j, textBox);
                        }
                    }
                    textBox.HorizontalAlignment = HorizontalAlignment.Center;
                    textBox.HorizontalContentAlignment = HorizontalAlignment.Center;
                    border.Child = textBox;
                    border.SetValue(Grid.RowProperty, i + 1);
                    border.SetValue(Grid.ColumnProperty, j);
                    _grid_detail.Children.Add(border);
                }
            }

            _grid_detail.RowDefinitions.Add(new RowDefinition());
            for (int j = 0; j < tableHeaders.Count; j++)
            {
                Border border = new Border();
                border.BorderBrush = Brushes.Black;
                border.HorizontalAlignment = HorizontalAlignment.Center;
                border.BorderThickness = new Thickness(0.2);
                TextBox textBox = new TextBox();
                textBox.Margin = new Thickness(0);
                textBox.IsReadOnly = true;
                if (j == 0)
                {
                    textBox.Text = "合计";
                }
                else
                {
                    textBox.Text = "0";
                    textBoxs.Add("列合计" + j, textBox);
                }
                textBox.Foreground = Brushes.Black;
                textBox.HorizontalAlignment = HorizontalAlignment.Center;
                textBox.HorizontalContentAlignment = HorizontalAlignment.Center;
                border.Child = textBox;
                border.SetValue(Grid.RowProperty, _grid_detail.RowDefinitions.Count - 1);
                border.SetValue(Grid.ColumnProperty, j);
                _grid_detail.Children.Add(border);
            }
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
