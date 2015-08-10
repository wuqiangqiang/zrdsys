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

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// CertificatePreview.xaml 的交互逻辑
    /// </summary>
    public partial class CertificatePreview : Window
    {
        public CertificatePreview()
        {
            InitializeComponent();
        }

        private void _close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void _print_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
