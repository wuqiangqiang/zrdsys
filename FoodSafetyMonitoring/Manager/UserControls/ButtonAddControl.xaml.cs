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

namespace FoodSafetyMonitoring.Manager.UserControls
{
    /// <summary>
    /// ButtonAddControl.xaml 的交互逻辑
    /// </summary>
    public partial class ButtonAddControl : UserControl
    {

        public event EventHandler ContentClick;

        public event EventHandler AddClick;

        public object Text
        {
            get { return this.btn.Content; }
            set 
            { 
                this.btn.Content = value;
                this.add.Content = value;
            }
        }

        public ButtonAddControl()
        {
            InitializeComponent();
            this.Text = "";
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            if (ContentClick != null)
            {
                ContentClick(this, e);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (AddClick != null)
            {
                AddClick(this, e);
            }
        }
    }
}
