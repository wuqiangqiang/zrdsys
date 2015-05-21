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
using System.ComponentModel;

namespace FoodSafetyMonitoring
{
    /// <summary>
    /// VerSplitControl.xaml 的交互逻辑
    /// </summary>
    public partial class SplitBarControl : UserControl
    {
        private bool bExpand = true;
        public delegate void ExpandEventHandler(bool bexpand);
        public event ExpandEventHandler ExpandEvent;
        public SplitBarControl()
        {
            InitializeComponent();
            this.BtnDir.MouseDown += new MouseButtonEventHandler(BtnDir_MouseDown);
            this.BtnDir.MouseEnter += new MouseEventHandler(BtnDir_MouseEnter);
            this.BtnDir.MouseLeave += new MouseEventHandler(BtnDir_MouseLeave);
        }

       public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(string), typeof(SplitBarControl),new PropertyMetadata("VER"));
       public static readonly DependencyProperty ControlWidthProperty = DependencyProperty.Register("ControlWidth",typeof(double),typeof(SplitBarControl),new PropertyMetadata(10.0));
       public static readonly DependencyProperty ControlHeightProperty = DependencyProperty.Register("ControlHeight", typeof(double), typeof(SplitBarControl), new PropertyMetadata(10.0));

       [TypeConverter(typeof(LengthConverter))]
       public double ControlWidth
       {
           get
           {
               return (double)base.GetValue(ControlWidthProperty);
           }
           set
           {
               base.SetValue(ControlWidthProperty, value);
           }

       }

       [TypeConverter(typeof(LengthConverter))]
       public double ControlHeight
       {
           get
           {
               return (double)base.GetValue(ControlHeightProperty);
           }
           set
           {
               base.SetValue(ControlHeightProperty,value);
           }
       }

        public string Orientation
        {
            get
            {
                return (string)base.GetValue(OrientationProperty);
            }
            set
            {
                base.SetValue(OrientationProperty, value);
                if (value == "VER")
                {
                    this.BackgoundImage = new ImageBrush(((Image)FindResource("VerSplitBackground")).Source);
                    Binding bindwidth = new Binding();
                    bindwidth.Source = this.ControlWidth;
                    bindwidth.Path =new PropertyPath(".");
                    this.ForegoundFillVer.SetBinding(WidthProperty, bindwidth);

                    Binding bindheight = new Binding();
                    bindheight.Source = this;
                    bindheight.Path =new PropertyPath( "ActualHeight");
                    this.ForegoundFillVer.SetBinding(HeightProperty, bindheight);

                    this.BtnDir.Source = ((Image)FindResource("DirLeft")).Source;
                }
                else if (value == "HOR")
                {
                    this.BackgoundImage = new ImageBrush(((Image)FindResource("HorSplitBackground")).Source);
                    Binding bindwidth = new Binding();
                    bindwidth.Source = this;
                    bindwidth.Path = new PropertyPath("ActualWidth");
                    this.ForegoundFillHor.SetBinding(WidthProperty, bindwidth);

                    Binding bindheight = new Binding();
                    bindheight.Source = this.ControlHeight;
                    bindheight.Path = new PropertyPath(".");
                    this.ForegoundFillHor.SetBinding(HeightProperty, bindheight);

                    this.BtnDir.Source = ((Image)FindResource("DirTop")).Source;
                }
            }
        }

        void BtnDir_MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.Orientation == "VER")
            {
                if (bExpand)
                {
                    BtnDir.Source = ((Image)FindResource("DirLeft")).Source;
                }
                else
                {
                    BtnDir.Source = ((Image)FindResource("DirRight")).Source;
                }
            }
            else if (this.Orientation == "HOR")
            {
                if (bExpand)
                {
                    BtnDir.Source = ((Image)FindResource("DirTop")).Source;
                }
                else
                {
                    BtnDir.Source = ((Image)FindResource("DirBottom")).Source;
                }
            }
        }

        void BtnDir_MouseEnter(object sender, MouseEventArgs e)
        {
            if (this.Orientation == "VER")
            {

                if (bExpand)
                {
                    BtnDir.Source = ((Image)FindResource("DirLeftSelect")).Source;
                }
                else
                {
                    BtnDir.Source = ((Image)FindResource("DirRightSelect")).Source;
                }
            }
            else if (this.Orientation == "HOR")
            {
                if (bExpand)
                {
                    BtnDir.Source = ((Image)FindResource("DirTopSelect")).Source;
                }
                else
                {
                    BtnDir.Source = ((Image)FindResource("DirBottomSelect")).Source;
                }
            }
        }

        void BtnDir_MouseDown(object sender, MouseButtonEventArgs e)
        {
            bExpand=!bExpand;
            if (ExpandEvent != null)
            {
                ExpandEvent(bExpand);
            }
        }
    }
}
