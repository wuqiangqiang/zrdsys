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
using System.Windows.Forms.Integration;
using System.Windows.Threading;
using FoodSafetyMonitoring.dao;
using System.Data;
using Visifire.Charts;
using Toolkit = Microsoft.Windows.Controls;


namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// SysComparisonAndAnalysis.xaml 的交互逻辑
    /// </summary>
    public partial class SysComparisonAndAnalysis : UserControl
    {
        private IDBOperation dbOperation;
        private string user_flag_tier;
        private string dept_name;
        private string dept_name_2;
        private string dept_name_3;
        private readonly List<string> analysisThemes;
        private string depttype;
        private string detecttype;


        public SysComparisonAndAnalysis(IDBOperation dbOperation, string dept_type, string detect_type)
        {
            InitializeComponent();
            this.dbOperation = dbOperation;
            this.depttype = dept_type;
            this.detecttype = detect_type;
            user_flag_tier = (Application.Current.Resources["User"] as UserInfo).FlagTier;
            dtpStartDate.SelectedDate = DateTime.Now.AddDays(-1);
            dtpEndDate.SelectedDate = DateTime.Now;
            switch (user_flag_tier)
            {
                case "0": dept_name = "各省检测总量占比分析";
                    dept_name_2 = "各省阳性样本检出量占比分析";
                    dept_name_3 = "各省疑似阳性样本检出量占比分析";
                    break;
                case "1": dept_name = "各地市检测总量占比分析";
                    dept_name_2 = "各地市阳性样本检出量占比分析";
                    dept_name_3 = "各地市疑似阳性样本检出量占比分析";
                    break;
                case "2": dept_name = "各区县检测总量占比分析";
                    dept_name_2 = "各区县阳性样本检出量占比分析";
                    dept_name_3 = "各区县疑似阳性样本检出量占比分析";
                    break;
                case "3": dept_name = "各检测单位检测总量占比分析";
                    dept_name_2 = "各检测单位阳性样本检出量占比分析";
                    dept_name_3 = "各检测单位疑似阳性样本检出量占比分析";
                    break;
                case "4": dept_name = "各检测单位检测总量占比分析";
                    dept_name_2 = "各检测单位阳性样本检出量占比分析";
                    dept_name_3 = "各检测单位疑似阳性样本检出量占比分析";
                    break;
                default: break;
            }
            analysisThemes = new List<string>() { "-请选择-",
            dept_name, 
            dept_name_2,
            dept_name_3,
            //"直属与企业检测点检测总量比较分析",
            //"直属与企业检测点阳性样本检出比较分析",
            //"直属与企业检测点疑似阳性样本检出比较分析",
            "不同检测项目检测量占比分析",
            "不同检测项目阳性样本检测占比分析",
            "不同检测项目疑似阳性样本检测占比分析",
            "疑似阳性/阳性样本检出总量占比分析"};//初始化变量

            _analysis_theme.ItemsSource = analysisThemes;
            _analysis_theme.SelectedIndex = 0;
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
                case "各省检测总量占比分析": function = "p_dbfx_jczl_hb"; break;
                case "各地市检测总量占比分析": function = "p_dbfx_jczl_hb"; break;
                case "各区县检测总量占比分析": function = "p_dbfx_jczl_hb"; break;
                case "各检测单位检测总量占比分析": function = "p_dbfx_jczl_hb"; break;
                case "各省阳性样本检出量占比分析": function = "p_dbfx_jczl_yang_hb"; break;
                case "各地市阳性样本检出量占比分析": function = "p_dbfx_jczl_yang_hb"; break;
                case "各区县阳性样本检出量占比分析": function = "p_dbfx_jczl_yang_hb"; break;
                case "各检测单位阳性样本检出量占比分析": function = "p_dbfx_jczl_yang_hb"; break;
                case "各省疑似阳性样本检出量占比分析": function = "p_dbfx_jczl_like_yang_hb"; break;
                case "各地市疑似阳性样本检出量占比分析": function = "p_dbfx_jczl_like_yang_hb"; break;
                case "各区县疑似阳性样本检出量占比分析": function = "p_dbfx_jczl_like_yang_hb"; break;
                case "各检测单位疑似阳性样本检出量占比分析": function = "p_dbfx_jczl_like_yang_hb"; break;
                //case "直属与企业检测点检测总量比较分析": function = "p_dbfx_jcdfx"; break;
                //case "直属与企业检测点阳性样本检出比较分析": function = "p_dbfx_jcdyxfx"; break;
                //case "直属与企业检测点疑似阳性样本检出比较分析": function = "p_dbfx_jcdyxfx_like"; break;
                case "不同检测项目检测量占比分析": function = "p_dbfx_jcxm_hb"; break;
                case "不同检测项目阳性样本检测占比分析": function = "p_dbfx_jcxmyx_hb"; break;
                case "不同检测项目疑似阳性样本检测占比分析": function = "p_dbfx_jcxmyx_like_hb"; break;
                case "疑似阳性/阳性样本检出总量占比分析": function = "p_dbfx_yxyx_hb"; break;
                default: break;
            }

            table = dbOperation.GetDbHelper().GetDataSet(string.Format("call {0}({1},'{2}','{3}','{4}','{5}')", function, userId, (DateTime)dtpStartDate.SelectedDate, (DateTime)dtpEndDate.SelectedDate, depttype, detecttype)).Tables[0];


            switch (_analysis_theme.Text)
            {
                case "各省检测总量占比分析": table.Columns[0].ColumnName = "省名称";
                    table.Columns[1].ColumnName = "检测数量";
                    break;
                case "各地市检测总量占比分析": table.Columns[0].ColumnName = "地市名称";
                    table.Columns[1].ColumnName = "检测数量";
                    break;
                case "各区县检测总量占比分析": table.Columns[0].ColumnName = "区县名称";
                    table.Columns[1].ColumnName = "检测数量";
                    break;
                case "各检测单位检测总量占比分析": table.Columns[0].ColumnName = "检测单位名称";
                    table.Columns[1].ColumnName = "检测数量";
                    break;
                case "各省阳性样本检出量占比分析": table.Columns[0].ColumnName = "省名称";
                    table.Columns[1].ColumnName = "阳性样本数量";
                    break;
                case "各地市阳性样本检出量占比分析": table.Columns[0].ColumnName = "地市名称";
                    table.Columns[1].ColumnName = "阳性样本数量";
                    break;
                case "各区县阳性样本检出量占比分析": table.Columns[0].ColumnName = "区县名称";
                    table.Columns[1].ColumnName = "阳性样本数量";
                    break;
                case "各检测单位阳性样本检出量占比分析": table.Columns[0].ColumnName = "检测单位名称";
                    table.Columns[1].ColumnName = "阳性样本数量";
                    break;
                case "各省疑似阳性样本检出量占比分析": table.Columns[0].ColumnName = "省名称";
                    table.Columns[1].ColumnName = "疑似阳性样本数量";
                    break;
                case "各地市疑似阳性样本检出量占比分析": table.Columns[0].ColumnName = "地市名称";
                    table.Columns[1].ColumnName = "疑似阳性样本数量";
                    break;
                case "各区县疑似阳性样本检出量占比分析": table.Columns[0].ColumnName = "区县名称";
                    table.Columns[1].ColumnName = "疑似阳性样本数量";
                    break;
                case "各检测单位疑似阳性样本检出量占比分析": table.Columns[0].ColumnName = "检测单位名称";
                    table.Columns[1].ColumnName = "疑似阳性样本数量";
                    break;
                //case "直属与企业检测点检测总量比较分析": table.Columns[0].ColumnName = "检测单位性质";
                //    table.Columns[1].ColumnName = "检测数量";
                //    break;
                //case "直属与企业检测点阳性样本检出比较分析": table.Columns[0].ColumnName = "检测单位性质";
                //    table.Columns[1].ColumnName = "阳性样本数量";
                //    break;
                //case "直属与企业检测点疑似阳性样本检出比较分析": table.Columns[0].ColumnName = "检测单位性质";
                //    table.Columns[1].ColumnName = "疑似阳性样本数量";
                //    break;
                case "不同检测项目检测量占比分析": table.Columns.Remove("ItemID");
                    table.Columns[0].ColumnName = "检测项目";
                    table.Columns[1].ColumnName = "检测数量";
                    break;
                case "不同检测项目阳性样本检测占比分析": table.Columns.Remove("ItemID");
                    table.Columns[0].ColumnName = "检测项目";
                    table.Columns[1].ColumnName = "阳性样本数量";
                    break;
                case "不同检测项目疑似阳性样本检测占比分析": table.Columns.Remove("ItemID");
                    table.Columns[0].ColumnName = "检测项目";
                    table.Columns[1].ColumnName = "疑似阳性样本数量";
                    break;
                case "疑似阳性/阳性样本检出总量占比分析": table.Columns[0].ColumnName = "项目";
                    table.Columns[1].ColumnName = "检测数量";
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
