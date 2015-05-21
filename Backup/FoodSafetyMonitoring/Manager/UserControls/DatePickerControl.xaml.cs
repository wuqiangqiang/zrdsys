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
    /// DatePickerControl.xaml 的交互逻辑
    /// </summary>
    public partial class DatePickerControl : UserControl
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
            get { return dtp.Value; }
            set
            {
                this.dtp.Value = value;
            }
        }

        //时间改变事件
        public event EventHandler OnValueChanged;

        public DatePickerControl()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(DatePickerControl_Loaded);
        }

        void DatePickerControl_Loaded(object sender, RoutedEventArgs e)
        {
            //this.dtp.Value = DateTime.Now;
            this.dtp.Format = this.Format;
            this.dtp.FormatString = "yyyy-MM-dd";
            this.dtp.IsEditable = false;
        }

        private void dtp_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            this.Value = dtp.Value;
            //触发用户控件的OnValueChanged事件
            if (OnValueChanged != null)
            {
                OnValueChanged(sender, e);
            }
        }
    }
}
