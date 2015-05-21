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

namespace FoodSafetyMonitoring
{
    /// <summary>
    /// TitleBarControl.xaml 的交互逻辑
    /// </summary>
    public partial class TitleBarControl : Grid
    {
        public delegate void CloseFrameEventHandler();
        public event CloseFrameEventHandler CloseEvent;
        public delegate void MinFrameEventHandler();
        public event MinFrameEventHandler MinFrameEvent;
        public delegate void MaxFrameEventHandler();
        public event MaxFrameEventHandler MaxFrameEvent;
        private bool bMinium = false;
        public TitleBarControl()
        {
            InitializeComponent();
            this.BtnClose.Click += new RoutedEventHandler(BtnClose_Click);
           // this.BtnMinMax.Click += new RoutedEventHandler(BtnMinMax_Click);
            this.Loaded += new RoutedEventHandler(TitleBarControl_Loaded);
        }

        void TitleBarControl_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

        void BtnMinMax_Click(object sender, RoutedEventArgs e)
        {
            bMinium = !bMinium;
            if (bMinium)
            {
               // this.BtnMinMax.Background = new ImageBrush(((Image)FindResource("MaxImage")).Source);
                if (MinFrameEvent != null)
                {
                    MinFrameEvent();
                }
            }
            else
            {
               // this.BtnMinMax.Background = new ImageBrush(((Image)FindResource("MinImage")).Source);
                if (MaxFrameEvent != null)
                {
                    MaxFrameEvent();
                }
            }
        }

                  
        void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            if (CloseEvent != null)
            {
                CloseEvent();
            }
        }

        public string Title
        {
            set
            {
                this.TitleNameTextBlock.Text = value;
            }
        }

        public int TitleWidth
        {
            get
            {
                return (int)(TitleNameTextBlock.ActualWidth + BtnClose.ActualWidth * 2 + 2); ;
            }
        }

        public int TitleHeight
        {
            get
            {
                return (int)(TitleNameTextBlock.ActualHeight + 2); 
            }
        }

      


    }
}
