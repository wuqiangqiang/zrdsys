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
    /// SysBigReport.xaml 的交互逻辑
    /// </summary>
    public partial class SysTzGrade : UserControl
    {
        private IDBOperation dbOperation;
        private string user_flag_tier;
        private string depttype;

        public SysTzGrade(IDBOperation dbOperation, string dept_type)
        {
            InitializeComponent();

            this.dbOperation = dbOperation;
            this.depttype = dept_type;
            user_flag_tier = (Application.Current.Resources["User"] as UserInfo).FlagTier;

            dtpStartDate.SelectedDate = DateTime.Now.AddDays(-1);
            dtpEndDate.SelectedDate = DateTime.Now;

            //检测站点
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

            ComboboxTool.InitComboboxSource(_detect_dept, string.Format("call p_dept_cxtj_hb({0},'{1}')", (Application.Current.Resources["User"] as UserInfo).ID, depttype), "cxtj");
            _detect_dept.SelectionChanged += new SelectionChangedEventHandler(_detect_dept_SelectionChanged);

        }

        void _detect_dept_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_detect_dept.SelectedIndex > 0)
            {
                ComboboxTool.InitComboboxSource(_detect_dept_2, string.Format("call p_xj_dept_cxtj_hb('{0}','{1}')", (_detect_dept.SelectedItem as Label).Tag, depttype), "cxtj");

            }
        }


        private void _query_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void _export_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
