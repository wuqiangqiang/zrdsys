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
using System.Data;

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
            //定义数组存放：二级菜单外部大控件
            Expander[] expanders = new Expander[] { _expander_0, _expander_1, _expander_2, _expander_3, _expander_4, _expander_5 };

            //先让所有控件都可见
            for (int i = 0; i < 6; i++)
            {
                expanders[i].Visibility = Visibility.Visible;
            }
            //再根据二级菜单的个数隐藏部门控件
            for(int i = childMenus.Count; i < 6; i++ )
            {
                expanders[i].Visibility = Visibility.Hidden;
            }
            //加载二、三级菜单
            loadMenu();
        }

        public void loadMenu()
        {
            //定义数组存放：三级菜单的控件
            Grid[] grids = new Grid[] { _grid_0, _grid_1, _grid_2, _grid_3, _grid_4, _grid_5 };
            //定义数组存放：二级菜单的控件
            TextBlock[] texts = new TextBlock[] { _text_0, _text_1, _text_2, _text_3, _text_4, _text_5 };
            //先将三级菜单控件进行清空
            for (int i = 0; i < grids.Length; i++)
            {
                grids[i].Children.Clear();
            }

            for (int i = 0; i < childMenus.Count; i++)
            {
                //二级菜单文字
                texts[i].Text = childMenus[i].name;

                //三级菜单
                int j = 0;
                foreach (DataRow row in childMenus[i].child_childmenu)
                {
                    grids[i].RowDefinitions.Add(new RowDefinition());
                    grids[i].RowDefinitions[j].Height = new GridLength(25, GridUnitType.Pixel);
                    childMenus[i].buttons[j].SetValue(Grid.RowProperty, j);
                    grids[i].Children.Add(childMenus[i].buttons[j]);
                    j = j + 1;
                }
            }    
        }
       
    }

    public class MyChildMenu
    {
        //public Button btn;
        public List<Button> buttons;
        public string name;
        MainWindow mainWindow;
        public TabControl tab;
        public DataRow[] child_childmenu;
        TabItem temptb = null;

        public MyChildMenu(string name, MainWindow mainWindow, DataRow[] child_childmenu)
        {
            this.name = name;
            this.mainWindow = mainWindow;
            this.child_childmenu = child_childmenu;
            this.tab = mainWindow._tab;
            buttons = new List<Button>();

            foreach (DataRow row in child_childmenu)
            {
                Button btn = new Button();
                btn.Content = row["SUB_NAME"].ToString();
                btn.Tag = row["SUB_ID"].ToString();
                btn.MinWidth = 30;
                btn.Click += new RoutedEventHandler(this.btn_Click);
                buttons.Add(btn);
            }
        }

        private void btn_Click(object sender, System.EventArgs e)
        {
            int flag_exits = 0;
            foreach (TabItem item in tab.Items)
            {
                if (item.Header.ToString() == (sender as Button).Content.ToString())
                {
                    tab.SelectedItem = item;
                    flag_exits = 1;
                    break;
                }
            }
            if (flag_exits == 0)
            {
                temptb = new TabItem();
                temptb.Header = (sender as Button).Content.ToString();
                switch ((sender as Button).Tag.ToString())
                {
                    //首页
                    case "1": temptb.Content = new UcMainPage();
                        break;
                    //养殖检测->档案管理->新建档案
                    case "20101": temptb.Content = new UcCultureFile(mainWindow.dbOperation);
                        break;
                    //养殖检测->档案管理->档案信息查询
                    case "20102": temptb.Content = new UcQueryCultureFile(mainWindow.dbOperation);
                        break;
                    //养殖检测->检测单管理->新建养殖检测单
                    case "20201": temptb.Content = new UcCultureDetectManager(mainWindow.dbOperation);
                        break;
                    //养殖检测->检测单管理->养殖检测单列表
                    case "20202": temptb.Content = new UcCultureDetectInquire(mainWindow.dbOperation);
                        break;
                    //出证检测->检测单管理->新建检测单
                    case "30101": temptb.Content = new UcDetectBillManager();
                        break;
                    //出证检测->检测单管理->检测单列表
                    case "30102": temptb.Content = new UcDetectBillManager();
                        break;
                    //出证检测->电子出证单->新建电子出证单
                    case "30201": temptb.Content = new UcDetectBillManager();
                        break;
                    //出证检测->电子出证单->出证单列表
                    case "30202": temptb.Content = new UcDetectBillManager();
                        break;
                    //宰前检测->检测单管理->新建检测单
                    case "40101": temptb.Content = new UcDetectBillManager();
                        break;
                    //宰前检测->检测单管理->检测单列表
                    case "40102": temptb.Content = new UcDetectInquire(mainWindow.dbOperation);
                        break;
                    //屠宰同步检测->检测单管理->新建检测单
                    case "50101": temptb.Content = new UcDetectBillManager();
                        break;
                    //屠宰同步检测->检测单管理->检测单列表
                    case "50102": temptb.Content = new UcDetectBillManager();
                        break;
                    //养殖检测->检测单管理->新建档案
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
                    //系统管理->系统管理->执法队伍
                    case "60102": temptb.Content = new UcUserManager(mainWindow.dbOperation);
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
