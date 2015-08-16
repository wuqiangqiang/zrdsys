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
using System.Windows.Forms.Integration;
using System.Data;
using FoodSafetyMonitoring.Common;
using FoodSafetyMonitoring.Manager.UserControls;
using Toolkit = Microsoft.Windows.Controls;

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// SysAnalysis.xaml 的交互逻辑
    /// </summary>
    public partial class SysAnalysis : UserControl
    {
        private IDBOperation dbOperation;
        private string user_flag_tier;
        private string depttype;
        private readonly List<string> analysisThemes1;
        private readonly List<string> analysisThemes2;
        private readonly List<string> analysisThemes3;
        private readonly List<string> analysisThemes4;
        private readonly List<string> area;
        private string Type;


        public SysAnalysis(IDBOperation dbOperation,string type,string dept_type)
        {
            InitializeComponent();
            this.dbOperation = dbOperation;
            this.Type = type;
            this.depttype = dept_type;

            dtpStartDate.SelectedDate = DateTime.Now.AddDays(-1);
            dtpEndDate.SelectedDate = DateTime.Now;
            user_flag_tier = (Application.Current.Resources["User"] as UserInfo).FlagTier;

            analysisThemes1 = new List<string>() { "-请选择-",
            "检测样本量排名分析", 
            "检测量占比分析",
            "不同检测项目检测样本量占比分析"};//初始化变量

            analysisThemes2 = new List<string>() { "-请选择-",
            "检测任务完成率", 
            "检测执行趋势分析",
            "执行力指数评估"};//初始化变量

            analysisThemes3 = new List<string>() { "-请选择-",
            "实际执行抽检率分析", 
            "规定抽检率与实际执行抽检率对比分析",
            "不同检测项目抽检率对比分析"};//初始化变量

            analysisThemes4 = new List<string>() { "-请选择-",
            "疑似阳性样本检出排名（按地市）", 
            "疑似阳性样本占比分析（按检测项目）",
            "疑似阳性样本检出趋势分析（按年月）", 
            "疑似阳性样本来源区域分析",
            "疑似阳性样本来源区域分析",
            "综合风险评估指数",
            "检测项目风险评估指数"};//初始化变量

            area = new List<string>() { "-请选择-",
            "全国", 
            "省内"};//初始化变量

            switch (user_flag_tier)
            {
                case "0": _dept_name.Text = "选择省:";
                    break;
                case "1": _dept_name.Text = "选择市(州):";
                    break;
                case "2": _dept_name.Text = "选择区县:";
                    break;
                case "3": _dept_name.Text = "选择检测单位:";
                    break;
                case "4": _dept_name.Text = "选择检测单位:";
                    break;
                default: break;
            }
            //检测单位
            ComboboxTool.InitComboboxSource(_detect_dept, string.Format("call p_dept_cxtj_hb({0},'{1}')", (Application.Current.Resources["User"] as UserInfo).ID, depttype), "cxtj");

            ComboboxTool.InitComboboxSource(_detect_item, "SELECT ItemID,ItemNAME FROM t_det_item_hb WHERE OPENFLAG = '1'", "cxtj");

            if (Type == "1")
            {
                _analysis_theme.ItemsSource = analysisThemes1;
                _analysis_theme.SelectedIndex = 0;
            }
            else if (Type == "2")
            {
                _analysis_theme.ItemsSource = analysisThemes2;
                _analysis_theme.SelectedIndex = 0;
            }
            else if (Type == "3")
            {
                _analysis_theme.ItemsSource = analysisThemes3;
                _analysis_theme.SelectedIndex = 0;
            }
            else if (Type == "4")
            {
                _analysis_theme.ItemsSource = analysisThemes4;
                _analysis_theme.SelectedIndex = 0;
            }

            _area.ItemsSource = area;
            _area.SelectedIndex = 0;
            
        }

        private void _query_Click(object sender, RoutedEventArgs e)
        {
            _sj.Visibility = Visibility.Visible;
            _hj.Visibility = Visibility.Visible;
            _title_1.Text = "0";

            //if (row_count == 0)
            //{
                Toolkit.MessageBox.Show("没有查询到数据！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            //}
        }

        private void _export_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
