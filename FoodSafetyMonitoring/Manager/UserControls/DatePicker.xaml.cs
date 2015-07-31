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
using Microsoft.Windows.Controls;

namespace FoodSafetyMonitoring.Manager.UserControls
{
    /// <summary>
    /// DatePicker.xaml 的交互逻辑
    /// </summary>
    public partial class DatePicker : UserControl
    {
        //设置时间控件的格式
        private DateTimeFormat _format = DateTimeFormat.ShortDate;

        public DateTimeFormat Format
        {
            get { return _format; }
            set { _format = value; }
        }

        //当前时间
        public DateTime? Value
        {
            get { return dtp.SelectedDate; }
            set
            {
                this.dtp.SelectedDate = value;
            }
        }
        public DatePicker()
        {
            InitializeComponent();
        }


        private void dtp_SelectedChanged(object sender, SelectionChangedEventArgs e)
        {
            this.Value = dtp.SelectedDate;
        }
    }
}
