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
using FoodSafetyMonitoring.dao;
using System.Data;
using FoodSafetyMonitoring.Common;
using Toolkit = Microsoft.Windows.Controls;
using FoodSafetyMonitoring.Manager.UserControls;
using System.Printing;

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// UcCreateCertificate_product.xaml 的交互逻辑
    /// </summary>
    public partial class UcCreateCertificate_product : UserControl
    {
        public IDBOperation dbOperation = null;
        string userId = (Application.Current.Resources["User"] as UserInfo).ID;
        string username = (Application.Current.Resources["User"] as UserInfo).ShowName;


        public UcCreateCertificate_product(IDBOperation dbOperation)
        {
            InitializeComponent();

            this.dbOperation = dbOperation;

            ComboboxTool.InitComboboxSource(_source_company, string.Format(" call p_user_company_wcz('{0}') ", userId), "lr");
        }

        private void _query_Click(object sender, RoutedEventArgs e)
        {

        }

        private void _create_Click(object sender, RoutedEventArgs e)
        {

        }

        private void _print_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
