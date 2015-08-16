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
            Expander[] expanders = new Expander[] { _expander_0, _expander_1, _expander_2, _expander_3, _expander_4, _expander_5, _expander_6 };

            //先让所有控件都可见
            for (int i = 0; i < 7; i++)
            {
                expanders[i].Visibility = Visibility.Visible;
            }
            //再根据二级菜单的个数隐藏部门控件
            for(int i = childMenus.Count; i < 7; i++ )
            {
                expanders[i].Visibility = Visibility.Hidden;
            }
            //加载二、三级菜单
            loadMenu();
        }

        public void loadMenu()
        {
            //定义数组存放：三级菜单的控件
            Grid[] grids = new Grid[] { _grid_0, _grid_1, _grid_2, _grid_3, _grid_4, _grid_5, _grid_6 };
            //定义数组存放：二级菜单的控件
            TextBlock[] texts = new TextBlock[] { _text_0, _text_1, _text_2, _text_3, _text_4, _text_5, _text_6 };
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
                    //插入一行两列
                    grids[i].RowDefinitions.Add(new RowDefinition());
                    grids[i].RowDefinitions[j].Height = new GridLength(35, GridUnitType.Pixel);
                    Grid grid = new Grid();
                    grid.SetValue(Grid.RowProperty, j);
                    grid.SetValue(Grid.ColumnSpanProperty, 2);
                    grid.SetBinding(Grid.BackgroundProperty, new Binding("IsPressed"));
                    grids[i].Children.Add(grid);

                    grids[i].ColumnDefinitions.Add(new ColumnDefinition());
                    grids[i].ColumnDefinitions.Add(new ColumnDefinition());
                    grids[i].ColumnDefinitions[0].Width = new GridLength(65, GridUnitType.Pixel);
                    grids[i].ColumnDefinitions[1].Width = new GridLength(145, GridUnitType.Pixel);
                       
                    //第一列插入图片
                    Image img = new Image();
                    img.Source = new BitmapImage(new Uri("pack://application:,," + "/res/file_2.png"));
                    img.Width = 14;
                    img.Height = 19;
                    Thickness thick = new Thickness(35, 0, 5, 0);
                    img.Margin = thick;
                    img.SetValue(Grid.RowProperty, j);
                    img.SetValue(Grid.ColumnProperty, 0);
                    grids[i].Children.Add(img);

                    //第二列插入button
                    childMenus[i].buttons[j].SetValue(Grid.RowProperty, j);
                    childMenus[i].buttons[j].SetValue(Grid.ColumnProperty, 1);
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
                btn.MinWidth = 120;
                btn.VerticalAlignment = VerticalAlignment.Center;
                btn.Click += new RoutedEventHandler(this.btn_Click);
                buttons.Add(btn);
            }
        }

        private void btn_Click(object sender, System.EventArgs e)
        {
            //(sender as Button).Foreground = Brushes.White;
            int flag_exits = 0;
            foreach (TabItem item in tab.Items)
            {
                if (item.Tag.ToString() == (sender as Button).Tag.ToString())
                {
                    tab.SelectedItem = item;
                    flag_exits = 1;
                    break;
                }
            }
            if (flag_exits == 0)
            {
                int flag = 0;
                temptb = new TabItem();
                temptb.Tag = (sender as Button).Tag.ToString();
                switch ((sender as Button).Tag.ToString())
                {
                    //首页-地图
                    case "10101": temptb.Header = (sender as Button).Content.ToString();
                        temptb.Content = new UcMainPage();
                        flag = 1;
                        break;
                    //首页-报表
                    case "10102": temptb.Header = (sender as Button).Content.ToString();
                        temptb.Content = new UcMainPageTwo();
                        flag = 1;
                        break;
                    //饲料检测->检测单管理->新建饲料检测单
                    case "20101": temptb.Header = (sender as Button).Content.ToString() + "(饲料)";
                        temptb.Content = new UcFeedDetectManager();
                        flag = 1;
                        break;
                    //饲料检测->检测单管理->饲料检测单列表
                    case "20102": temptb.Header = (sender as Button).Content.ToString() + "(饲料)";
                        temptb.Content = new UcFeedDetectInquire(mainWindow.dbOperation);
                        flag = 1;
                        break;
                    //养殖检测->档案管理->新建档案
                    case "30101": temptb.Header = (sender as Button).Content.ToString();
                        temptb.Content = new UcCultureFile(mainWindow.dbOperation);
                        flag = 1;
                        break;
                    //养殖检测->档案管理->档案信息查询
                    case "30102": temptb.Header = (sender as Button).Content.ToString();
                        temptb.Content = new UcQueryCultureFile(mainWindow.dbOperation);
                        flag = 1;
                        break;
                    //养殖检测->档案管理->出栏
                    case "30103": temptb.Header = (sender as Button).Content.ToString();
                        temptb.Content = new UcSoldCultureFile(mainWindow.dbOperation);
                        flag = 1;
                        break;
                    //养殖检测->检测单管理->新建养殖检测单
                    case "30201": temptb.Header = (sender as Button).Content.ToString() + "(养殖)";
                        temptb.Content = new UcCultureDetectManager(mainWindow.dbOperation);
                        flag = 1;
                        break;
                    //养殖检测->检测单管理->养殖检测单列表
                    case "30202": temptb.Header = (sender as Button).Content.ToString() + "(养殖)";
                        temptb.Content = new UcCultureDetectInquire(mainWindow.dbOperation);
                        flag = 1;
                        break;
                    //出证检测->检测单管理->新建出证检测单
                    case "40101": temptb.Header = (sender as Button).Content.ToString() + "(出证)";
                        temptb.Content = new UcCertificateDetectManager(mainWindow.dbOperation);
                        flag = 1;
                        break;
                    //出证检测->检测单管理->出证检测单列表
                    case "40102": temptb.Header = (sender as Button).Content.ToString() + "(出证)";
                        temptb.Content = new UcCertificateInquire(mainWindow.dbOperation);
                        flag = 1;
                        break;
                    //宰前检测->检测单管理->新建宰前检测单
                    case "50101": temptb.Header = (sender as Button).Content.ToString() + "(宰前)";
                        temptb.Content = new UcDetectBillManager();
                        flag = 1;
                        break;
                    //宰前检测->检测单管理->宰前检测单列表
                    case "50102": temptb.Header = (sender as Button).Content.ToString() + "(宰前)";
                        temptb.Content = new UcDetectInquire(mainWindow.dbOperation);
                        flag = 1;
                        break;
                    //屠宰检测->检测单管理->新建屠宰检测单
                    case "60101": temptb.Header = (sender as Button).Content.ToString() + "(屠宰)";
                        temptb.Content = new UcSlaughterDetectManager();
                        flag = 1;
                        break;
                    //屠宰检测->检测单管理->屠宰检测单列表
                    case "60102": temptb.Header = (sender as Button).Content.ToString() + "(屠宰)";
                        temptb.Content = new UcSlaughterDetectInquier(mainWindow.dbOperation);
                        flag = 1;
                        break;
                    //电子出证->电子出证->新建检疫证单(动物)
                    case "70101": temptb.Header = (sender as Button).Content.ToString();
                        temptb.Content = new UcCreateCertificate(mainWindow.dbOperation);
                        flag = 1;
                        break;
                    //电子出证->电子出证->新建检疫证单(产品)
                    case "70102": temptb.Header = (sender as Button).Content.ToString();
                        temptb.Content = new UcCreateCertificate_product(mainWindow.dbOperation);
                        flag = 1;
                        break;
                    //电子出证->电子出证->电子证查询
                    case "70103": temptb.Header = (sender as Button).Content.ToString();
                        temptb.Content = new UcCreateCertificatequery(mainWindow.dbOperation);
                        flag = 1;
                        break;
                    //电子出证->出证数据统计->动物出证
                    case "70301": temptb.Header = (sender as Button).Content.ToString();
                        temptb.Content = new SysCertificateReport(mainWindow.dbOperation, "cz");
                        flag = 1;
                        break;
                    //电子出证->出证数据统计->产品出证
                    case "70302": temptb.Header = (sender as Button).Content.ToString();
                        temptb.Content = new SysCertificateReport(mainWindow.dbOperation, "tz");
                        flag = 1;
                        break;
                    //饲料检测->数据统计->日报表
                    case "20201": temptb.Header = (sender as Button).Content.ToString() + "(饲料)";
                        temptb.Content = new SysDayReport(mainWindow.dbOperation, "yz", "3"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //饲料检测->数据统计->月报表
                    case "20202": temptb.Header = (sender as Button).Content.ToString() + "(饲料)";
                        temptb.Content = new SysMonthReport(mainWindow.dbOperation, "yz", "3"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    ////饲料检测->数据统计->年报表
                    //case "20303": temptb.Header = (sender as Button).Content.ToString() + "(饲料)";
                    //    temptb.Content = new SysYearReport(mainWindow.dbOperation, "yz", "3"); //传入检测站点的类型和检测单的类型
                    //    break;
                    //饲料检测->数据统计->自定义报表
                    case "20203": temptb.Header = (sender as Button).Content.ToString() + "(饲料)";
                        temptb.Content = new SysYearReport(mainWindow.dbOperation, "yz", "3"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //养殖检测->数据统计->日报表
                    case "30301": temptb.Header = (sender as Button).Content.ToString() + "(养殖)";
                        temptb.Content = new SysDayReport(mainWindow.dbOperation,"yz","0"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //养殖检测->数据统计->月报表
                    case "30302": temptb.Header = (sender as Button).Content.ToString() + "(养殖)";
                        temptb.Content = new SysMonthReport(mainWindow.dbOperation, "yz", "0"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    ////养殖检测->数据统计->年报表
                    //case "20303": temptb.Header = (sender as Button).Content.ToString() + "(养殖)";
                    //    temptb.Content = new SysYearReport(mainWindow.dbOperation, "yz", "0"); //传入检测站点的类型和检测单的类型
                    //    break;
                    //养殖检测->数据统计->自定义报表
                    case "30303": temptb.Header = (sender as Button).Content.ToString() + "(养殖)";
                        temptb.Content = new SysYearReport(mainWindow.dbOperation, "yz", "0"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //养殖检测->数据统计->档案统计报表
                    case "30304": temptb.Header = (sender as Button).Content.ToString();
                        temptb.Content = new SysCultureFileReport(mainWindow.dbOperation); 
                        flag = 1;
                        break;
                    //出证检测->数据统计->日报表
                    case "40201": temptb.Header = (sender as Button).Content.ToString() + "(出证)";
                        temptb.Content = new SysDayReport(mainWindow.dbOperation, "cz", "1"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //出证检测->数据统计->月报表
                    case "40202": temptb.Header = (sender as Button).Content.ToString() + "(出证)";
                        temptb.Content = new SysMonthReport(mainWindow.dbOperation, "cz", "1"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    ////出证检测->数据统计->年报表
                    //case "30303": temptb.Header = (sender as Button).Content.ToString() + "(出证)";
                    //    temptb.Content = new SysYearReport(mainWindow.dbOperation, "cz", "1"); //传入检测站点的类型和检测单的类型
                    //    break;
                    //出证检测->数据统计->自定义报表
                    case "40203": temptb.Header = (sender as Button).Content.ToString() + "(出证)";
                        temptb.Content = new SysYearReport(mainWindow.dbOperation, "cz", "1"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //宰前检测->数据统计->日报表
                    case "50201": temptb.Header = (sender as Button).Content.ToString() + "(宰前)";
                        temptb.Content = new SysDayReport(mainWindow.dbOperation, "tz", ""); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //宰前检测->数据统计->月报表
                    case "50202": temptb.Header = (sender as Button).Content.ToString() + "(宰前)";
                        temptb.Content = new SysMonthReport(mainWindow.dbOperation, "tz", ""); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    ////宰前检测->数据统计->年报表
                    //case "40203": temptb.Header = (sender as Button).Content.ToString() + "(宰前)";
                    //    temptb.Content = new SysYearReport(mainWindow.dbOperation, "tz", ""); //传入检测站点的类型和检测单的类型
                    //    break;
                    //宰前检测->数据统计->自定义报表
                    case "50203": temptb.Header = (sender as Button).Content.ToString() + "(宰前)";
                        temptb.Content = new SysYearReport(mainWindow.dbOperation, "tz", ""); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //屠宰检测->数据统计->日报表
                    case "60201": temptb.Header = (sender as Button).Content.ToString() + "(屠宰)";
                        temptb.Content = new SysDayReport(mainWindow.dbOperation, "tz", "2"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //屠宰检测->数据统计->月报表
                    case "60202": temptb.Header = (sender as Button).Content.ToString() + "(屠宰)";
                        temptb.Content = new SysMonthReport(mainWindow.dbOperation, "tz", "2"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    ////屠宰检测->数据统计->年报表
                    //case "50203": temptb.Header = (sender as Button).Content.ToString() + "(屠宰)";
                    //    temptb.Content = new SysYearReport(mainWindow.dbOperation, "tz", "2"); //传入检测站点的类型和检测单的类型
                    //    break;
                    //屠宰检测->数据统计->自定义报表
                    case "60203": temptb.Header = (sender as Button).Content.ToString() + "(屠宰)";
                        temptb.Content = new SysYearReport(mainWindow.dbOperation, "tz", "2"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //饲料检测->数据分析->对比分析
                    case "20301": temptb.Header = (sender as Button).Content.ToString() + "(饲料)";
                        temptb.Content = new SysComparisonAndAnalysis(mainWindow.dbOperation, "yz", "3"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //饲料检测->数据分析->趋势分析
                    case "20302": temptb.Header = (sender as Button).Content.ToString() + "(饲料)";
                        temptb.Content = new SysTrendAnalysis(mainWindow.dbOperation, "yz", "3"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //养殖检测->数据分析->对比分析
                    case "30401": temptb.Header = (sender as Button).Content.ToString() + "(养殖)";
                        temptb.Content = new SysComparisonAndAnalysis(mainWindow.dbOperation, "yz", "0"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //养殖检测->数据分析->趋势分析
                    case "30402": temptb.Header = (sender as Button).Content.ToString() + "(养殖)";
                        temptb.Content = new SysTrendAnalysis(mainWindow.dbOperation, "yz", "0"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //养殖检测->数据分析->区域分析
                    case "30403": temptb.Header = (sender as Button).Content.ToString() + "(养殖)";
                        temptb.Content = new SysAreaAnalysis(mainWindow.dbOperation, "yz", "0"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //养殖检测->数据分析->抽检率分析
                    case "30404": temptb.Header = (sender as Button).Content.ToString() + "(养殖)";
                        temptb.Content = new SysAreaAnalysis(mainWindow.dbOperation, "yz", "0"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //出证检测->数据分析->对比分析
                    case "40301": temptb.Header = (sender as Button).Content.ToString() + "(出证)";
                        temptb.Content = new SysComparisonAndAnalysis(mainWindow.dbOperation, "cz", "1"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //出证检测->数据分析->趋势分析
                    case "40302": temptb.Header = (sender as Button).Content.ToString() + "(出证)";
                        temptb.Content = new SysTrendAnalysis(mainWindow.dbOperation, "cz", "1"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //出证检测->数据分析->区域分析
                    case "40303": temptb.Header = (sender as Button).Content.ToString() + "(出证)";
                        temptb.Content = new SysAreaAnalysis(mainWindow.dbOperation, "cz", "1"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //出证检测->数据分析->抽检率分析
                    case "40304": temptb.Header = (sender as Button).Content.ToString() + "(出证)";
                        temptb.Content = new SysAreaAnalysis(mainWindow.dbOperation, "cz", "1"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //宰前检测->数据分析->对比分析
                    case "50301": temptb.Header = (sender as Button).Content.ToString() + "(宰前)";
                        temptb.Content = new SysComparisonAndAnalysis(mainWindow.dbOperation, "tz", ""); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //宰前检测->数据分析->趋势分析
                    case "50302": temptb.Header = (sender as Button).Content.ToString() + "(宰前)";
                        temptb.Content = new SysTrendAnalysis(mainWindow.dbOperation, "tz", ""); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //宰前检测->数据分析->区域分析
                    case "50303": temptb.Header = (sender as Button).Content.ToString() + "(宰前)";
                        temptb.Content = new SysAreaAnalysis(mainWindow.dbOperation, "tz", ""); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //宰前检测->数据分析->抽检率分析
                    case "50304": temptb.Header = (sender as Button).Content.ToString() + "(宰前)";
                        temptb.Content = new SysAreaAnalysis(mainWindow.dbOperation, "tz", ""); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //屠宰检测->数据分析->对比分析
                    case "60301": temptb.Header = (sender as Button).Content.ToString() + "(屠宰)";
                        temptb.Content = new SysComparisonAndAnalysis(mainWindow.dbOperation, "tz", "2"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //屠宰检测->数据分析->趋势分析
                    case "60302": temptb.Header = (sender as Button).Content.ToString() + "(屠宰)";
                        temptb.Content = new SysTrendAnalysis(mainWindow.dbOperation, "tz", "2"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //屠宰检测->数据分析->区域分析
                    case "60303": temptb.Header = (sender as Button).Content.ToString() + "(屠宰)";
                        temptb.Content = new SysAreaAnalysis(mainWindow.dbOperation, "tz", "2"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //养殖检测->检测任务->任务分配
                    case "30501": temptb.Header = (sender as Button).Content.ToString() + "(养殖)";
                        temptb.Content = new UcSetSamplingRate(mainWindow.dbOperation,"yz","0"); //传入检测站点的类型
                        flag = 1;
                        break;
                    //养殖检测->检测任务->任务考评
                    case "30502": temptb.Header = (sender as Button).Content.ToString() + "(养殖)";
                        temptb.Content = new SysTaskCheck(mainWindow.dbOperation,"yz", "0"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //养殖检测->检测任务->抽检率设置
                    case "30503": temptb.Header = (sender as Button).Content.ToString() + "(养殖)";
                        temptb.Content = new SysTaskCheck(mainWindow.dbOperation, "yz", "0"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //出证检测->检测任务->任务分配
                    case "40401": temptb.Header = (sender as Button).Content.ToString() + "(出证)";
                        temptb.Content = new UcSetSamplingRate(mainWindow.dbOperation,"cz","1"); //传入检测站点的类型
                        flag = 1;
                        break;
                    //出证检测->检测任务->任务考评
                    case "40402": temptb.Header = (sender as Button).Content.ToString() + "(出证)";
                        temptb.Content = new SysTaskCheck(mainWindow.dbOperation,"cz", "1"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //出证检测->检测任务->抽检率设置
                    case "40403": temptb.Header = (sender as Button).Content.ToString() + "(出证)";
                        temptb.Content = new SysTaskCheck(mainWindow.dbOperation, "cz", "1"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //宰前检测->检测任务->任务分配
                    case "50401": temptb.Header = (sender as Button).Content.ToString() + "(宰前)";
                        temptb.Content = new UcSetSamplingRate(mainWindow.dbOperation,"tz",""); //传入检测站点的类型
                        flag = 1;
                        break;
                    //宰前检测->检测任务->任务考评
                    case "50402": temptb.Header = (sender as Button).Content.ToString() + "(宰前)";
                        temptb.Content = new SysTaskCheck(mainWindow.dbOperation,"tz", ""); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //宰前检测->检测任务->抽检率设置
                    case "50403": temptb.Header = (sender as Button).Content.ToString() + "(宰前)";
                        temptb.Content = new SysTaskCheck(mainWindow.dbOperation, "tz", ""); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    ////屠宰检测->检测任务->任务分配
                    //case "50401": temptb.Header = (sender as Button).Content.ToString() + "(屠宰)";
                    //    temptb.Content = new UcTaskAllocation("tz"); //传入检测站点的类型
                    //    break;
                    ////屠宰检测->检测任务->任务考评
                    //case "50402": temptb.Header = (sender as Button).Content.ToString() + "(屠宰)";
                    //    temptb.Content = new SysTaskCheck(mainWindow.dbOperation,"tz", "2"); //传入检测站点的类型和检测单的类型
                    //    break;
                    //饲料检测->风险预警->实时风险
                    case "90101": temptb.Header = (sender as Button).Content.ToString() + "(饲料)";
                        temptb.Content = new SysWarningInfo(mainWindow.dbOperation, "yz", "3"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //饲料检测->风险预警->预警复核
                    case "90102": temptb.Header = (sender as Button).Content.ToString() + "(饲料)";
                        temptb.Content = new SysReviewInfo(mainWindow.dbOperation, "yz", "3"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //饲料检测->风险预警->复核日志
                    case "90103": temptb.Header = (sender as Button).Content.ToString() + "(饲料)";
                        temptb.Content = new SysReviewLog(mainWindow.dbOperation, "yz", "3"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //养殖检测->风险预警->实时风险
                    case "90201": temptb.Header = (sender as Button).Content.ToString() + "(养殖)";
                        temptb.Content = new SysWarningInfo(mainWindow.dbOperation,"yz","0"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //养殖检测->风险预警->预警复核
                    case "90202": temptb.Header = (sender as Button).Content.ToString() + "(养殖)";
                        temptb.Content = new SysReviewInfo(mainWindow.dbOperation, "yz", "0"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //养殖检测->风险预警->复核日志
                    case "90203": temptb.Header = (sender as Button).Content.ToString() + "(养殖)";
                        temptb.Content = new SysReviewLog(mainWindow.dbOperation, "yz", "0"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //出证检测->风险预警->实时风险
                    case "90301": temptb.Header = (sender as Button).Content.ToString() + "(出证)";
                        temptb.Content = new SysWarningInfo(mainWindow.dbOperation, "cz", "1"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //出证检测->风险预警->预警复核
                    case "90302": temptb.Header = (sender as Button).Content.ToString() + "(出证)";
                        temptb.Content = new SysReviewInfo(mainWindow.dbOperation, "cz", "1"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //出证检测->风险预警->复核日志
                    case "90303": temptb.Header = (sender as Button).Content.ToString() + "(出证)";
                        temptb.Content = new SysReviewLog(mainWindow.dbOperation, "cz", "1"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break; 
                    //宰前检测->风险预警->实时风险
                    case "90401": temptb.Header = (sender as Button).Content.ToString() + "(宰前)";
                        temptb.Content = new SysWarningInfo(mainWindow.dbOperation, "tz", ""); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //宰前检测->风险预警->预警复核
                    case "90402": temptb.Header = (sender as Button).Content.ToString() + "(宰前)";
                        temptb.Content = new SysReviewInfo(mainWindow.dbOperation, "tz", ""); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //宰前检测->风险预警->复核日志
                    case "90403": temptb.Header = (sender as Button).Content.ToString() + "(宰前)";
                        temptb.Content = new SysReviewLog(mainWindow.dbOperation, "tz", ""); //传入检测站点的类型和检测单的类型
                        break;
                    //屠宰检测->风险预警->实时风险
                    case "90501": temptb.Header = (sender as Button).Content.ToString() + "(屠宰)";
                        temptb.Content = new SysWarningInfo(mainWindow.dbOperation, "tz", "2"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //屠宰检测->风险预警->预警复核
                    case "90502": temptb.Header = (sender as Button).Content.ToString() + "(屠宰)";
                        temptb.Content = new SysReviewInfo(mainWindow.dbOperation, "tz", "2"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //屠宰检测->风险预警->复核日志
                    case "90503": temptb.Header = (sender as Button).Content.ToString() + "(屠宰)";
                        temptb.Content = new SysReviewLog(mainWindow.dbOperation, "tz", "2"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    ////养殖检测->风险预警->预警数据统计
                    //case "20604": temptb.Header = (sender as Button).Content.ToString() + "(养殖)";
                    //    temptb.Content = new SysReviewLog(mainWindow.dbOperation, "yz", "0"); //传入检测站点的类型和检测单的类型
                    //    break;
                    ////出证检测->风险预警->预警数据统计
                    //case "30604": temptb.Header = (sender as Button).Content.ToString() + "(出证)";
                    //    temptb.Content = new SysReviewLog(mainWindow.dbOperation, "cz", "1"); //传入检测站点的类型和检测单的类型
                    //    break;
                    ////宰前检测->风险预警->预警数据统计
                    //case "40504": temptb.Header = (sender as Button).Content.ToString() + "(宰前)";
                    //    temptb.Content = new SysReviewLog(mainWindow.dbOperation, "tz", ""); //传入检测站点的类型和检测单的类型
                    //    break;
                    ////屠宰检测->风险预警->预警数据统计
                    //case "50504": temptb.Header = (sender as Button).Content.ToString() + "(屠宰)";
                    //    temptb.Content = new SysReviewLog(mainWindow.dbOperation, "tz", "2"); //传入检测站点的类型和检测单的类型
                    //    break;
                    //风险预警->预警数据统计
                    case "90601": temptb.Header = (sender as Button).Content.ToString();
                        temptb.Content = new SysWarningReport(mainWindow.dbOperation); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //风险预警->预警数据分析
                    case "90701": temptb.Header = (sender as Button).Content.ToString();
                        temptb.Content = new SysWarningReport(mainWindow.dbOperation); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //数据统计->检测数据统计->瘦肉精检测
                    case "F0201": temptb.Header = (sender as Button).Content.ToString();
                        temptb.Content = new SysBigReport(mainWindow.dbOperation,"cz"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //数据统计->检测数据统计->抗生素检测
                    case "F0202": temptb.Header = (sender as Button).Content.ToString();
                        temptb.Content = new SysBigReport(mainWindow.dbOperation, "yz"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //数据统计->检测数据统计->寄生虫检测
                    case "F0203": temptb.Header = (sender as Button).Content.ToString();
                        temptb.Content = new SysBigReport(mainWindow.dbOperation, "tz"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //数据统计->检测数据统计->疫病检测
                    case "F0204": temptb.Header = (sender as Button).Content.ToString();
                        temptb.Content = new SysBigReport(mainWindow.dbOperation, "yz"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;

                    //大数据分析->养殖环节->检测量分析
                    case "80101": temptb.Header = (sender as Button).Content.ToString() + "(养殖)";
                        temptb.Content = new SysAnalysis(mainWindow.dbOperation,"1","yz"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //大数据分析->养殖环节->执行力分析
                    case "80102": temptb.Header = (sender as Button).Content.ToString() + "(养殖)";
                        temptb.Content = new SysAnalysis(mainWindow.dbOperation,"2","yz"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //大数据分析->养殖环节->抽检率分析
                    case "80103": temptb.Header = (sender as Button).Content.ToString() + "(养殖)";
                        temptb.Content = new SysAnalysis(mainWindow.dbOperation,"3","yz"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //大数据分析->养殖环节->风险评估分析
                    case "80104": temptb.Header = (sender as Button).Content.ToString() + "(养殖)";
                        temptb.Content = new SysAnalysis(mainWindow.dbOperation,"4","yz"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;

                    //大数据分析->出证环节->检测量分析
                    case "80201": temptb.Header = (sender as Button).Content.ToString() + "(出证)";
                        temptb.Content = new SysAnalysis(mainWindow.dbOperation,"1","cz"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //大数据分析->出证环节->执行力分析
                    case "80202": temptb.Header = (sender as Button).Content.ToString() + "(出证)";
                        temptb.Content = new SysAnalysis(mainWindow.dbOperation,"2","cz"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //大数据分析->出证环节->抽检率分析
                    case "80203": temptb.Header = (sender as Button).Content.ToString() + "(出证)";
                        temptb.Content = new SysAnalysis(mainWindow.dbOperation,"3","cz"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //大数据分析->出证环节->风险评估分析
                    case "80204": temptb.Header = (sender as Button).Content.ToString() + "(出证)";
                        temptb.Content = new SysAnalysis(mainWindow.dbOperation,"4","cz"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;

                    //大数据分析->宰前环节->检测量分析
                    case "80301": temptb.Header = (sender as Button).Content.ToString() + "(宰前)";
                        temptb.Content = new SysAnalysis(mainWindow.dbOperation,"1","tz"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //大数据分析->宰前环节->执行力分析
                    case "80302": temptb.Header = (sender as Button).Content.ToString() + "(宰前)";
                        temptb.Content = new SysAnalysis(mainWindow.dbOperation,"2","tz"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //大数据分析->宰前环节->抽检率分析
                    case "80303": temptb.Header = (sender as Button).Content.ToString() + "(宰前)";
                        temptb.Content = new SysAnalysis(mainWindow.dbOperation,"3","tz"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //大数据分析->宰前环节->风险评估分析
                    case "80304": temptb.Header = (sender as Button).Content.ToString() + "(宰前)";
                        temptb.Content = new SysAnalysis(mainWindow.dbOperation,"4","tz"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;

                    //大数据分析->屠宰环节->检测量分析
                    case "80401": temptb.Header = (sender as Button).Content.ToString() + "(屠宰)";
                        temptb.Content = new SysAnalysis(mainWindow.dbOperation,"1","tz"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //大数据分析->屠宰环节->执行力分析
                    case "80402": temptb.Header = (sender as Button).Content.ToString() + "(屠宰)";
                        temptb.Content = new SysAnalysis(mainWindow.dbOperation,"2","tz"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //大数据分析->屠宰环节->抽检率分析
                    case "80403": temptb.Header = (sender as Button).Content.ToString() + "(屠宰)";
                        temptb.Content = new SysAnalysis(mainWindow.dbOperation,"3","tz"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //大数据分析->屠宰环节->风险评估分析
                    case "80404": temptb.Header = (sender as Button).Content.ToString() + "(屠宰)";
                        temptb.Content = new SysAnalysis(mainWindow.dbOperation,"4","tz"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;

                    //大数据分析->专项大数据分析->瘦肉精风险指数评估
                    case "80501": temptb.Header = (sender as Button).Content.ToString();
                        temptb.Content = new SysSrjGrade(mainWindow.dbOperation); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;

                    //大数据分析->专项大数据分析->抗生素风险指数评估
                    case "80502": temptb.Header = (sender as Button).Content.ToString();
                        temptb.Content = new SysSrjGrade(mainWindow.dbOperation); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //大数据分析->专项大数据分析->地市执行力评级
                    case "80503": temptb.Header = (sender as Button).Content.ToString();
                        temptb.Content = new SysCityGrade(mainWindow.dbOperation); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //大数据分析->专项大数据分析->检测师评级
                    case "80504": temptb.Header = (sender as Button).Content.ToString();
                        temptb.Content = new SysDetectUserGrade(mainWindow.dbOperation,"cz"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;

                    //大数据分析->屠宰厂评级分析->屠宰规模评级
                    case "80701": temptb.Header = (sender as Button).Content.ToString();
                        temptb.Content = new SysTzGrade(mainWindow.dbOperation,"tz"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;

                    //大数据分析->检测卡质量评估->质量评估分析
                    case "80801": temptb.Header = (sender as Button).Content.ToString();
                        temptb.Content = new SysCardGrade(mainWindow.dbOperation); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;

                    //屠宰溯源->屠宰企业管理->屠宰企业档案信息
                    case "B0101": temptb.Header = (sender as Button).Content.ToString() ;
                        temptb.Content = new SysTzCompanyInfo(mainWindow.dbOperation); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;

                    //屠宰溯源->屠宰溯源码管理->屠宰溯源码分配
                    //case "B0201": temptb.Header = (sender as Button).Content.ToString() ;
                    //    temptb.Content = new SysAnalysis(mainWindow.dbOperation); //传入检测站点的类型和检测单的类型
                    //    flag = 1;
                    //    break;
                    //屠宰溯源->溯源查询->查询
                    case "B0301": temptb.Header = (sender as Button).Content.ToString() ;
                        temptb.Content = new SysRetroactiveQuery(); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;
                    //屠宰溯源->屠宰溯源码统计->使用量统计
                    case "B0401": temptb.Header = (sender as Button).Content.ToString() ;
                        temptb.Content = new SysCertificateReport(mainWindow.dbOperation,"tz"); //传入检测站点的类型和检测单的类型
                        flag = 1;
                        break;


                    //检测师管理->签到记录管理->签到记录查询
                    case "C0101": temptb.Header = (sender as Button).Content.ToString();
                        temptb.Content = new SysDetectUserManager(mainWindow.dbOperation);
                        flag = 1;
                        break;
                    //系统管理->系统管理->部门管理
                    case "D0101": temptb.Header = (sender as Button).Content.ToString();
                        temptb.Content = new SysDeptManager(mainWindow.dbOperation);
                        flag = 1;
                        break;
                    //系统管理->系统管理->执法队伍
                    case "D0102": temptb.Header = (sender as Button).Content.ToString();
                        temptb.Content = new UcUserManager(mainWindow.dbOperation);
                        flag = 1;
                        break;
                    //系统管理->系统管理->修改密码
                    case "D0104": temptb.Header = (sender as Button).Content.ToString();
                        temptb.Content = new SysModifyPassword();
                        flag = 1;
                        break;
                    //系统管理->系统管理->图片上传
                    case "D0105": temptb.Header = (sender as Button).Content.ToString();
                        temptb.Content = new SysLoadPicture(mainWindow.dbOperation);
                        flag = 1;
                        break;
                    //系统管理->系统管理->系统日志
                    case "D0106": temptb.Header = (sender as Button).Content.ToString();
                        temptb.Content = new SysLogManager();
                        flag = 1;
                        break;
                    //系统管理->系统管理->权限管理
                    case "D0107": temptb.Header = (sender as Button).Content.ToString();
                        temptb.Content = new SysRolePowerManager();
                        flag = 1;
                        break;
                    //帮助->帮助->帮助
                    case "E0101": temptb.Header = (sender as Button).Content.ToString();
                        temptb.Content = new UcUnrealizedModul();
                        flag = 1;
                        break;
                    //帮助->帮助->关于
                    case "E0102": temptb.Header = (sender as Button).Content.ToString();
                        temptb.Content = new SysHelp();
                        flag = 1;
                        break;
                    //case "角色管理": temptb.Content = new SysRoleManager();
                    //    break;
                    //case "权限管理": temptb.Content = new SysRolePowerManager();
                    //    break;
                    //case "用户管理": temptb.Content = new SysUserManager();
                    //    break;
                    default: break;
                }
                if (flag == 1)
                {
                    tab.Items.Add(temptb);
                    tab.SelectedIndex = tab.Items.Count - 1;
                }
                
            }

            //mainWindow.IsEnbleMouseEnterLeave = false;
            //if (uc is IClickChildMenuInitUserControlUI)
            //{
            //    ((IClickChildMenuInitUserControlUI)uc).InitUserControlUI();
            //}
        }
    }
}
