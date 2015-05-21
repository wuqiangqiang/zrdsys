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
using System.Windows.Forms.Integration;

namespace FoodSafetyMonitoring.Manager.Themes
{
    /// <summary>
    /// UcChooseDataTime.xaml 的交互逻辑
    /// </summary>
    public partial class UcChooseDataTime : UserControl
    {
        public UcChooseDataTime()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(CalendarControl_Loaded);
        }

        void CalendarControl_Loaded(object sender, RoutedEventArgs e)
        {
            DateTime dateTime = DateTime.Now;
            WindowsFormsHost host = new WindowsFormsHost();
            System.Windows.Forms.DateTimePicker calendar = new System.Windows.Forms.DateTimePicker();
            host.Child = calendar;
            this.CalendarContain.Children.Add(host);
            WindowsFormsHost hourhost = new WindowsFormsHost();
            System.Windows.Forms.TextBox hourTextBox = new System.Windows.Forms.TextBox();
            hourTextBox.LostFocus += new EventHandler(hourTextBox_LostFocus);
            hourTextBox.Tag = "hour";
            hourTextBox.Text = dateTime.Hour.ToString();
            hourhost.Child = hourTextBox;
            this.HourContain.Children.Add(hourhost);

            WindowsFormsHost minutehost = new WindowsFormsHost();
            System.Windows.Forms.TextBox minuteTextBox = new System.Windows.Forms.TextBox();
            minuteTextBox.LostFocus += new EventHandler(minuteTextBox_LostFocus);
            minuteTextBox.Text = dateTime.Minute.ToString();
            minuteTextBox.Tag = "minute";
            minutehost.Child = minuteTextBox;
            this.MinuteContain.Children.Add(minutehost);
        }

        void minuteTextBox_LostFocus(object sender, EventArgs e)
        {
            string minuteTxt = ((System.Windows.Forms.TextBox)sender).Text;
            if (minuteTxt != null)
            {
                try
                {
                    int minute = Convert.ToInt32(minuteTxt);
                    if (minute < 0 || minute > 60)
                    {
                        MessageBox.Show("请输入0到60之间的数据！");
                    }
                }
                catch
                {
                    MessageBox.Show("请输入0到60之间的数据！");
                }

            }
        }

        void hourTextBox_LostFocus(object sender, EventArgs e)
        {
            string hourTxt = ((System.Windows.Forms.TextBox)sender).Text;
            if (hourTxt != null)
            {
                try
                {
                    int hour = Convert.ToInt32(hourTxt);
                    if (hour < 0 || hour > 24)
                    {
                        MessageBox.Show("请输入0到24之间的数据！");
                    }
                }
                catch
                {
                    MessageBox.Show("请输入0到24之间的数据！");
                }

            }
        }

        private bool Validate()
        {
            bool bok = true;
            WindowsFormsHost minutehost = (WindowsFormsHost)this.MinuteContain.Children[0];
            System.Windows.Forms.TextBox minuteText = (System.Windows.Forms.TextBox)minutehost.Child;
            string minuteTxt = minuteText.Text;
            if (minuteTxt != null)
            {
                try
                {
                    int minute = Convert.ToInt32(minuteTxt);
                    if (minute < 0 || minute > 60)
                    {
                        bok = false;
                        MessageBox.Show("请输入0到60之间的数据！");
                    }
                }
                catch
                {
                    bok = false;
                    MessageBox.Show("请输入0到60之间的数据！");
                }

            }


            WindowsFormsHost hourhost = (WindowsFormsHost)this.HourContain.Children[0];
            System.Windows.Forms.TextBox hourText = (System.Windows.Forms.TextBox)hourhost.Child;
            string hourTxt = hourText.Text;
            if (hourTxt != null)
            {
                try
                {
                    int hour = Convert.ToInt32(hourTxt);
                    if (hour < 0 || hour > 24)
                    {
                        bok = false;
                        MessageBox.Show("请输入0到24之间的数据！");
                    }
                }
                catch
                {
                    bok = false;
                    MessageBox.Show("请输入0到24之间的数据！");
                }

            }
            return bok;

        }

        public DateTime GetTime()
        {
            DateTime dateTime = DateTime.Now;
            WindowsFormsHost calendarhost = (WindowsFormsHost)this.CalendarContain.Children[0];
            System.Windows.Forms.DateTimePicker dateTimePicker = (System.Windows.Forms.DateTimePicker)calendarhost.Child;
            int year = dateTimePicker.Value.Year;
            int month = dateTimePicker.Value.Month;
            int day = dateTimePicker.Value.Day;
            if (!Validate())
            {
                return dateTime;
            }
            WindowsFormsHost hourhost = (WindowsFormsHost)this.HourContain.Children[0];
            System.Windows.Forms.TextBox hourText = (System.Windows.Forms.TextBox)hourhost.Child;
            int hour = Convert.ToInt32(hourText.Text);
            WindowsFormsHost minutehost = (WindowsFormsHost)this.MinuteContain.Children[0];
            System.Windows.Forms.TextBox minuteText = (System.Windows.Forms.TextBox)minutehost.Child;
            int minute = Convert.ToInt32(minuteText.Text);
            dateTime = new DateTime(year, month, day, hour, minute, 0);
            return dateTime;
        }

        public void SetTime(DateTime value)
        {
            WindowsFormsHost calendarhost = (WindowsFormsHost)this.CalendarContain.Children[0];
            System.Windows.Forms.DateTimePicker dateTimePicker = (System.Windows.Forms.DateTimePicker)calendarhost.Child;
            dateTimePicker.Value = value;
            WindowsFormsHost hourhost = (WindowsFormsHost)this.HourContain.Children[0];
            System.Windows.Forms.TextBox hourText = (System.Windows.Forms.TextBox)hourhost.Child;
            hourText.Text = value.Hour.ToString();
            WindowsFormsHost minutehost = (WindowsFormsHost)this.MinuteContain.Children[0];
            System.Windows.Forms.TextBox minuteText = (System.Windows.Forms.TextBox)minutehost.Child;
            minuteText.Text = value.Minute.ToString();
        }

    }
}
