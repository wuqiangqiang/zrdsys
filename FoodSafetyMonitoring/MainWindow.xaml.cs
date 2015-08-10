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
        private DispatcherTimer timer;
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

            //if (!FullScreenHelper.IsFullscreen(this))
            //{
            //    FullScreenHelper.GoFullscreen(this);
            //}
            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
            //this.KeyDown += new KeyEventHandler(MainWindow_KeyDown);
            //this.StateChanged += new EventHandler(MainWindow_StateChanged);
        }


        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

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

        private int flag = 0;
        void timer_Tick(object sender, EventArgs e)
        {
            if (flag == 0)
            {
                LoadWindow load = new LoadWindow();
                load.Show();

                Application.Current.Resources.Add("省市表", dbOperation.GetProvinceCity());
                UserInfo userInfo = Application.Current.Resources["User"] as UserInfo;
                this.userName = userInfo.ShowName;

                //加载标题
                this._user.Text = this.userName;
                //this._date.Text = DateTime.Now.ToLongDateString().ToString() +  DateTime.Now.ToString("dddd");
                //this._date.Text = DateTime.Now.ToLongDateString().ToString();

                DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("select companyName,phone from t_supplier where supplierId ='{0}'", (Application.Current.Resources["User"] as UserInfo).SupplierId == "" ? "zrd" : (Application.Current.Resources["User"] as UserInfo).SupplierId)).Tables[0];
                this._bottom.Text = table.Rows[0][0].ToString() + "    版本号：" + ConfigurationManager.AppSettings["version"] + "    技术服务热线：" + table.Rows[0][1].ToString();

                DataTable dt = dbOperation.GetDbHelper().GetDataSet(string.Format("SELECT INFO_NAME,image from sys_client_sysdept where INFO_CODE ='{0}'", (Application.Current.Resources["User"] as UserInfo).DepartmentID)).Tables[0];
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
                TabItem temptb = new TabItem();
                temptb.Header = "首页";
                temptb.Tag = "1";
                temptb.Content = new UcMainPage();
                _tab.Items.Add(temptb);
                _tab.SelectedIndex = _tab.Items.Count - 1;

                flag = 1;
                timer.Interval = new TimeSpan(1000);

                load.Close();

            }
            //header.UpdateTime();
        }


        //加载父菜单和子菜单
        private void MainMenu_Load()
        {
            //int flag_exits = 0;

            //用户的查看权限
            string strSql = "SELECT rp.SUB_ID,s.SUB_NAME,s.SUB_FATHER_ID,s.SUB_NORMAL_URL " +
                            "FROM sys_sub_hb s ,sys_rolepermission_hb rp , sys_client_user u " +
                            "WHERE s.SUB_ID = rp.SUB_ID " +
                            "AND rp.ROLE_ID = u.ROLE_ID " +
                            "AND u.RECO_PKID = " + (Application.Current.Resources["User"] as UserInfo).ID +
                            " order by rp.SUB_ID asc";

            DataTable table = dbOperation.GetDbHelper().GetDataSet(strSql).Tables[0];
            //一级菜单
            DataRow[] row_mainmenu = table.Select("SUB_FATHER_ID = '0'");
            //定义数组存放：一级菜单图片控件和一级菜单文字控件
            Image[] images = new Image[] { _image_0,_image_1,_image_2,_image_3,_image_4,_image_5,_image_6,
                                           _image_7,_image_8,_image_9,_image_10,_image_11,_image_12,_image_13};
            TextBlock[] texts = new TextBlock[] { _text_0, _text_1, _text_2, _text_3, _text_4, _text_5, _text_6,
                                                  _text_7, _text_8, _text_9, _text_10, _text_11, _text_12, _text_13};
            Grid[] grids = new Grid[] { _grid_0, _grid_1, _grid_2, _grid_3, _grid_4, _grid_5, _grid_6,
                                                  _grid_7, _grid_8, _grid_9, _grid_10, _grid_11, _grid_12, _grid_13};

            int i = 0;
            foreach (DataRow row in row_mainmenu )
            {
                //二级菜单
                List<MyChildMenu> childMenus = new List<MyChildMenu>();
                DataRow[] row_childmenu = table.Select("SUB_FATHER_ID ='" + row["SUB_ID"] + "'");
                //当一级菜单存在，但二级菜单为空时
                if( row_childmenu.Count() == 0 )
                {

                }
                else
                {
                    foreach (DataRow row_child in row_childmenu)
                    {
                        DataRow[] row_child_childmenu = table.Select("SUB_FATHER_ID ='" + row_child["SUB_ID"] + "'");
                        childMenus.Add(new MyChildMenu(row_child["SUB_NAME"].ToString(), this, row_child_childmenu));
                    }
                }
                //鉴于帮助是最后一个菜单，帮助又必须显示在_grid_13区域内，所以特殊处理如下
                if (row["SUB_NAME"].ToString() == "帮助")
                {
                    mainMenus.Add(new MainMenuItem(row["SUB_NAME"].ToString(), images[13], grids[13], row["SUB_NORMAL_URL"].ToString(), childMenus, this));
                    texts[13].Text = row["SUB_NAME"].ToString();
                }
                else
                {
                    mainMenus.Add(new MainMenuItem(row["SUB_NAME"].ToString(), images[i], grids[i], row["SUB_NORMAL_URL"].ToString(), childMenus, this));
                    texts[i].Text = row["SUB_NAME"].ToString();
                } 
                i = i + 1;
            }       
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

        //判断用户的菜单权限
        //private int MainMenu_exits(DataRow[] sub_row, string mainmenu)
        //{
        //    int flag_exits = 0;
        //    foreach (DataRow row in sub_row)
        //    {
        //        if (row["SUB_NAME"].ToString() == mainmenu)
        //        {
        //            flag_exits = 1;
        //            break;
        //        }
        //    }
        //    return flag_exits;
        //}

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
        //public BitmapImage img_mouseEnter;
        public BitmapImage img_mouseLeave;
        //public BitmapImage img_mouseUnpressed;
        public List<MyChildMenu> childMenus;
        public ChildMenu childMenu;
        public Image img;
        public Grid grid_Menu;
        private MainWindow mainWindow;
        public Grid grid;
        //public int Flag_Exits;

        public MainMenuItem(string name, Image img,Grid grid, string mouseLeaveBackImgPath, List<MyChildMenu> childMenus, MainWindow mainWindow)
        {
            this.Name = name;
            this.childMenus = childMenus;
            this.mainWindow = mainWindow;
            grid_Menu = mainWindow.grid_Menu;
            this.childMenu = new ChildMenu(childMenus);
            this.img = img;
            this.grid = grid;
            this.img.Tag = name;
            //this.Flag_Exits = flag_exits;
            //img_mouseEnter = new BitmapImage(new Uri("pack://application:,," + mouseEnterBackImgPath));
            img_mouseLeave = new BitmapImage(new Uri("pack://application:,," + mouseLeaveBackImgPath));
            //img_mouseUnpressed = new BitmapImage(new Uri("pack://application:,," + mouseUnpressedBackImgPath));
            //if (Flag_Exits == 1)
            //{
                this.img.Source = img_mouseLeave;
            //}
            //else
            //{
            //    this.img.Source = img_mouseUnpressed;
            //    this.img.ToolTip = "无操作权限";
            //}

            this.img.MouseDown += new MouseButtonEventHandler(img_MouseDown);
            this.img.MouseEnter += new MouseEventHandler(img_MouseEnter);
            //this.img.MouseLeave += new MouseEventHandler(img_MouseLeave);

        }


        void img_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //if (Name == "首页" && Flag_Exits == 1)
            if (Name == "首页" )
            {
                int flag = 0;
                foreach (TabItem item in mainWindow._tab.Items)
                {
                    if (item.Tag.ToString() == "1")
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
                    temptb.Tag = "1";
                    temptb.Content = new UcMainPage();

                    mainWindow._tab.Items.Add(temptb);
                    mainWindow._tab.SelectedIndex = mainWindow._tab.Items.Count - 1;
                }
            }
            //mainWindow.IsEnbleMouseEnterLeave = true;
            //if (Flag_Exits == 1)
            //{
                for (int i = 0; i < grid_Menu.Children.Count; i++)
                {
                    grid_Menu.Children.RemoveAt(i);
                    i--;
                }
                this.grid_Menu.Children.Add(childMenu);
            //}

            //if (Name == "首页")
            //{

            //}
            //else
            //{

            //}
            //一旦鼠标点击在主菜单图标上，主菜单的背景色颜色加深
            for (int i = 0; i < mainWindow.mainMenus.Count; i++)
            {
                //if (mainWindow.mainMenus[i].Flag_Exits == 1)
                //{
                //mainWindow.mainMenus[i].img.Source = mainWindow.mainMenus[i].img_mouseLeave;
                mainWindow.mainMenus[i].grid.Background = new SolidColorBrush(Color.FromRgb(25, 86, 162));
                //}
            }
            grid.Background = new SolidColorBrush(Color.FromRgb(25, 49, 115));
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
                //if (mainWindow.mainMenus[i].Flag_Exits == 1)
                //{
                    mainWindow.mainMenus[i].img.Source = mainWindow.mainMenus[i].img_mouseLeave;
                //}
            }
            //img.Source = img_mouseEnter;
        }

        void img_MouseEnter(object sender, MouseEventArgs e)
        {
            //if (Flag_Exits == 1)
            //{
               // LoadChildMenu();
            //}
        }

    }


}
