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

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// UcMainPage.xaml 的交互逻辑
    /// </summary>
    public partial class UcMainPage : UserControl
    { 
        public UcMainPage()
        {
            InitializeComponent();
            _webBrowser.Source = new Uri(string.Format("http://61.183.9.175/ulsocialevent/getMapTesttt.do?user_id={0}", (Application.Current.Resources["User"] as UserInfo).ID));
            //_webBrowser.Source = new Uri("http://www.baidu.com");

        } 
    }
}
