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
using System.Windows.Threading;
using FoodSafetyMonitoring.dao;
using DBUtility;

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// UcMainPage.xaml 的交互逻辑
    /// </summary>
    public partial class UcMainPage : UserControl
    {
        DbHelperMySQL dbOperation;
        public UcMainPage()
        {
            InitializeComponent();

            dbOperation = DBUtility.DbHelperMySQL.CreateDbHelper();
            //首页地址改为从数据库中获取
            string page_url = dbOperation.GetSingle("select firstpageurl from t_url ").ToString();
            if (page_url == "")
            {
                page_url = "http://www.zrodo.com:8040/ulsocialevent/getMapTesttt.do?user_id={0}";
            }
            _webBrowser.Source = new Uri(string.Format(page_url, (Application.Current.Resources["User"] as UserInfo).ID));

           // _webBrowser.Source = new Uri(string.Format("http://www.zrodo.com:8040/ulsocialevent/getMapTesttt.do?user_id={0}", (Application.Current.Resources["User"] as UserInfo).ID));

        } 
    }
}
