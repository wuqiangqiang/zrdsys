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
using Toolkit = Microsoft.Windows.Controls; 
using FoodSafetyMonitoring.Manager;
using System.Data;
using FoodSafetyMonitoring.dao;

namespace FoodSafetyMonitoring
{
    /// <summary>
    /// MainMenu.xaml 的交互逻辑
    /// </summary>
    public partial class MainMenu : UserControl
    { 
        MainWindow mainWindow;
        private List<MainMenuItem> mainMenus = new List<MainMenuItem>(); 

        public MainMenu(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            InitMenu();

            List<string> limitedMenus = (Application.Current.Resources["User"] as UserInfo).Menus;
            //limitedMenus.Add("用户管理");
            //limitedMenus.Add("权限管理");
            //limitedMenus.Add("日志管理");
            //limitedMenus.Add("角色设置");
            //limitedMenus.Add("字典设置");
            //limitedMenus.Add("部门设置");
            //limitedMenus.Add("人工信息录入");
            //limitedMenus.Add("省城市维护");
            //limitedMenus.Add("系统床位设置");
            //limitedMenus.Add("趋势分析");
            //limitedMenus.Add("消息中心");

            //limitedMenus.Add("检测单维护");
            //limitedMenus.Add("数据统计报表");
            //limitedMenus.Add("数据管理"); 
 
            //foreach (MainMenuItem mainMenu in mainMenus)
            //{
            //    int count = 0;
            //    foreach (MyChildMenu childMenu in mainMenu.childMenus)
            //    {
            //        if (!limitedMenus.Contains(childMenu.name))
            //        {
            //            childMenu.img.IsEnabled = false;
            //        }
            //        else
            //        {
            //            count++;
            //        }
            //    }
            //    if (count == 0)
            //    {
            //        mainMenu.img.IsEnabled = false;
            //    }
            //}
        }

        public void InitMenu()
        {
        //    //主菜单个数8
        //    for (int i = 0; i < 8; i++)
        //    {
        //        _menu.ColumnDefinitions.Add(new ColumnDefinition());
        //    }

        //    List<Image> mainMenuImages = new List<Image>();

        //    for (int i = 0; i < 8; i++)
        //    {
        //        Image image = new Image();
        //        image.SetValue(Grid.RowProperty, 1);
        //        image.SetValue(Grid.ColumnProperty, i);
        //        image.Source = new BitmapImage(new Uri(@"pack://application:,,/res/firstpage_normal.gif"));
        //        mainMenuImages.Add(image);
        //        _menu.Children.Add(image);
        //    }

        //    List<MyChildMenu> childMenus = new List<MyChildMenu>();
        //    mainMenus.Add(new MainMenuItem("首页", mainMenuImages[0], "/res/firstpage_select.gif", "/res/firstpage_normal.gif", childMenus, mainWindow));

        //    childMenus = new List<MyChildMenu>();
        //    mainMenus.Add(new MainMenuItem("新建检测单", mainMenuImages[1], "/res/detect_select.gif", "/res/detect_normal.gif", childMenus, mainWindow));

        //    childMenus = new List<MyChildMenu>();
        //    childMenus.Add(new MyChildMenu("日报表", "/res/report_select.gif", "/res/report_normal.gif", new SysDayReport(mainWindow.dbOperation), mainWindow));
            //childMenus.Add(new MyChildMenu("月报表", "/res/report_select.gif", "/res/report_normal.gif", new SysMonthReport(mainWindow.dbOperation), mainWindow));
        //    childMenus.Add(new MyChildMenu("年报表", "/res/report_select.gif", "/res/report_normal.gif", new SysYearReport(mainWindow.dbOperation), mainWindow));
        //    childMenus.Add(new MyChildMenu("自定义报表", "/res/report_select.gif", "/res/report_normal.gif", new SysDesignReport(mainWindow.dbOperation), mainWindow));
        //    mainMenus.Add(new MainMenuItem("统计报表", mainMenuImages[2], "/res/report_select.gif", "/res/report_normal.gif", childMenus, mainWindow));

        //    childMenus = new List<MyChildMenu>();
        //    childMenus.Add(new MyChildMenu("对比分析", "/res/report_select.gif", "/res/report_normal.gif", new SysComparisonAndAnalysis(mainWindow.dbOperation), mainWindow));
        //    childMenus.Add(new MyChildMenu("趋势分析", "/res/report_select.gif", "/res/report_normal.gif", new SysTrendAnalysis(mainWindow.dbOperation), mainWindow));
        //    childMenus.Add(new MyChildMenu("区域分析", "/res/report_select.gif", "/res/report_normal.gif", new SysAreaAnalysis(mainWindow.dbOperation), mainWindow));
        //    mainMenus.Add(new MainMenuItem("统计分析", mainMenuImages[3], "/res/analysis_select.gif", "/res/analysis_normal.gif", childMenus, mainWindow));

        //    childMenus = new List<MyChildMenu>();
        //    childMenus.Add(new MyChildMenu("任务分配", "/res/report_select.gif", "/res/report_normal.gif", new UcTaskAllocation(), mainWindow));
        //    childMenus.Add(new MyChildMenu("任务考评", "/res/report_select.gif", "/res/report_normal.gif", new SysTaskCheck(mainWindow.dbOperation), mainWindow));
        //    mainMenus.Add(new MainMenuItem("检测任务", mainMenuImages[4], "/res/task_select.gif", "/res/task_normal.gif", childMenus, mainWindow));

        //    childMenus = new List<MyChildMenu>();
        //    childMenus.Add(new MyChildMenu("实时风险", "/res/report_select.gif", "/res/report_normal.gif", new SysWarningInfo(mainWindow.dbOperation), mainWindow));
        //    childMenus.Add(new MyChildMenu("预警复核", "/res/report_select.gif", "/res/report_normal.gif", new SysReviewInfo(mainWindow.dbOperation), mainWindow));
        //    mainMenus.Add(new MainMenuItem("风险预警", mainMenuImages[5], "/res/warning_select.gif", "/res/warning_normal.gif", childMenus, mainWindow));

        //    childMenus = new List<MyChildMenu>();
        //    childMenus.Add(new MyChildMenu("部门管理", "/res/report_select.gif", "/res/report_normal.gif", new SysDeptManager(mainWindow.dbOperation), mainWindow));
        //    childMenus.Add(new MyChildMenu("角色管理", "/res/report_select.gif", "/res/report_normal.gif", new SysRoleManager(), mainWindow));
        //    childMenus.Add(new MyChildMenu("权限管理", "/res/report_select.gif", "/res/report_normal.gif", new SysRolePowerManager(), mainWindow));
        //    childMenus.Add(new MyChildMenu("用户管理", "/res/report_select.gif", "/res/report_normal.gif", new SysUserManager(), mainWindow));
        //    mainMenus.Add(new MainMenuItem("系统管理", mainMenuImages[6], "/res/system_select.gif", "/res/system_normal.gif", childMenus, mainWindow));

        //    childMenus = new List<MyChildMenu>();
        //    childMenus.Add(new MyChildMenu("帮助", "/res/report_select.gif", "/res/report_normal.gif", new UcUnrealizedModul(), mainWindow));
        //    childMenus.Add(new MyChildMenu("关于", "/res/report_select.gif", "/res/report_normal.gif", new UcUnrealizedModul(), mainWindow));
        //    mainMenus.Add(new MainMenuItem("帮助", mainMenuImages[7], "/res/help_select.gif", "/res/help_normal.jpg", childMenus, mainWindow));

        //    mainMenus[2].LoadChildMenu();
        }

        private void CreateMenu(DataTable dt, string mainMenu)
        {

        }
    }


    //public class MainMenuItem
    //{
    //    public string Name;
    //    public BitmapImage img_mouseEnter;
    //    public BitmapImage img_mouseLeave;
    //    public List<MyChildMenu> childMenus;
    //    public ChildMenu childMenu;
    //    public Image img;
    //    public Grid grid_MenuOrComponent;
    //    private MainWindow mainWindow;

    //    public MainMenuItem(string name, Image img, string mouseEnterBackImgPath, string mouseLeaveBackImgPath, List<MyChildMenu> childMenus, MainWindow mainWindow)
    //    {
    //        this.Name = name;
    //        this.childMenus = childMenus;
    //        this.mainWindow = mainWindow;
    //        grid_MenuOrComponent = mainWindow.grid_Menu;
    //        this.childMenu = new ChildMenu(childMenus);
    //        this.img = img;
    //        this.img.Tag = name;
    //        img_mouseEnter = new BitmapImage(new Uri("pack://application:,," + mouseEnterBackImgPath));
    //        img_mouseLeave = new BitmapImage(new Uri("pack://application:,," + mouseLeaveBackImgPath));
    //        this.img.Source = img_mouseLeave;

    //        this.img.MouseDown += new MouseButtonEventHandler(img_MouseDown);
    //        this.img.MouseEnter += new MouseEventHandler(img_MouseEnter);
    //        this.img.MouseLeave += new MouseEventHandler(img_MouseLeave);

    //    }


    //    void img_MouseDown(object sender, MouseButtonEventArgs e)
    //    {
    //        if (childMenus.Count == 0)
    //        {
    //            Toolkit.MessageBox.Show("无权限!!!");
    //            return;
    //        }
    //        mainWindow.IsEnbleMouseEnterLeave = true;
    //        for (int i = 0; i < grid_MenuOrComponent.Children.Count; i++)
    //        {
    //            if (grid_MenuOrComponent.Children[i] is UserControl)
    //            {
    //                if (grid_MenuOrComponent.Children[i] is UserControlTemplet)
    //                {
    //                    ((UserControlTemplet)grid_MenuOrComponent.Children[0]).grid.Children.RemoveAt(0);
    //                }
    //                grid_MenuOrComponent.Children.RemoveAt(i);
    //                i--;
    //            }
    //        }
    //        this.grid_MenuOrComponent.Children.Add(childMenu);
    //    }

    //    void img_MouseLeave(object sender, MouseEventArgs e)
    //    {
    //        ((Image)sender).Source = img_mouseLeave;
    //    }

    //    public void LoadChildMenu()
    //    {
    //        if (mainWindow.IsEnbleMouseEnterLeave)
    //        {
    //            for (int i = 0; i < grid_MenuOrComponent.Children.Count; i++)
    //            {
    //                if (grid_MenuOrComponent.Children[i] is UserControl)
    //                {
    //                    if (grid_MenuOrComponent.Children[i] is UserControlTemplet)
    //                    {
    //                        ((UserControlTemplet)grid_MenuOrComponent.Children[0]).grid.Children.RemoveAt(0);
    //                    }
    //                    grid_MenuOrComponent.Children.RemoveAt(i);
    //                    i--;
    //                }
    //            }
    //            this.grid_MenuOrComponent.Children.Add(childMenu);
    //        }
    //        img.Source = img_mouseEnter;
    //    }

    //    void img_MouseEnter(object sender, MouseEventArgs e)
    //    {
    //        LoadChildMenu();
    //    }

    //}
}
