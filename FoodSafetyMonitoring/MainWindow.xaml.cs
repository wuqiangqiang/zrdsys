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
using System.Windows.Controls.Primitives;
using System.Runtime.InteropServices;
using DBUtility;
using FoodSafetyMonitoring.Manager;
using FoodSafetyMonitoring.dao;
using Toolkit = Microsoft.Windows.Controls;
using System.Data;
using System.IO;
using System.Configuration;


namespace FoodSafetyMonitoring
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        //private DispatcherTimer timer;
        public delegate void UserControlCloseEventHandler();
        //public bool IsEnbleMouseEnterLeave = true;
        private string userName;
        public IDBOperation dbOperation = null;
        public List<MainMenuItem> mainMenus = new List<MainMenuItem>();
        private Rect rcnormal;//定义一个全局rect记录还原状态下窗口的位置和大小。


        public MainWindow(IDBOperation dbOperation)
        {
            //Rect rc = SystemParameters.WorkArea;//获取工作区大小
            this.Width = 1366;
            this.Height = 766;
            InitializeComponent();
            this.dbOperation = dbOperation;
            UserInfo userInfo = Application.Current.Resources["User"] as UserInfo;
            this.userName = userInfo.ShowName;
            Application.Current.Resources.Add("省市表", dbOperation.GetProvinceCity());

            //加载标题
            this._user.Text = this.userName;
            //this._date.Text = DateTime.Now.ToLongDateString().ToString() +  DateTime.Now.ToString("dddd");
            this._date.Text = DateTime.Now.ToLongDateString().ToString();

            DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("select companyName,phone from t_supplier where supplierId ='{0}'", (Application.Current.Resources["User"] as UserInfo).SupplierId == "" ? "zrd" : (Application.Current.Resources["User"] as UserInfo).SupplierId)).Tables[0];
            this._bottom.Text = table.Rows[0][0].ToString() + "    版本号：" + ConfigurationManager.AppSettings["version"] + "    技术服务热线：" + table.Rows[0][1].ToString();

            DataTable dt= dbOperation.GetDbHelper().GetDataSet(string.Format("SELECT INFO_NAME,image from sys_client_sysdept where INFO_CODE ='{0}'", (Application.Current.Resources["User"] as UserInfo).DepartmentID)).Tables[0];
            this._title_dept.Text = dt.Rows[0][0].ToString();
            if (dt.Rows[0][1].ToString() != null && dt.Rows[0][1].ToString() != "")
            {
                byte[] img = (byte[])dt.Rows[0][1];
                ShowSelectedIMG(img);                //以流的方式显示图片的方法
            }

            //加载父菜单和子菜单
            MainMenu_Load();
            this.SizeChanged += new SizeChangedEventHandler(MainWindow_SizeChanged);

            //加载主画面
            if (mainMenus[0].Flag_Exits == 1)
            {
                TabItem temptb = new TabItem();
                temptb.Header = "首页";
                temptb.Content = new UcMainPage();

                _tab.Items.Add(temptb);
                _tab.SelectedIndex = _tab.Items.Count - 1;
            }

            //if (!FullScreenHelper.IsFullscreen(this))
            //{
            //    FullScreenHelper.GoFullscreen(this);
            //}
            //this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
            //this.KeyDown += new KeyEventHandler(MainWindow_KeyDown);
            //this.StateChanged += new EventHandler(MainWindow_StateChanged);
        }

        //显示上传的自定义图片
        private void ShowSelectedIMG(byte[] img)
        {
            MemoryStream ms = new MemoryStream(img);//img是从数据库中读取出来的字节数组
            ms.Seek(0, System.IO.SeekOrigin.Begin);

            BitmapImage newBitmapImage = new BitmapImage();
            newBitmapImage.BeginInit();
            newBitmapImage.StreamSource = ms;
            newBitmapImage.EndInit();
            _logo.Source = newBitmapImage;
        }


        //void MainWindow_Loaded(object sender, RoutedEventArgs e)
        //{
        //    timer = new DispatcherTimer();
        //    timer.Interval = TimeSpan.FromMilliseconds(1);
        //    timer.Tick += new EventHandler(timer_Tick);
        //    timer.Start();  
        //}

        //void MainWindow_StateChanged(object sender, EventArgs e)
        //{
        //    if (this.WindowState == WindowState.Normal)
        //    {

        //    }
        //    if (this.WindowState == WindowState.Minimized)
        //    {

        //    }
        //}

        //void MainWindow_KeyDown(object sender, KeyEventArgs e)
        //{
        //    //if (e.Key == Key.F1 && !FullScreenHelper.IsFullscreen(this))
        //    //{
        //    //    FullScreenHelper.GoFullscreen(this);
        //    //}
        //    //else if (e.Key == Key.Escape && FullScreenHelper.IsFullscreen(this))
        //    //{
        //    //    FullScreenHelper.ExitFullscreen(this);
        //    //}
        //}

        //private int flag = 0;
        //void timer_Tick(object sender, EventArgs e)
        //{
        //    if (flag == 0)
        //    {
        //        Application.Current.Resources.Add("省市表", dbOperation.GetProvinceCity());
        //        //header = new UcHeaderTitle(this);
        //        //grid_header.Children.Add(header);
        //        //header.SetUserName(this.userName);
        //        //header.SetDate(DateTime.Now);
        //        //mainMenu = new MainMenu(this);
        //        //grid_MainMenu.Children.Add(mainMenu);
        //        //加载菜单
        //        MainMenu_Load();
        //        this.SizeChanged += new SizeChangedEventHandler(MainWindow_SizeChanged);
        //        flag = 1;
        //        timer.Interval = new TimeSpan(1000);

        //    }
        //    //header.UpdateTime();
        //}


        //加载父菜单和子菜单
        private void MainMenu_Load()
        {
            int flag_exits = 0;

            //用户的查看权限
            string strSql = "SELECT rp.SUB_ID,s.SUB_NAME,s.SUB_FATHER_ID " +
                            "FROM sys_sub s ,sys_rolepermission rp , sys_client_user u " +
                            "WHERE s.SUB_ID = rp.SUB_ID " +
                            "AND rp.ROLE_ID = u.ROLE_ID " +
                            "AND u.RECO_PKID = " + (Application.Current.Resources["User"] as UserInfo).ID +
                            " order by rp.SUB_ID";

            DataTable table = dbOperation.GetDbHelper().GetDataSet(strSql).Tables[0];
            DataRow[] row_mainmenu = table.Select("SUB_FATHER_ID = '0'");
            

            //首页
            List<MyChildMenu> childMenus = new List<MyChildMenu>();
            flag_exits = MainMenu_exits(row_mainmenu, "首页");
            mainMenus.Add(new MainMenuItem("首页", _first, "/res/firstpage_select.jpg", "/res/firstpage_normal.jpg", "/res/firstpage_unpressed.jpg", childMenus, this, flag_exits));

            //新建检测单
            childMenus = new List<MyChildMenu>();
            DataRow[] row_detect = table.Select("SUB_FATHER_ID = '2'");
            foreach (DataRow row in row_detect)
            {
                childMenus.Add(new MyChildMenu(row["SUB_NAME"].ToString(), this, 2));
            }
            flag_exits = MainMenu_exits(row_mainmenu, "检测单");
            mainMenus.Add(new MainMenuItem("检测单", _detect, "/res/detect_select.jpg", "/res/detect_normal.jpg", "/res/detect_unpressed.jpg", childMenus, this, flag_exits));

            //统计报表
            childMenus = new List<MyChildMenu>();
            DataRow[] row_report = table.Select("SUB_FATHER_ID = '3'");
            foreach (DataRow row in row_report)
            {
                childMenus.Add(new MyChildMenu(row["SUB_NAME"].ToString(), this, 3));
            }
            flag_exits = MainMenu_exits(row_mainmenu, "数据统计");
            mainMenus.Add(new MainMenuItem("数据统计", _report, "/res/report_select.jpg", "/res/report_normal.jpg", "/res/report_unpressed.jpg", childMenus, this, flag_exits));


            //统计分析
            childMenus = new List<MyChildMenu>();
            DataRow[] row_analysis = table.Select("SUB_FATHER_ID = '4'");
            foreach (DataRow row in row_analysis)
            {
                childMenus.Add(new MyChildMenu(row["SUB_NAME"].ToString(), this, 4));
            }
            flag_exits = MainMenu_exits(row_mainmenu, "数据分析");
            mainMenus.Add(new MainMenuItem("数据分析", _analysis, "/res/analysis_select.jpg", "/res/analysis_normal.jpg", "/res/analysis_unpressed.jpg", childMenus, this, flag_exits));


            //检测任务
            childMenus = new List<MyChildMenu>();
            DataRow[] row_task = table.Select("SUB_FATHER_ID = '5'");
            foreach (DataRow row in row_task)
            {
                childMenus.Add(new MyChildMenu(row["SUB_NAME"].ToString(), this, 5));
            }
            flag_exits = MainMenu_exits(row_mainmenu, "检测任务");
            mainMenus.Add(new MainMenuItem("检测任务", _task, "/res/task_select.jpg", "/res/task_normal.jpg", "/res/task_unpressed.jpg", childMenus, this, flag_exits));


            //风险预警
            childMenus = new List<MyChildMenu>();
            DataRow[] row_warning = table.Select("SUB_FATHER_ID = '6'");
            foreach (DataRow row in row_warning)
            {
                childMenus.Add(new MyChildMenu(row["SUB_NAME"].ToString(), this, 6));
            }
            flag_exits = MainMenu_exits(row_mainmenu, "风险预警");
            mainMenus.Add(new MainMenuItem("风险预警", _warning, "/res/warning_select.jpg", "/res/warning_normal.jpg", "/res/warning_unpressed.jpg", childMenus, this, flag_exits));


            //系统管理
            childMenus = new List<MyChildMenu>();
            DataRow[] row_system = table.Select("SUB_FATHER_ID = '7'");
            foreach (DataRow row in row_system)
            {
                childMenus.Add(new MyChildMenu(row["SUB_NAME"].ToString(), this, 7));
            }
            flag_exits = MainMenu_exits(row_mainmenu, "系统管理");
            mainMenus.Add(new MainMenuItem("系统管理", _system, "/res/system_select.jpg", "/res/system_normal.jpg", "/res/system_unpressed.jpg", childMenus, this, flag_exits));


            //帮助
            childMenus = new List<MyChildMenu>();
            DataRow[] row_help = table.Select("SUB_FATHER_ID = '8'");
            foreach (DataRow row in row_help)
            {
                childMenus.Add(new MyChildMenu(row["SUB_NAME"].ToString(), this, 8));
            }
            flag_exits = MainMenu_exits(row_mainmenu, "帮助");
            mainMenus.Add(new MainMenuItem("帮助", _help, "/res/help_select.jpg", "/res/help_normal.jpg", "/res/help_unpressed.jpg", childMenus, this, flag_exits));

            //mainMenus[0].LoadChildMenu();
        }


        //判断用户的菜单权限
        private int MainMenu_exits(DataRow[] sub_row, string mainmenu)
        {
            int flag_exits = 0;
            foreach (DataRow row in sub_row)
            {
                if (row["SUB_NAME"].ToString() == mainmenu)
                {
                    flag_exits = 1;
                    break;
                }
            }
            return flag_exits;
        }

        //关闭子窗口
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string header = btn.Tag.ToString();
            foreach (TabItem item in _tab.Items)
            {
                if (item.Header.ToString() == header)
                {
                    _tab.Items.Remove(item);
                    break;
                }
            } 
        }

        void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.ActualHeight > SystemParameters.WorkArea.Height || this.ActualWidth > SystemParameters.WorkArea.Width)
            {
                this.WindowState = System.Windows.WindowState.Normal;
                max_MouseDown(null, null);
            }

        }

        //退出主窗体
        private void exit_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.CloseWindow();
        }

        public void CloseWindow()
        {
            //if (timer != null && timer.IsEnabled)
            //{
            //    timer.Stop();
            //}
            this.Close();
        }

        //最小化窗口
        private void min_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        //最大化或正常窗口
        private void max_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (max.ToolTip.ToString() == "最大化")
            {
                MaxWindow();
            }
            else if (max.ToolTip.ToString() == "正常")
            {
                NormalWindow();
            }
        }

        //最大化窗口
        private void MaxWindow()
        {
            max.ToolTip = "正常";
            rcnormal = new Rect(this.Left, this.Top, this.Width, this.Height);//保存下当前位置与大小
            this.Left = 0;//设置位置
            this.Top = 0;
            Rect rc = SystemParameters.WorkArea;//获取工作区大小
            this.Width = rc.Width;
            this.Height = rc.Height;

        }

        //正常窗口
        private void NormalWindow()
        {
            max.ToolTip = "最大化";
            this.Left = rcnormal.Left;
            this.Top = rcnormal.Top;
            this.Width = rcnormal.Width;
            this.Height = rcnormal.Height;
        }

        //移动窗口
        private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            this.Left += e.HorizontalChange;
            this.Top += e.VerticalChange;
        }

        private void min_MouseEnter(object sender, MouseEventArgs e)
        {
            min.Source = new BitmapImage(new Uri("pack://application:,," + "/res/min_on.png"));
        }

        private void max_MouseEnter(object sender, MouseEventArgs e)
        {
            max.Source = new BitmapImage(new Uri("pack://application:,," + "/res/max_on.png"));
        }

        private void exit_MouseEnter(object sender, MouseEventArgs e)
        {
            exit.Source = new BitmapImage(new Uri("pack://application:,," + "/res/close_on.png"));
        }

        private void all_MouseLeave(object sender, MouseEventArgs e)
        {
            min.Source = new BitmapImage(new Uri("pack://application:,," + "/res/min.png"));
            max.Source = new BitmapImage(new Uri("pack://application:,," + "/res/max.png"));
            exit.Source = new BitmapImage(new Uri("pack://application:,," + "/res/close.png"));
        }
    }

    public class MainMenuItem
    {
        public string Name;
        public BitmapImage img_mouseEnter;
        public BitmapImage img_mouseLeave;
        public BitmapImage img_mouseUnpressed;
        public List<MyChildMenu> childMenus;
        public ChildMenu childMenu;
        public Image img;
        public Grid grid_Menu;
        private MainWindow mainWindow;
        public int Flag_Exits;

        public MainMenuItem(string name, Image img, string mouseEnterBackImgPath, string mouseLeaveBackImgPath, string mouseUnpressedBackImgPath, List<MyChildMenu> childMenus, MainWindow mainWindow, int flag_exits)
        {
            this.Name = name;
            this.childMenus = childMenus;
            this.mainWindow = mainWindow;
            grid_Menu = mainWindow.grid_Menu;
            this.childMenu = new ChildMenu(childMenus);
            this.img = img;
            this.img.Tag = name;
            this.Flag_Exits = flag_exits;
            img_mouseEnter = new BitmapImage(new Uri("pack://application:,," + mouseEnterBackImgPath));
            img_mouseLeave = new BitmapImage(new Uri("pack://application:,," + mouseLeaveBackImgPath));
            img_mouseUnpressed = new BitmapImage(new Uri("pack://application:,," + mouseUnpressedBackImgPath));
            if (Flag_Exits == 1)
            {
                this.img.Source = img_mouseLeave;
            }
            else
            {
                this.img.Source = img_mouseUnpressed;
                this.img.ToolTip = "无操作权限";
            }

            this.img.MouseDown += new MouseButtonEventHandler(img_MouseDown);
            this.img.MouseEnter += new MouseEventHandler(img_MouseEnter);
            //this.img.MouseLeave += new MouseEventHandler(img_MouseLeave);

        }


        void img_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Name == "首页" && Flag_Exits == 1)
            {
                int flag = 0;
                foreach (TabItem item in mainWindow._tab.Items)
                {
                    if (item.Header.ToString() == Name)
                    {
                        mainWindow._tab.SelectedItem = item;
                        flag = 1;
                        break;
                    }
                }
                if (flag == 0)
                {
                    TabItem temptb = new TabItem();
                    temptb.Header = Name;
                    temptb.Content = new UcMainPage();

                    mainWindow._tab.Items.Add(temptb);
                    mainWindow._tab.SelectedIndex = mainWindow._tab.Items.Count - 1;
                }
            }
            //mainWindow.IsEnbleMouseEnterLeave = true;
            if (Flag_Exits == 1)
            {
                for (int i = 0; i < grid_Menu.Children.Count; i++)
                {
                    grid_Menu.Children.RemoveAt(i);
                    i--;
                }
                this.grid_Menu.Children.Add(childMenu);
            }
        }

        //void img_MouseLeave(object sender, MouseEventArgs e)
        //{

        //    if (Flag_Exits == 1)
        //    {
        //        ((Image)sender).Source = img_mouseLeave;
        //        mainWindow._childmenubar.ImageSource = new BitmapImage(new Uri("pack://application:,," + "/res/childmenu_bar.jpg"));
        //    }
        //}

        public void LoadChildMenu()
        {
            //if (mainWindow.IsEnbleMouseEnterLeave)
            //{
                for (int i = 0; i < grid_Menu.Children.Count; i++)
                {
                    grid_Menu.Children.RemoveAt(i);
                    i--;
                }
                this.grid_Menu.Children.Add(childMenu);
            //}
            //一旦鼠标移在主菜单图标上，主菜单的图标变成黄色，其余均为正常色
            for (int i = 0; i < mainWindow.mainMenus.Count; i++)
            {
                if (mainWindow.mainMenus[i].Flag_Exits == 1)
                {
                    mainWindow.mainMenus[i].img.Source = mainWindow.mainMenus[i].img_mouseLeave;
                }
            }
            img.Source = img_mouseEnter;

            switch (Name)
            {
                case "首页": mainWindow._childmenubar.ImageSource = new BitmapImage(new Uri("pack://application:,," + "/res/firstpage_bar.jpg"));
                    break;
                case "检测单": mainWindow._childmenubar.ImageSource = new BitmapImage(new Uri("pack://application:,," + "/res/detect_bar.jpg"));
                    break;
                case "数据统计": mainWindow._childmenubar.ImageSource = new BitmapImage(new Uri("pack://application:,," + "/res/report_bar.jpg"));
                    break;
                case "数据分析": mainWindow._childmenubar.ImageSource = new BitmapImage(new Uri("pack://application:,," + "/res/analysis_bar.jpg"));
                    break;
                case "风险预警": mainWindow._childmenubar.ImageSource = new BitmapImage(new Uri("pack://application:,," + "/res/warning_bar.jpg"));
                    break;
                case "检测任务": mainWindow._childmenubar.ImageSource = new BitmapImage(new Uri("pack://application:,," + "/res/task_bar.jpg"));
                    break;
                case "系统管理": mainWindow._childmenubar.ImageSource = new BitmapImage(new Uri("pack://application:,," + "/res/system_bar.jpg"));
                    break;
                case "帮助": mainWindow._childmenubar.ImageSource = new BitmapImage(new Uri("pack://application:,," + "/res/help_bar.jpg"));
                    break;
                default: break;

            }
        }

        void img_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Flag_Exits == 1)
            {
                LoadChildMenu();
            }
        }

    }


}
