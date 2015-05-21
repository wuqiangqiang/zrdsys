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
using System.Windows.Media.Animation;
using System.Drawing;
using System.Windows.Forms.Integration;
using System.Windows.Interop;

namespace FoodSafetyMonitoring
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class UserControlTemplet : UserControl
    {
        public delegate void CloseUserControlEventHandler(string title);
        public event CloseUserControlEventHandler CloseUserControlEvent;

        public string Title
        {
            get { return this._title.Text; }
        }

        public UserControlTemplet(string title, UserControl uc)
        {
            InitializeComponent();
            _title.Text = title;
            grid.Children.Add(uc);
        }


        private void exit_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            if (CloseUserControlEvent != null)
            {
                CloseUserControlEvent(this._title.Text);
            }
        }

    }


}
