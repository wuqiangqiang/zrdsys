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
    /// SetSampling.xaml 的交互逻辑
    /// </summary>
    public partial class SetSampling : Window
    {
        private IDBOperation dbOperation;
        private string deptid;
        private string deptname;
        private string detecttype;
        List<string> tableHeaders ;
        private UcSetSamplingRate setSampling;
        public SetSampling(IDBOperation dbOperation, string dept_id,string dept_name, string detect_type,UcSetSamplingRate setsampling)
        {
            InitializeComponent();

            this.dbOperation = dbOperation;
            this.deptid = dept_id;
            this.detecttype = detect_type;
            this.deptname = dept_name;
            this.setSampling = setsampling;
            createTask();

        }


        Dictionary<string, TextBox> textBoxs = new Dictionary<string, TextBox>();

        private void createTask()
        {
            tableHeaders = new List<string>() { "检测项目", deptname };
            //清空控件区域
            _grid_detail.Children.Clear();
            _grid_detail.RowDefinitions.Clear();
            _grid_detail.ColumnDefinitions.Clear();
            textBoxs.Clear();
 
            //根据列个数，插入列
            for (int i = 0; i < tableHeaders.Count; i++)
            {
                _grid_detail.ColumnDefinitions.Add(new ColumnDefinition());
                _grid_detail.ColumnDefinitions[i].Width = new GridLength(150, GridUnitType.Pixel);
            }

            //画出标题
            _grid_detail.RowDefinitions.Add(new RowDefinition());
            for (int i = 0; i < tableHeaders.Count; i++)
            {
                Border border = new Border();
                border.BorderBrush = new SolidColorBrush(Color.FromRgb(228, 227, 225));
                border.HorizontalAlignment = HorizontalAlignment.Center;
                border.Width = 150;
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
                textBox.Text = tableHeaders[i];
                textBox.HorizontalAlignment = HorizontalAlignment.Center;
                textBox.HorizontalContentAlignment = HorizontalAlignment.Center;
                border.Child = textBox;
                border.SetValue(Grid.RowProperty, 0);
                border.SetValue(Grid.ColumnProperty, i);
                _grid_detail.Children.Add(border);
            }

            string sql = "SELECT ItemID,ItemNAME,t.task FROM t_det_item_hb i" +
                         " left join t_task_assign_hb  t on i.ItemID = t.iid " +
                         "and t.did = '" + deptid + "' " +
                         "and t.detecttype = '" + detecttype + "'" +
                         "WHERE  (tradeId ='1' or ifnull(tradeId,'') = '') and OPENFLAG = '1' ";

            DataTable table = dbOperation.GetDbHelper().GetDataSet(sql).Tables[0];
            _grid_detail.Tag = table;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                _grid_detail.RowDefinitions.Add(new RowDefinition());
                for (int j = 0; j < tableHeaders.Count; j++)
                {
                    Border border = new Border();
                    border.BorderBrush = new SolidColorBrush(Color.FromRgb(228, 227, 225));
                    border.HorizontalAlignment = HorizontalAlignment.Center;
                    border.Width = 150;

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
                        if (table.Rows[i][2].ToString() == null || table.Rows[i][2].ToString() == "")
                        {
                            textBox.Text = "0";
                        }
                        else
                        {
                            textBox.Text = table.Rows[i][2].ToString();
                        }
                        //InputMethod.IsInputMethodEnabled="False" DataObject.Pasting="Object_Count_Pasting"
                        
                        textBox.PreviewKeyDown += new KeyEventHandler(Object_Count_PreviewKeyDown);
                        textBox.PreviewTextInput += new TextCompositionEventHandler(Object_Count_PreviewTextInput);
                        textBox.MaxLength = 3;
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
        }

        private void Object_Count_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!isNumberic(text))
                { e.CancelCommand(); }
            }
            else { e.CancelCommand(); }
        }

        private void Object_Count_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void Object_Count_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!isNumberic(e.Text))
            {
                e.Handled = true;
            }
            else
                e.Handled = false;
        }

        //isDigit是否是数字
        public static bool isNumberic(string _string)
        {
            if (string.IsNullOrEmpty(_string))

                return false;
            foreach (char c in _string)
            {
                if (!char.IsDigit(c))
                    //if(c<'0' c="">'9')//最好的方法,在下面测试数据中再加一个0，然后这种方法效率会搞10毫秒左右
                    return false;
            }
            return true;
        }

        private void exit_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.Close();
                setSampling.Load_table();
                
            }
        }

        private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            this.Left += e.HorizontalChange;
            this.Top += e.VerticalChange;
        }

        private void exit_MouseEnter(object sender, MouseEventArgs e)
        {
            exit.Source = new BitmapImage(new Uri("pack://application:,," + "/res/close_on.png"));
        }

        private void exit_MouseLeave(object sender, MouseEventArgs e)
        {
            exit.Source = new BitmapImage(new Uri("pack://application:,," + "/res/close.png"));
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
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

            try
            {
                int result = dbOperation.GetDbHelper().ExecuteSql(string.Format("call p_set_sampling_rate ('{0}','{1}','{2}')",
                                                    deptid, sb.ToString(), detecttype));

                if (result == 1)
                {
                    Toolkit.MessageBox.Show("抽检率设置成功！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                else
                {
                    Toolkit.MessageBox.Show("抽检率设置失败！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch
            {
                Toolkit.MessageBox.Show("抽检率设置失败2！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
