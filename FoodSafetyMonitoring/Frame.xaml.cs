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
using System.Windows.Media.Animation;

namespace FoodSafetyMonitoring
{
    /// <summary>
    /// Frame.xaml 的交互逻辑
    /// </summary>
    public partial class Frame : Window
    {
        private bool bminium = false;
        public delegate void MinMaxFrameEventHandler();
        public event MinMaxFrameEventHandler MinMaxFrameEvent;
        public delegate void CloseFrameEventHandler(string title);
        public event CloseFrameEventHandler CloseFrameEvent;
        private double frameWidth = 0;
        private double frameHeight = 0;
        private string title = "";
        private UserControl childControl;
        private double left = 0;
        private double top = 0;
        public delegate void FrameCloseEventHandler();
        public Frame(string title,UserControl control)
        {
            InitializeComponent();
            this.title = title;
            this.childControl = control;
            titlebar.Title = title;
            titlebar.CloseEvent += new TitleBarControl.CloseFrameEventHandler(titlebar_CloseEvent);
            titlebar.MinFrameEvent += new TitleBarControl.MinFrameEventHandler(titlebar_MinFrameEvent);
            titlebar.MaxFrameEvent += new TitleBarControl.MaxFrameEventHandler(titlebar_MaxFrameEvent);
            this.Top = 100;
            usercontrol.Children.Add(control);
            if (control is IFrameChild)
            {
                IFrameChild frameChild = (IFrameChild)control;
                frameChild.FrameCloseEvent += new FrameCloseEventHandler(frameChild_FrameCloseEvent);
            }
            this.Loaded += new RoutedEventHandler(Frame_Loaded);
            this.ShowInTaskbar = false;
            this.ResizeMode = ResizeMode.NoResize;
            this.Topmost = true;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        void frameChild_FrameCloseEvent()
        {
            if (CloseFrameEvent != null)
            {
                CloseFrameEvent(this.title);
            }
            this.Close();
        }

        public string FrameTitle
        {
            get
            {
                return title;
            }
        }

        public bool bMinium
        {
            get
            {
                return bminium;
            }
        }

        public int MiniumWidth
        {
            get
            {
                return titlebar.TitleWidth;
            }
        }

       
        void titlebar_MaxFrameEvent()
        {
            this.Topmost = true;
            MaxFrame();
        }

        public void MinFrame(double left,double top)
        {
            DoubleAnimation daWidth = new DoubleAnimation();
            daWidth.From = this.childControl.ActualWidth;
            daWidth.To = titlebar.TitleWidth;
            Duration durationWidth = new Duration(TimeSpan.FromMilliseconds(100));
            daWidth.Duration = durationWidth;

            DoubleAnimation daHeight = new DoubleAnimation();
            daHeight.From = this.childControl.ActualHeight;
            daHeight.To = 0;
            Duration durationHeight = new Duration(TimeSpan.FromMilliseconds(100));
            daHeight.Duration = durationHeight;

            DoubleAnimation daLeft = new DoubleAnimation();
            daLeft.From = this.Left;
            daLeft.To = left;
            Duration durationLeft = new Duration(TimeSpan.FromMilliseconds(100));
            daLeft.Duration = durationLeft;

            DoubleAnimation daTop = new DoubleAnimation();
            daTop.From = this.Top;
            daTop.To = top;
            Duration durationTop = new Duration(TimeSpan.FromMilliseconds(100));
            daTop.Duration = durationTop;

            this.childControl.BeginAnimation(WidthProperty, daWidth);
            this.childControl.BeginAnimation(HeightProperty, daHeight);

            this.BeginAnimation(LeftProperty, daLeft);
            this.BeginAnimation(TopProperty, daTop);
        }

        public void MaxFrame()
        {
            bminium = false;
            //childControl.Height = double.NaN;
            //childControl.Width = frameWidth;

            DoubleAnimation daWidth = new DoubleAnimation();
            daWidth.From = this.ActualWidth;
            daWidth.To = frameWidth;
            Duration durationWidth = new Duration(TimeSpan.FromMilliseconds(100));
            daWidth.Duration = durationWidth;

            DoubleAnimation daHeight = new DoubleAnimation();
            daHeight.From = 0;
            daHeight.To = frameHeight;
            Duration durationHeight = new Duration(TimeSpan.FromMilliseconds(100));
            daHeight.Duration = durationHeight;

            DoubleAnimation daLeft = new DoubleAnimation();
            daLeft.From = this.Left;
            daLeft.To = left;
            Duration durationLeft = new Duration(TimeSpan.FromMilliseconds(100));
            daLeft.Duration = durationLeft;

            DoubleAnimation daTop = new DoubleAnimation();
            daTop.From = this.Top;
            daTop.To = top;
            Duration durationTop = new Duration(TimeSpan.FromMilliseconds(100));
            daTop.Duration = durationTop;

            this.childControl.BeginAnimation(WidthProperty, daWidth);
            this.childControl.BeginAnimation(HeightProperty, daHeight);
                     
            this.BeginAnimation(LeftProperty, daLeft);
            this.BeginAnimation(TopProperty, daTop);

           
            // frameHeight //frameHeight;
            if (MinMaxFrameEvent != null)
            {
                MinMaxFrameEvent();
            }
            
        }

       

        void titlebar_MinFrameEvent()
        {
            bminium = true;
            //this.childControl.Width = titlebar.MinHeight;
            //this.childControl.Height = 0;
            this.left = this.Left;
            this.top = this.Top;
            //frameWidth = this.childControl.ActualWidth;
            //frameHeight = this.childControl.ActualHeight;
            if (this.MinMaxFrameEvent != null)
            {
                MinMaxFrameEvent();
            }
        }

       
        void titlebar_CloseEvent()
        {
            if (CloseFrameEvent != null)
            {
                CloseFrameEvent(this.title);
            }
            if (childControl is IFrameChild)
            {
                IFrameChild child = (IFrameChild)childControl;
                child.FrameClose();
            }
            this.Close();
            //DoubleAnimation rotation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
            //rotation.Completed += new EventHandler(rotation_Completed);
            //rotation.AutoReverse = false;
            //rotation.FillBehavior = FillBehavior.HoldEnd;
            //this.Background = Brushes.Transparent;
            //this.rotate.BeginAnimation(RotateTransform.AngleProperty, rotation, HandoffBehavior.SnapshotAndReplace);         
        }




        void Frame_Loaded(object sender, RoutedEventArgs e)
        {
            this.frameWidth = (int)this.ActualWidth;
            this.frameHeight = (int)this.ActualHeight;
          
        }

        private void titlebar_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed )  
            {  
               this.DragMove();  
            }  

        }

        private void titlebar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!bminium)
            {
                //this.left = this.Left;
                //this.top = this.Top;
            }
        }
    }
}
