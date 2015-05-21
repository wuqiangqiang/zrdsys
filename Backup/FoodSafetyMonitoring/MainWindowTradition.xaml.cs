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
using System.Windows.Shapes;
using Toolkit = Microsoft.Windows.Controls;
using FoodSafetyMonitoring.Manager;
using FoodSafetyMonitoring.dao;
using System.Windows.Threading;
using System.Data;
using DBUtility;

namespace FoodSafetyMonitoring
{
    /// <summary>
    /// MainWindowTradition.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindowTradition : Window
    {
        public IDBOperation dbOperation = null;
        UcMainPage mainPage = null;
        private DBUtility.DbHelperMySQL dbHelper = null;

        class FatherMenuTitle
        {
            public string Name { get; set; }
            public List<string> ChildMenuTitle { get; set; }

            public FatherMenuTitle(string Name, List<string> ChildMenuTitle)
            {
                this.Name = Name;
                this.ChildMenuTitle = ChildMenuTitle;
            }
        }

        class SubInfo
        {
            public string SubId { get; set; }
            public string SubName { get; set; }
            public string SubFatherId { get; set; }
        }

        //List<FatherMenuTitle> menuTitles = new List<FatherMenuTitle>() 
        //{
        //    new FatherMenuTitle ("首页",new List<string>()),
        //    new FatherMenuTitle ("检测单",new List<string>()),
        //    new FatherMenuTitle ("统计报表",new List<string>(){"日报表" , "月报表","年报表" , "自定义报表"  }),
        //    new FatherMenuTitle ("统计分析",new List<string>(){"对比分析" , "趋势分析","区域分析" }),
        //    new FatherMenuTitle ("检测任务",new List<string>(){"任务分配" ,"任务考评"}),
        //    new FatherMenuTitle ("实时风险",new List<string>(){"风险预警", "风险复核"}),
        //    new FatherMenuTitle ("系统管理",new List<string>(){"部门管理","角色管理" ,"权限管理","用户管理","来源单位管理", "系统日志" }),
        //    new FatherMenuTitle ("帮助",new List<string>(){"帮助","关于"})
        //};

        private List<FatherMenuTitle> menuTitles = new List<FatherMenuTitle>(); 
        private List<SubInfo> subList = new List<SubInfo>();
        private List<string> subChildList = new List<string>();

        //根据用户的查看权限显示菜单标题栏
        private void MenuRole()
        {
            string strSql = "SELECT rp.SUB_ID,s.SUB_NAME,s.SUB_FATHER_ID " +
                            "FROM sys_sub s ,sys_rolepermission rp , sys_client_user u "+
                            "WHERE s.SUB_ID = rp.SUB_ID " +
                            "AND rp.ROLE_ID = u.ROLE_ID " +
                            "AND u.RECO_PKID = " + (Application.Current.Resources["User"] as UserInfo).ID +
                            " order by rp.SUB_ID";

            DataTable table = dbHelper.GetDataSet(strSql).Tables[0];

            subList.Clear();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                SubInfo info = new SubInfo();
                info.SubId = table.Rows[i][0].ToString();
                info.SubName = table.Rows[i][1].ToString();
                info.SubFatherId = table.Rows[i][2].ToString();
                subList.Add(info);
            }

            //第一层菜单
            var subFatherArray = (from c in subList where (c.SubFatherId == '0'.ToString()) select new { c.SubId, c.SubName }).ToArray();

            for (int i = 0; i < subFatherArray.Length; i++)
            {
                string subid = subFatherArray[i].SubId;
                //第二层菜单
                subChildList = (from t in subList where (t.SubFatherId == subid.ToString()) select t.SubName).ToList();
                FatherMenuTitle fathermenu = new FatherMenuTitle(subFatherArray[i].SubName,subChildList);
                menuTitles.Add(fathermenu);
            }
        }

        public MainWindowTradition(IDBOperation dbOperation)
        {
            InitializeComponent();

            this.dbOperation = dbOperation;
            dbHelper = DbHelperMySQL.CreateDbHelper();

            //系统标题的显示
            string sql = string.Format("select title from sys_client_sysdept where INFO_CODE = '{0}'", (Application.Current.Resources["User"] as UserInfo).DepartmentID);
            string title = dbOperation.GetDbHelper().GetSingle(sql).ToString();

            this.Title = title + "              "+"当前登录用户：" + (Application.Current.Resources["User"] as UserInfo).ShowName;

            Application.Current.Resources.Add("省市表", dbOperation.GetProvinceCity());
            MenuRole();
            foreach (FatherMenuTitle fatherMenuTitle in menuTitles)
            {
                MenuItem fatherMenu = new MenuItem();
                fatherMenu.FontSize = 16;
                fatherMenu.Header = fatherMenuTitle.Name;
                if (fatherMenuTitle.ChildMenuTitle.Count == 0)
                {
                    fatherMenu.Click += new RoutedEventHandler(childMenu_Click);
                }
                else
                {
                    foreach (string childMenuTitle in fatherMenuTitle.ChildMenuTitle)
                    {
                        MenuItem childMenu = new MenuItem();
                        childMenu.FontSize = 16;
                        childMenu.Header = childMenuTitle;
                        childMenu.Click += new RoutedEventHandler(childMenu_Click);
                        fatherMenu.Items.Add(childMenu);
                    }
                }
                _menu.Items.Add(fatherMenu);
            }

            mainPage = new UcMainPage();
            grid_Component.Children.Add(mainPage);

            this.WindowState = WindowState.Maximized;
        }



        void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }


        private Dictionary<string, UserControlTemplet> moduleMap = new Dictionary<string, UserControlTemplet>();


        void childMenu_Click(object sender, RoutedEventArgs e)
        {
            string menuTitle = (sender as MenuItem).Header.ToString();

            foreach (UserControlTemplet userControlTemplet in moduleMap.Values)
            {
                userControlTemplet.Visibility = Visibility.Hidden;
            }

            //如果存在，则移除，为了确保每次打开都刷新画面
            if (moduleMap.ContainsKey(menuTitle))
            {
                moduleMap.Remove(menuTitle);
            }

            //if (moduleMap.ContainsKey(menuTitle))
            //{
            //    moduleMap[menuTitle].Visibility = Visibility.Visible;
            //}
            //else
            //{
                switch (menuTitle)
                {
                    case "首页": break;
                    case "新建检测单": moduleMap.Add(menuTitle, new UserControlTemplet(menuTitle, new UcDetectBillManager())); break;
                    case "统计报表": break;
                    case "日报表": moduleMap.Add(menuTitle, new UserControlTemplet(menuTitle, new SysDayReport(dbOperation))); break;
                    case "月报表": moduleMap.Add(menuTitle, new UserControlTemplet(menuTitle, new SysMonthReport(dbOperation))); break;
                    case "年报表": moduleMap.Add(menuTitle, new UserControlTemplet(menuTitle, new SysYearReport(dbOperation))); break;
                    case "自定义报表": moduleMap.Add(menuTitle, new UserControlTemplet(menuTitle, new SysDesignReport(dbOperation))); break;
                    case "统计分析": break;
                    case "对比分析": moduleMap.Add(menuTitle, new UserControlTemplet(menuTitle, new SysComparisonAndAnalysis(dbOperation))); break;
                    case "趋势分析": moduleMap.Add(menuTitle, new UserControlTemplet(menuTitle, new SysTrendAnalysis(dbOperation))); break;
                    case "区域分析": moduleMap.Add(menuTitle, new UserControlTemplet(menuTitle, new SysAreaAnalysis(dbOperation))); break;
                    case "检测任务": break;
                    case "任务分配": moduleMap.Add(menuTitle, new UserControlTemplet(menuTitle, new UcTaskAllocation())); break;
                    case "任务考评": moduleMap.Add(menuTitle, new UserControlTemplet(menuTitle, new SysTaskCheck(dbOperation))); break;
                    case "风险预警": break;
                    case "实时风险": moduleMap.Add(menuTitle, new UserControlTemplet(menuTitle, new SysWarningInfo(dbOperation))); break;
                    case "预警复核": moduleMap.Add(menuTitle, new UserControlTemplet(menuTitle, new SysReviewInfo(dbOperation))); break;
                    case "系统管理": break;
                    case "部门管理": moduleMap.Add(menuTitle, new UserControlTemplet(menuTitle, new SysDeptManager(dbOperation))); break;
                    case "角色管理": moduleMap.Add(menuTitle, new UserControlTemplet(menuTitle, new SysRoleManager())); break;
                    case "权限管理": moduleMap.Add(menuTitle, new UserControlTemplet(menuTitle, new SysRolePowerManager())); break;
                    case "用户管理": moduleMap.Add(menuTitle, new UserControlTemplet(menuTitle, new SysUserManager())); break;
                    //case "来源单位管理": moduleMap.Add(menuTitle, new UserControlTemplet(menuTitle, new UcSourceCompanyManager(dbOperation))); break;
                    case "系统日志": moduleMap.Add(menuTitle, new UserControlTemplet(menuTitle, new SysLogManager())); break;
                    case "帮助": moduleMap.Add(menuTitle, new UserControlTemplet(menuTitle, new UcUnrealizedModul())); break;
                    case "关于": moduleMap.Add(menuTitle, new UserControlTemplet(menuTitle, new UcUnrealizedModul())); break;
                    default: return;
                }
                if (moduleMap.ContainsKey(menuTitle))
                {
                    moduleMap[menuTitle].CloseUserControlEvent += new UserControlTemplet.CloseUserControlEventHandler(MainWindowTradition_CloseUserControlEvent);
                }
                if (moduleMap.ContainsKey(menuTitle) && moduleMap[menuTitle] != null)
                {
                    grid_Component.Children.Add(moduleMap[menuTitle]);
                }
            //}
                //如果直接点击首页菜单,则将首页显示；若不是的话，首页隐藏
                if (menuTitle == "首页")
                {
                    mainPage.Visibility = Visibility.Visible;
                }
                else
                {
                    mainPage.Visibility = Visibility.Hidden;
                }

        }

        void MainWindowTradition_CloseUserControlEvent(string title)
        {
            mainPage.Visibility = Visibility.Visible;
        }
    }

}
