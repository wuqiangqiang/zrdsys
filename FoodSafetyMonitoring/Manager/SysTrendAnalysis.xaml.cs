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

using Toolkit = Microsoft.Windows.Controls;
using Visifire.Charts;

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// SysTrendAnalysis.xaml 的交互逻辑
    /// </summary>
    public partial class SysTrendAnalysis : UserControl
    {
        IDBOperation dbOperation;
        public SysTrendAnalysis(IDBOperation dbOperation)
        {
            InitializeComponent();
            this.dbOperation = dbOperation;
            DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_year_count({0})", (Application.Current.Resources["User"] as UserInfo).ID)).Tables[0];
            List<string> years = new List<string>();
            years.Add("-请选择-");
            for (int i = 0; i < table.Rows.Count; i++)
            {
                years.Add(table.Rows[i][0].ToString());
            }

            _year.ItemsSource = years;
            _year.SelectedIndex = 0;

            List<string> analysisThemes = new List<string>();
            analysisThemes.Add("-请选择-");
            analysisThemes.Add("年度各项目检测执行趋势分析");
            analysisThemes.Add("年度各检测项目阳性样本检出趋势分析");
            analysisThemes.Add("年度各检测项目疑似阳性样本检出趋势分析");
            _analysis_theme.ItemsSource = analysisThemes;
            _analysis_theme.SelectedIndex = 0;

            this.SizeChanged += new SizeChangedEventHandler(SysTrendAnalysis_SizeChanged);
            _chart.SizeChanged += new SizeChangedEventHandler(_chart_SizeChanged);
        }

        void SysTrendAnalysis_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        void _chart_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }


        private DataTable GetTrendAnalysisData(string theme, string year)
        {

            try
            {
                if (_analysis_theme.SelectedIndex > 0 && _analysis_theme.Text == "年度各项目检测执行趋势分析")
                {
                    return dbOperation.GetDbHelper().GetDataSet(string.Format("call sp_ndxmqs('{0}','{1}')", (Application.Current.Resources["User"] as UserInfo).ID, year)).Tables[0];
                }
                else if (_analysis_theme.SelectedIndex > 0 && _analysis_theme.Text == "年度各检测项目阳性样本检出趋势分析")
                {
                    return dbOperation.GetDbHelper().GetDataSet(string.Format("call sp_ndyxqs('{0}','{1}')", (Application.Current.Resources["User"] as UserInfo).ID, year)).Tables[0];
                }
                else if (_analysis_theme.SelectedIndex > 0 && _analysis_theme.Text == "年度各检测项目疑似阳性样本检出趋势分析")
                {
                    return dbOperation.GetDbHelper().GetDataSet(string.Format("call sp_ndyxqs_like('{0}','{1}')", (Application.Current.Resources["User"] as UserInfo).ID, year)).Tables[0];
                }
                else
                {
                    return null;
                }

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(".SysTrendAnalysis.GetTrendAnalysisData异常");
                throw new Exception(e.Message);
            }
        }

        private Chart chart = null;

        private void _query_Click(object sender, RoutedEventArgs e)
        {
            if (_analysis_theme.SelectedIndex < 1)
            {
                  Toolkit.MessageBox.Show("请先选择分析主题!!!");
                return;
            }
            if (_year.SelectedIndex < 1)
            {
                  Toolkit.MessageBox.Show("请先选择日期!!!");
                return;
            }

            DataTable table = GetTrendAnalysisData(_analysis_theme.Text, _year.Text);
            table.Columns.Remove("itemid");

            table.Columns[0].ColumnName = "检测项目";
            table.Columns[1].ColumnName = "1月";
            table.Columns[2].ColumnName = "2月";
            table.Columns[3].ColumnName = "3月";
            table.Columns[4].ColumnName = "4月";
            table.Columns[5].ColumnName = "5月";
            table.Columns[6].ColumnName = "6月";
            table.Columns[7].ColumnName = "7月";
            table.Columns[8].ColumnName = "8月";
            table.Columns[9].ColumnName = "9月";
            table.Columns[10].ColumnName = "10月";
            table.Columns[11].ColumnName = "11月";
            table.Columns[12].ColumnName = "12月";

            table.Columns.Add("合计", Type.GetType("System.String"));

            int sum_row = 0;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                int sum = 0;
                for (int j = 1; j < table.Columns.Count - 1; j++)
                {
                    sum += Convert.ToInt32(table.Rows[i][j].ToString());
                }
                sum_row += sum;
                table.Rows[i][table.Columns.Count - 1] = sum;
            }

            int sum_column = 0;
            //计算报表总条数
            int row_count = 0;

            if (table.Rows.Count != 0)
            {
                table.Rows.Add(table.NewRow()[0] = "合计");
                for (int j = 1; j < table.Columns.Count; j++)
                {
                    int sum = 0;
                    for (int i = 0; i < table.Rows.Count - 1; i++)
                    {
                        sum += Convert.ToInt32(table.Rows[i][j].ToString());
                    }
                    sum_column += sum;
                    table.Rows[table.Rows.Count - 1][j] = sum;
                }

                row_count = table.Rows.Count - 1;
            }
            else
            {
                row_count = 0;
            }

            _tableview.SetDataTable(table, "数据统计时间:" + _year.Text + "年" + "  合计" + row_count + "条数据", new List<int>());

            _chart.Children.Clear(); 
            chart = new Chart();
            chart.Background = Brushes.Transparent;
            chart.View3D = true;
            chart.Bevel = true;
            Title title = new Title();
            title.Text = _year.Text + "年" + _analysis_theme.Text;
            title.FontFamily = new FontFamily("楷体");
            title.FontSize = 16;
            chart.Titles.Add(title);

            for (int i = 0; i < table.Rows.Count; i++)
            {
                DataSeries dataSeries = new DataSeries();
                dataSeries.RenderAs = RenderAs.Line;
                dataSeries.LegendText = table.Rows[i][0].ToString();
                dataSeries.LabelFontFamily = new FontFamily("楷体");
                for (int j = 1; j < table.Columns.Count - 1; j++)
                {
                    DataPoint point = new DataPoint();
                    point.LabelStyle = LabelStyles.OutSide;
                    point.LabelFontSize = 14;
                    point.AxisXLabel = table.Columns[j].ColumnName;
                    point.YValue = Convert.ToDouble(table.Rows[i][j].ToString());
                    dataSeries.DataPoints.Add(point);
                }
                chart.Series.Add(dataSeries);
            }
          _chart.Children.Add(chart);
        }

        private void _export_Click(object sender, RoutedEventArgs e)
        {
            _tableview.ExportExcel();
        }
    }
}
