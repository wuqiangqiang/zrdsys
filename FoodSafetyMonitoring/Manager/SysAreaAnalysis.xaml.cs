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
    /// SysAreaAnalysis.xaml 的交互逻辑
    /// </summary>
    public partial class SysAreaAnalysis : UserControl
    {
        private IDBOperation dbOperation;
        private string depttype;
        private string detecttype;

        private readonly List<string> analysisThemes = new List<string>() { "-请选择-",
            "检测样本来源产地分布(全国)分析", 
            "检测样本来源产地分布(省内)分析",
            "阳性样本检出来源产地分布(全国)分析",
            "阳性样本检出来源产地分布(省内)分析",
            "疑似阳性样本检出来源产地分布(全国)分析",
            "疑似阳性样本检出来源产地分布(省内)分析"};//初始化变量

        public SysAreaAnalysis(IDBOperation dbOperation, string dept_type, string detect_type)
        {
            InitializeComponent();
            this.dbOperation = dbOperation;
            this.depttype = dept_type;
            this.detecttype = detect_type;
            _analysis_theme.ItemsSource = analysisThemes;
            _analysis_theme.SelectedIndex = 0;
            dtpStartDate.SelectedDate = DateTime.Now;
            dtpEndDate.SelectedDate = DateTime.Now;
        }

        private void _query_Click(object sender, RoutedEventArgs e)
        {
            if (_analysis_theme.SelectedIndex < 1)
            {
                Toolkit.MessageBox.Show("请先选择分析主题!!!");
                return;
            }

            if (dtpStartDate.SelectedDate.Value.Date > dtpEndDate.SelectedDate.Value.Date)
            {
                Toolkit.MessageBox.Show("开始时间大于结束时间，请重新选择！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            DataTable table = null;
            string userId = (Application.Current.Resources["User"] as UserInfo).ID;
            string function = "";

            switch (_analysis_theme.Text)
            {
                case "检测样本来源产地分布(全国)分析": function = "p_qyfx_provice_hb"; break;
                case "检测样本来源产地分布(省内)分析": function = "p_qyfx_city_hb"; break;
                case "阳性样本检出来源产地分布(全国)分析": function = "p_qyfx_provice_yang_hb"; break;
                case "阳性样本检出来源产地分布(省内)分析": function = "p_qyfx_city_yang_hb"; break;
                case "疑似阳性样本检出来源产地分布(全国)分析": function = "p_qyfx_provice_like_yang_hb"; break;
                case "疑似阳性样本检出来源产地分布(省内)分析": function = "p_qyfx_city_like_yang_hb"; break;
                default: break;
            }

            table = dbOperation.GetDbHelper().GetDataSet(string.Format("call {0}({1},'{2}','{3}','{4}','{5}')", function, userId, (DateTime)dtpStartDate.SelectedDate, (DateTime)dtpEndDate.SelectedDate, depttype, detecttype)).Tables[0];

            switch (_analysis_theme.Text)
            {
                case "检测样本来源产地分布(全国)分析": table.Columns[0].ColumnName = "来源省市";
                                                       table.Columns[1].ColumnName = "检测数量"; 
                                                       break;
                case "检测样本来源产地分布(省内)分析": table.Columns[0].ColumnName = "省内来源市州";
                                                       table.Columns[1].ColumnName = "检测数量";
                                                       break;
                case "阳性样本检出来源产地分布(全国)分析": table.Columns[0].ColumnName = "来源省市";
                                                           table.Columns[1].ColumnName = "阳性检出数量";
                                                           break;
                case "阳性样本检出来源产地分布(省内)分析": table.Columns[0].ColumnName = "省内来源市州";
                                                           table.Columns[1].ColumnName = "阳性检出数量";
                                                           break;
                case "疑似阳性样本检出来源产地分布(全国)分析": table.Columns[0].ColumnName = "来源省市";
                                                           table.Columns[1].ColumnName = "疑似阳性检出数量";
                                                           break;
                case "疑似阳性样本检出来源产地分布(省内)分析": table.Columns[0].ColumnName = "省内来源市州";
                                                           table.Columns[1].ColumnName = "疑似阳性检出数量";
                                                           break;
                default: break;
            }

            table.Columns.Add("占比(%)", Type.GetType("System.String"));
            double sum = 0;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                sum += Convert.ToDouble(table.Rows[i][1].ToString());
            }

            for (int i = 0; i < table.Rows.Count; i++)
            {
                table.Rows[i][2] = Math.Round(Convert.ToDouble(table.Rows[i][1].ToString()) / sum, 4, MidpointRounding.AwayFromZero) * 100 + "%";
            }

            _chart.Children.Clear();
            Chart chart = new Chart();
            chart.Background = Brushes.Transparent;
            chart.View3D = true;
            chart.Bevel = true;
            //Title title = new Title();
            //title.Text = _analysis_theme.Text;
            //title.FontFamily = new FontFamily("楷体");
            //title.FontSize = 16;
            DataSeries dataSeries = new DataSeries();
            dataSeries.RenderAs = RenderAs.Pie;

            for (int i = 0; i < table.Rows.Count; i++)
            {
                DataPoint point = new DataPoint();
                point.AxisXLabel = table.Rows[i][0].ToString();
                point.YValue = Convert.ToDouble(table.Rows[i][1].ToString());
                point.LabelStyle = LabelStyles.Inside;
                point.LabelText = table.Rows[i][0].ToString();
                point.LabelFontFamily = new FontFamily("微软雅黑");
                point.LabelFontSize = 12;
                dataSeries.DataPoints.Add(point);
            }
            chart.Series.Add(dataSeries);
            //chart.Titles.Add(title);
            _chart.Children.Add(chart);

            //计算报表总条数
            int row_count = 0;

            if (table.Rows.Count != 0)
            {
                table.Rows.Add(new object[] { "合计", sum, "100%" });

                row_count = table.Rows.Count - 1;
            }
            else
            {
                row_count = 0;
            }

            //string table_title = "数据统计时间:" + dtpStartDate.Value.Value.Year + "年" + dtpStartDate.Value.Value.Month + "月" + dtpStartDate.Value.Value.Day + "日到" + dtpEndDate.Value.Value.Year + "年" + dtpEndDate.Value.Value.Month + "月" + dtpEndDate.Value.Value.Day + "日" + "  合计" + row_count + "条数据";

            _title.Text = _analysis_theme.Text;
            _title_1.Text = string.Format("合计{0}条数据", row_count);
            _title_2.Text = _analysis_theme.Text;
            _tableview.SetDataTable(table, "", new List<int>());

        }

        private void _export_Click(object sender, RoutedEventArgs e)
        {
            _tableview.ExportExcel();
        }
    }
}
