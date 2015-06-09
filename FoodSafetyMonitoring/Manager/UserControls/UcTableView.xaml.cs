using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
using Toolkit = Microsoft.Windows.Controls;

namespace FoodSafetyMonitoring.Manager.UserControls
{
    /// <summary>
    /// UcTableView.xaml 的交互逻辑
    /// </summary>
    public partial class UcTableView : UserControl
    {
        public delegate void DetailsRowEventHandler(string id);
        public event DetailsRowEventHandler DetailsRowEnvent;

        public UcTableView()
        {
            InitializeComponent();
            this.SizeChanged += new SizeChangedEventHandler(UcTableView_SizeChanged);
        }

        void UcTableView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (dt == null)
            {
                return;
            }
            _gridview.Columns.Clear();
            foreach (DataColumn c in dt.Columns)
            {
                GridViewColumn gvc = new GridViewColumn();
                gvc.Header = c.ColumnName;
                if (BShowDetails)
                {
                    gvc.Width = (_listview.ActualWidth - 65) / dt.Columns.Count;
                }
                else
                {
                    gvc.Width = (_listview.ActualWidth - 25) / dt.Columns.Count;
                }
                gvc.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
                //gvc.DisplayMemberBinding = (new Binding(c.ColumnName));
                FrameworkElementFactory text = new FrameworkElementFactory(typeof(TextBlock));
                text.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                text.SetValue(TextBlock.TextAlignmentProperty, TextAlignment.Center);
                text.SetBinding(TextBlock.TextProperty, new Binding(c.ColumnName));
                DataTemplate dataTemplate = new DataTemplate() { VisualTree = text };
                gvc.CellTemplate = dataTemplate;
                _gridview.Columns.Add(gvc);
            }
            if (BShowDetails)
            {
                GridViewColumn gvc_details = new GridViewColumn();
                gvc_details.Header = "详情";
                FrameworkElementFactory button_details = new FrameworkElementFactory(typeof(Button));
                button_details.SetResourceReference(Button.HorizontalContentAlignmentProperty, HorizontalAlignment.Center);
                button_details.SetValue(Button.WidthProperty, 40.0);
                button_details.AddHandler(Button.ClickEvent, new RoutedEventHandler(details_Click));
                button_details.SetBinding(Button.TagProperty, new Binding(dt.Columns[1].ColumnName));
                button_details.SetValue(Button.ContentProperty, ">>");
                button_details.SetValue(Button.ForegroundProperty, Brushes.Blue);
                button_details.SetValue(Button.FontSizeProperty, 14.0);
                button_details.SetBinding(Button.VisibilityProperty, new Binding(dt.Columns[0].ColumnName) { Converter = new VisibleBtnConverter() });
                DataTemplate dataTemplate_details = new DataTemplate() { VisualTree = button_details };
                gvc_details.CellTemplate = dataTemplate_details;
                _gridview.Columns.Add(gvc_details);
            }
            _listview.ItemsSource = null;
            _listview.ItemsSource = dt.DefaultView;
        }

        private DataTable dt;
        private string title;
        private List<int> columnNumbers;
        //private List<int> sumColumns = new List<int>();

        public void SetDataTable(DataTable _dt, string title, List<int> columnNumbers)
        {
            this.dt = _dt.Copy();
            this.title = title;
            this.columnNumbers = columnNumbers;
            _title.Text = "▪ " + title;

            _gridview.Columns.Clear();
            foreach (DataColumn c in dt.Columns)
            {
                GridViewColumn gvc = new GridViewColumn();
                gvc.Header = c.ColumnName;
                if (BShowDetails)
                {
                    gvc.Width = (_listview.ActualWidth - 65) / dt.Columns.Count;
                }
                else
                {
                    gvc.Width = (_listview.ActualWidth - 25) / dt.Columns.Count;
                }
                //gvc.DisplayMemberBinding = (new Binding(c.ColumnName));
                FrameworkElementFactory text = new FrameworkElementFactory(typeof(TextBlock));
                text.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                text.SetValue(TextBlock.TextAlignmentProperty, TextAlignment.Center);
                text.SetBinding(TextBlock.TextProperty, new Binding(c.ColumnName));
                DataTemplate dataTemplate = new DataTemplate() { VisualTree = text };
                gvc.CellTemplate = dataTemplate;
                gvc.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
                _gridview.Columns.Add(gvc);
            }
            if (BShowDetails)
            {
                GridViewColumn gvc_details = new GridViewColumn();
                gvc_details.Header = "详情";
                FrameworkElementFactory button_details = new FrameworkElementFactory(typeof(Button));
                button_details.SetResourceReference(Button.HorizontalContentAlignmentProperty, HorizontalAlignment.Center);
                button_details.SetValue(Button.WidthProperty, 40.0);
                button_details.AddHandler(Button.ClickEvent, new RoutedEventHandler(details_Click));
                button_details.SetBinding(Button.TagProperty, new Binding(dt.Columns[1].ColumnName));
                button_details.SetValue(Button.ContentProperty, ">>");
                button_details.SetValue(Button.ForegroundProperty, Brushes.Blue);
                button_details.SetValue(Button.FontSizeProperty, 14.0);
                button_details.SetBinding(Button.VisibilityProperty, new Binding(dt.Columns[0].ColumnName) { Converter = new VisibleBtnConverter() });
                DataTemplate dataTemplate_details = new DataTemplate() { VisualTree = button_details };
                gvc_details.CellTemplate = dataTemplate_details;
                _gridview.Columns.Add(gvc_details);
            }
            _listview.ItemsSource = dt.DefaultView;
        }

        public bool BShowDetails
        {
            get;
            set;
        }

        private void details_Click(object sender, RoutedEventArgs e)
        {
            if (DetailsRowEnvent != null)
            {
                DetailsRowEnvent((sender as Button).Tag.ToString());
            }
        }

        public void ExportExcel()
        {
            if ((dt == null) || (dt.Rows.Count == 0))
            {
                Toolkit.MessageBox.Show("当前可导出数据为零！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
            sfd.Filter = "导出文件 (*.csv)|*.csv";
            sfd.FilterIndex = 0;
            sfd.RestoreDirectory = true;
            sfd.Title = "导出文件保存路径";
            sfd.ShowDialog();
            string strFilePath = sfd.FileName;
            if (strFilePath != "")
            {
                if (File.Exists(strFilePath))
                {
                    File.Delete(strFilePath);
                }
                StreamWriter sw = new StreamWriter(new FileStream(strFilePath, FileMode.CreateNew), Encoding.Default);
                string tableHeader = " ";
                foreach (DataColumn c in dt.Columns)
                {
                    GridViewColumn gvc = new GridViewColumn();
                    tableHeader += c.ColumnName + ",";
                }
                sw.WriteLine(title);
                sw.WriteLine(tableHeader);
                

                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    DataRow row = dt.Rows[j];
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        sb.Append(row[i]);
                        sb.Append(",");
                    }
                    sw.WriteLine(sb);
                }

                //StringBuilder sum_sb = new StringBuilder();
                //for (int i = 0; i < dt.Columns.Count; i++)
                //{
                //    if (i == 0)
                //    {
                //        sum_sb.Append("共计");
                //    }
                //    else if (columnNumbers.Contains(i))
                //    {
                //        sum_sb.Append(sumColumns[columnNumbers.IndexOf(i)]);
                //    }
                //    sum_sb.Append(",");
                //}
                //sw.WriteLine(sum_sb);

                sw.Close();
                //MessageBox.Show("导出文件成功！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                Toolkit.MessageBox.Show("导出文件成功！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


    }

    public class MyColumn
    {
        //原始列名
        private string column_source;

        public string Column_source
        {
            get { return column_source; }
            set { column_source = value; }
        }
        /// <summary>
        /// 显示列名
        /// </summary>
        private string column_show;

        public string Column_show
        {
            get { return column_show; }
            set { column_show = value; }
        }
        /// <summary>
        /// 列宽
        /// </summary>
        private int width = 1;

        public int Width
        {
            get { return width; }
            set { width = value; }
        }
        /// <summary>
        /// 是否显示
        /// </summary>
        private bool bShow = true;

        public bool BShow
        {
            get { return bShow; }
            set { bShow = value; }
        }

        public MyColumn(string column_source, string column_show) 
        {
            this.column_show = column_show;
            this.column_source = column_source;
        }
    }


    public class BackGroundConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ListViewItem lvi = (ListViewItem)value;
            ListView lv = ItemsControl.ItemsControlFromItemContainer(lvi) as ListView;
            int index = lv.ItemContainerGenerator.IndexFromContainer(lvi);
            if (index % 2 == 0)
            {
                return Brushes.White;
            }
            else
            {
                return Brushes.GhostWhite;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    public class VisibleBtnConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.ToString() == "合计")
            {
                return Visibility.Hidden;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Visibility.Visible;
        }
    }
}
