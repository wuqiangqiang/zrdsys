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
    public partial class UcTableOperableView : UserControl
    {
        public delegate void DeleteRowEventHandler(string id);
        public event DeleteRowEventHandler DeleteRowEnvent;
        public delegate void ModifyRowEventHandler(string id);
        public event ModifyRowEventHandler ModifyRowEnvent;
        public delegate void DetailsRowEventHandler(string id);
        public event DetailsRowEventHandler DetailsRowEnvent;
        public delegate void StateRowEventHandler(string id);
        public event StateRowEventHandler StateRowEnvent;
        public delegate void GetDataByPageNumberEventHandler();
        public event GetDataByPageNumberEventHandler GetDataByPageNumberEvent;


        public UcTableOperableView()
        {
            InitializeComponent();
            //BShowDelete = true;
            _page.Visibility = Visibility.Hidden;
            _listview.SizeChanged += new SizeChangedEventHandler(_listview_SizeChanged);
        }

        void _listview_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RowMax = (int)_listview.ActualHeight / 33;
            PageIndex = 1;

            if (GetDataByPageNumberEvent != null)
            {
                GetDataByPageNumberEvent();
            }

            update();
        }

        public int RowTotal//总行数
        {
            get { return Convert.ToInt32(textblock_row_sum.Text); }
            set { textblock_row_sum.Text = value.ToString(); }
        }
        private int PageTotal//总页数
        {
            get { return Convert.ToInt32(textblock_page_sum.Text); }
            set { textblock_page_sum.Text = value.ToString(); }
        }
        public int RowMax;//当页最大显示行数 
        public int PageIndex//当前第几页
        {
            get { return Convert.ToInt32(txtCurrentPage.Text); }
            set { txtCurrentPage.Text = value.ToString(); }
        }

        private Dictionary<string, MyColumn> myColumns = new Dictionary<string, MyColumn>();

        public Dictionary<string, MyColumn> MyColumns
        {
            get { return myColumns; }
            set { myColumns = value; }
        }


        private void update()
        {
            if (table == null)
            {
                return;
            }
            _gridview.Columns.Clear();
            //int sumWidth = 0;
            //for (int j = 0; j < table.Columns.Count; j++)
            //{
            //    int maxWidth = System.Text.Encoding.Default.GetByteCount(table.Columns[j].ColumnName.ToString());
            //    for (int i = 0; i < table.Rows.Count; i++)
            //    {
            //        if (maxWidth < System.Text.Encoding.Default.GetByteCount(table.Rows[i][j].ToString().Trim()))
            //        {
            //            maxWidth = System.Text.Encoding.Default.GetByteCount(table.Rows[i][j].ToString().Trim());
            //        }
            //    }
            //    sumWidth += maxWidth;
            //    myColumns[table.Columns[j].ColumnName.ToLower()].Width = maxWidth;
            //}
            for (int i = 0; i < table.Columns.Count; i++)
            {
                DataColumn c = table.Columns[i];
                if (myColumns[c.ColumnName.ToLower()].Column_show == "总行数")
                {
                    if (table.Rows.Count != 0)
                    {
                        RowTotal = Convert.ToInt32(table.Rows[0][c.ColumnName].ToString());
                        PageTotal = (int)Math.Ceiling((double)RowTotal / (double)RowMax);
                    }
                    else
                    {
                        RowTotal = 0;
                        PageTotal = 0;
                    }
                }


                if (!myColumns[c.ColumnName.ToLower()].BShow)
                {
                    continue;
                }
                GridViewColumn gvc = new GridViewColumn();
                gvc.Header = myColumns[c.ColumnName.ToLower()].Column_show;
                FrameworkElementFactory text = new FrameworkElementFactory(typeof(TextBlock));
                text.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                text.SetValue(TextBlock.TextAlignmentProperty, TextAlignment.Center);
                text.SetBinding(TextBlock.ForegroundProperty, new Binding(c.ColumnName) { Converter = new RedFontConverter() });
                if (table.Columns[i].ColumnName.ToLower().Contains("date"))
                {
                    text.SetBinding(TextBlock.TextProperty, new Binding(c.ColumnName));
                }
                else
                {
                    text.SetBinding(TextBlock.TextProperty, new Binding(c.ColumnName));
                }
                DataTemplate dataTemplate = new DataTemplate() { VisualTree = text };
                gvc.CellTemplate = dataTemplate;
                gvc.Width = 10 * myColumns[c.ColumnName.ToLower()].Width;
                _gridview.Columns.Add(gvc);
            }
            if (BShowModify)
            {
                GridViewColumn gvc_modify = new GridViewColumn();
                gvc_modify.Header = "修改";
                FrameworkElementFactory button_modify = new FrameworkElementFactory(typeof(Button));
                button_modify.SetResourceReference(Button.HorizontalContentAlignmentProperty, HorizontalAlignment.Center);
                button_modify.SetValue(Button.WidthProperty, 20.0);
                button_modify.AddHandler(Button.ClickEvent, new RoutedEventHandler(modify_Click));
                button_modify.SetBinding(Button.TagProperty, new Binding(table.Columns[0].ColumnName));
                button_modify.SetResourceReference(Button.StyleProperty, "ListModifyImageButtonTemplate");
                DataTemplate dataTemplate_modify = new DataTemplate() { VisualTree = button_modify };
                gvc_modify.CellTemplate = dataTemplate_modify;
                _gridview.Columns.Add(gvc_modify);
            }

            if (BShowDelete)
            {
                GridViewColumn gvc_delete = new GridViewColumn();
                gvc_delete.Header = "删除";
                FrameworkElementFactory button_delete = new FrameworkElementFactory(typeof(Button));
                button_delete.SetValue(Button.HorizontalContentAlignmentProperty, HorizontalAlignment.Center);
                button_delete.SetValue(Button.WidthProperty, 20.0);
                button_delete.AddHandler(Button.ClickEvent, new RoutedEventHandler(delete_Click));
                button_delete.SetResourceReference(Button.StyleProperty, "ListDeleteImageButtonTemplate");
                button_delete.SetBinding(Button.TagProperty, new Binding(table.Columns[0].ColumnName));
                DataTemplate dataTemplate_delete = new DataTemplate() { VisualTree = button_delete };

                gvc_delete.CellTemplate = dataTemplate_delete;
                _gridview.Columns.Add(gvc_delete);
            }

            if (BShowDetails)
            {
                GridViewColumn gvc_details = new GridViewColumn();
                gvc_details.Header = "详情";
                FrameworkElementFactory button_details = new FrameworkElementFactory(typeof(Button));
                button_details.SetResourceReference(Button.HorizontalContentAlignmentProperty, HorizontalAlignment.Center);
                button_details.SetValue(Button.WidthProperty, 40.0);
                button_details.AddHandler(Button.ClickEvent, new RoutedEventHandler(details_Click));
                button_details.SetBinding(Button.TagProperty, new Binding(table.Columns[0].ColumnName));
                //button_details.SetResourceReference(Button.StyleProperty, "ListDetailsImageButtonTemplate");
                button_details.SetValue(Button.ContentProperty, ">>");
                button_details.SetValue(Button.ForegroundProperty, Brushes.Blue);
                button_details.SetValue(Button.FontSizeProperty, 14.0);
                //button_details.SetValue(Button.FontFamilyProperty, "黑体");
                //button_details.SetValue(Button.FontWeightProperty, FontWeight.Compare);
                DataTemplate dataTemplate_details = new DataTemplate() { VisualTree = button_details };
                gvc_details.CellTemplate = dataTemplate_details;
                _gridview.Columns.Add(gvc_details);
            }

            if (BShowState)
            {
                GridViewColumn gvc_state = new GridViewColumn();
                gvc_state.Header = "状态";
                FrameworkElementFactory button_state = new FrameworkElementFactory(typeof(Button));
                button_state.SetResourceReference(Button.HorizontalContentAlignmentProperty, HorizontalAlignment.Center);
                button_state.SetValue(Button.WidthProperty, 60.0);
                button_state.AddHandler(Button.ClickEvent, new RoutedEventHandler(state_Click));
                button_state.SetBinding(Button.TagProperty, new Binding(table.Columns[0].ColumnName));
                button_state.SetBinding(Button.ContentProperty, new Binding(table.Columns[1].ColumnName));
                button_state.SetValue(Button.ForegroundProperty, Brushes.Blue);
                button_state.SetValue(Button.FontSizeProperty, 12.0);

                DataTemplate dataTemplate_state = new DataTemplate() { VisualTree = button_state };
                gvc_state.CellTemplate = dataTemplate_state;
                _gridview.Columns.Add(gvc_state);
            }
            _listview.ItemsSource = null;
            _listview.ItemsSource = table.DefaultView;

            if (PageTotal > 1)
            {
                _page.Visibility = Visibility.Visible;
            }
            else
            {
                _page.Visibility = Visibility.Hidden;
            }
        }

        public bool BShowModify { set; get; }
        public bool BShowDelete { set; get; }
        public bool BShowDetails { set; get; }
        public bool BShowState { set; get; }

        private DataTable table;
        public DataTable Table
        {
            set
            {
                this.table = value;
                update();
            }
            get { return table; }
        }
        private string title;
        public string Title
        {
            set
            {
                this.title = value;
                _title.Text = "▪ " + title;
            }
        }

        private void modify_Click(object sender, RoutedEventArgs e)
        {
            if (ModifyRowEnvent != null)
            {
                ModifyRowEnvent((sender as Button).Tag.ToString());
            }
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            if (DeleteRowEnvent != null)
            {
                DeleteRowEnvent((sender as Button).Tag.ToString());
            }
        }

        private void details_Click(object sender, RoutedEventArgs e)
        {
            if (DetailsRowEnvent != null)
            {
                DetailsRowEnvent((sender as Button).Tag.ToString());
            }
        }

        private void state_Click(object sender, RoutedEventArgs e)
        {
            if (StateRowEnvent != null)
            {
                StateRowEnvent((sender as Button).Tag.ToString());
            }
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            if (PageIndex > 1)
            {
                PageIndex = PageIndex - 1;
                if (GetDataByPageNumberEvent != null)
                {
                    GetDataByPageNumberEvent();
                }
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (PageIndex < (RowTotal % RowMax == 0 ? (int)RowTotal / RowMax : (int)RowTotal / RowMax + 1))
            {
                PageIndex = PageIndex + 1;
                if (GetDataByPageNumberEvent != null)
                {
                    GetDataByPageNumberEvent();
                }
            }
        }

        private void btnFirst_Click(object sender, RoutedEventArgs e)
        {
            PageIndex = 1;
            if (GetDataByPageNumberEvent != null)
            {
                GetDataByPageNumberEvent();
            }
        }


        private void btnLast_Click(object sender, RoutedEventArgs e)
        {
            if (PageIndex != (RowTotal % RowMax == 0 ? (int)RowTotal / RowMax : (int)RowTotal / RowMax + 1))
            {
                PageIndex = (RowTotal % RowMax == 0 ? (int)RowTotal / RowMax : (int)RowTotal / RowMax + 1);
                if (GetDataByPageNumberEvent != null)
                {
                    GetDataByPageNumberEvent();
                }
            }
        }


        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            if (GetDataByPageNumberEvent != null)
            {
                GetDataByPageNumberEvent();
            }
        }

        private void txtCurrentPage_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (RowMax == 0)
            {
                return;
            }
            int pageIndex = 1;
            try
            {
                pageIndex = Convert.ToInt32(txtCurrentPage.Text);
            }
            catch
            {
                txtCurrentPage.Text = "1";
            }
            if (pageIndex > (RowTotal % RowMax == 0 ? (int)RowTotal / RowMax : (int)RowTotal / RowMax + 1))
            {
                txtCurrentPage.Text = (RowTotal % RowMax == 0 ? (int)RowTotal / RowMax : (int)RowTotal / RowMax + 1).ToString();
            }
        }




        //导出文件
        public void ExportExcel(DataTable dt)
        {
            //查询数据为0时，提示不能导出
            if (dt.Rows.Count == 0) 
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
                    if (myColumns[c.ColumnName.ToLower()].Column_show != "总行数")
                    {
                        tableHeader += myColumns[c.ColumnName.ToLower()].Column_show + ",";
                    }  
                }
                sw.WriteLine(title);
                sw.WriteLine(tableHeader);

                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    DataRow row = dt.Rows[j];
                    StringBuilder sb = new StringBuilder();
                    //总行数在最后一列控制不导出
                    for (int i = 0; i < dt.Columns.Count - 1; i++)
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

        public class RedFontConverter : IValueConverter
        {

            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                if (value.ToString() == "疑似阳性" || value.ToString() == "阳性")
                {
                    return Brushes.Red;
                }
                else
                {
                    return Brushes.Black;
                }
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                return Brushes.Black;
            }
        }


    }



}
