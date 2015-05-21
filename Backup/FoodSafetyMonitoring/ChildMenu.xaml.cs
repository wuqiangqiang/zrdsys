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
using System.Windows.Interop;
using System.Windows.Forms.Integration;
using FoodSafetyMonitoring.Manager;

namespace FoodSafetyMonitoring
{
    /// <summary>
    /// ChildMenu.xaml 的交互逻辑
    /// </summary>
    public partial class ChildMenu : UserControl
    {
        private List<MyChildMenu> childMenus;
        public ChildMenu(List<MyChildMenu> childMenus)
        {
            InitializeComponent();
            this.childMenus = childMenus;
            if (childMenus.Count != 0)
            {
                switch (childMenus[0].mainmenu_num)
                {
                    case 2:loadMenu(_grid_1);
                           break;
                    case 3: _grid_1.HorizontalAlignment = HorizontalAlignment.Center;
                            loadMenu(_grid_1);
                            break;
                    case 4: _grid_1.HorizontalAlignment = HorizontalAlignment.Right;
                            loadMenu(_grid_1);
                            break;
                    case 5: loadMenu(_grid_2);
                            break;
                    case 6: _grid_2.HorizontalAlignment = HorizontalAlignment.Center;
                            loadMenu(_grid_2);
                            break;
                    case 7: _grid_2.HorizontalAlignment = HorizontalAlignment.Right;
                            loadMenu(_grid_2);
                            break;
                    case 8: _grid_2.HorizontalAlignment = HorizontalAlignment.Right;
                            loadMenu(_grid_2);
                            break;
                    default: break;

                }
            }

        }

        public void loadMenu(Grid grid)
        {
            grid.Children.Clear();
            for (int i = 0; i < childMenus.Count; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions[2 * i ].Width = GridLength.Auto;
                grid.ColumnDefinitions[2 * i + 1].Width = new GridLength(12, GridUnitType.Pixel);

                childMenus[i].btn.SetValue(Grid.ColumnProperty, 2*i);
                grid.Children.Add(childMenus[i].btn);

            }
            
        }
       
    }

    public class MyChildMenu
    {
        public Button btn;
        public string name;
        MainWindow mainWindow;
        public TabControl tab;
        public int mainmenu_num;
        TabItem temptb = null;

        public MyChildMenu(string name, MainWindow mainWindow,int mainmenu_num)
        {
            this.name = name;
            this.mainWindow = mainWindow;
            this.mainmenu_num = mainmenu_num;

            btn = new Button();
            btn.Content = name;
            btn.MinWidth = 30;
            this.tab = mainWindow._tab;
            btn.Click += new RoutedEventHandler(this.btn_Click);
        }

        private void btn_Click(object sender, System.EventArgs e)
        {
            int flag_exits = 0;
            foreach (TabItem item in tab.Items)
            {
                if (item.Header.ToString() == name)
                {
                    tab.SelectedItem = item;
                    flag_exits = 1;
                    break;
                }
            }
            if (flag_exits == 0)
            {
                temptb = new TabItem();
                temptb.Header = name;
                switch (name)
                {
                    case "首页": temptb.Content = new UcMainPage();
                        break;
                    case "新建": temptb.Content = new UcDetectBillManager();
                        break;
                    case "列表": temptb.Content = new UcDetectInquire(mainWindow.dbOperation);
                        break;
                    case "日报表": temptb.Content = new SysDayReport(mainWindow.dbOperation);
                        break;
                    case "月报表": temptb.Content = new SysMonthReport(mainWindow.dbOperation);
                        break;
                    case "年报表": temptb.Content = new SysYearReport(mainWindow.dbOperation);
                        break;
                    case "自定义报表": temptb.Content = new SysDesignReport(mainWindow.dbOperation);
                        break;
                    case "对比分析": temptb.Content = new SysComparisonAndAnalysis(mainWindow.dbOperation);
                        break;
                    case "趋势分析": temptb.Content = new SysTrendAnalysis(mainWindow.dbOperation);
                        break;
                    case "区域分析": temptb.Content = new SysAreaAnalysis(mainWindow.dbOperation);
                        break;
                    case "任务分配": temptb.Content = new UcTaskAllocation();
                        break;
                    case "任务考评": temptb.Content = new SysTaskCheck(mainWindow.dbOperation);
                        break;
                    case "实时风险": temptb.Content = new SysWarningInfo(mainWindow.dbOperation);
                        break;
                    case "预警复核": temptb.Content = new SysReviewInfo(mainWindow.dbOperation);
                        break;
                    case "预警数据统计": temptb.Content = new SysWarningReport(mainWindow.dbOperation);
                        break;
                    case "复核日志": temptb.Content = new SysReviewLog(mainWindow.dbOperation);
                        break;
                    case "部门管理": temptb.Content = new SysDeptManager(mainWindow.dbOperation);
                        break;
                    case "角色管理": temptb.Content = new SysRoleManager();
                        break;
                    case "权限管理": temptb.Content = new SysRolePowerManager();
                        break;
                    case "用户管理": temptb.Content = new SysUserManager();
                        break;
                    case "系统日志": temptb.Content = new SysLogManager();
                        break;
                    case "修改密码": temptb.Content = new SysModifyPassword();
                        break;
                    case "图片上传": temptb.Content = new SysLoadPicture(mainWindow.dbOperation);
                        break;
                    case "帮助": temptb.Content = new UcUnrealizedModul();
                        break;
                    case "关于": temptb.Content = new SysHelp();
                        break;
                    default: break;
                }
                tab.Items.Add(temptb);
                tab.SelectedIndex = tab.Items.Count - 1;
            }

            //mainWindow.IsEnbleMouseEnterLeave = false;
            //if (uc is IClickChildMenuInitUserControlUI)
            //{
            //    ((IClickChildMenuInitUserControlUI)uc).InitUserControlUI();
            //}
        }
    }
}
