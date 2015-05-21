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
    /// TextboxSearchControl.xaml 的交互逻辑
    /// </summary>
    public partial class TextboxSearchControl : UserControl
    {
        //图片点击事件
        public event EventHandler ImageClick;

        public string Text { get; set; }


        public TextboxSearchControl()
        {
            InitializeComponent();
            this.Text = "";
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.Text = this.txtSearchInfo.Text;
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ImageClick != null)
            {
                ImageClick(sender, e);
            }
        }
    }
}
