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
using FoodSafetyMonitoring.Manager;
using FoodSafetyMonitoring.dao;

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// SysHelp.xaml 的交互逻辑
    /// </summary>
    public partial class SysHelp : UserControl
    {
        public SysHelp()
        {
            InitializeComponent();

            string supplierId = (Application.Current.Resources["User"] as UserInfo).SupplierId;

            if (supplierId == "" || supplierId == "zrd")
            {
                _help.ImageSource = new BitmapImage(new Uri("pack://application:,," + "/res/zrd.png"));
            }
            else if(supplierId == "nkrx")
            {
                 _help.ImageSource = new BitmapImage(new Uri("pack://application:,," + "/res/nkrx.png"));
            }
        }
    }
}
