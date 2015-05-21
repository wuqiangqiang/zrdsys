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

namespace FoodSafetyMonitoring
{
    /// <summary>
    /// VerAniControl.xaml 的交互逻辑
    /// </summary>
    public partial class VerAniControl : UserControl
    {
        private System.Windows.Threading.DispatcherTimer dispatchTimer = null;

        Storyboard storyboard = null;

        public VerAniControl()
        {
            InitializeComponent();
            this.SizeChanged += new SizeChangedEventHandler(VerAniControl_SizeChanged);
            this.Loaded += new RoutedEventHandler(VerAniControl_Loaded);
        }

        void VerAniControl_Loaded(object sender, RoutedEventArgs e)
        {
            dispatchTimer = new System.Windows.Threading.DispatcherTimer();
            dispatchTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            dispatchTimer.Tick += new EventHandler(dispatchTimer_Tick);
        }

        void dispatchTimer_Tick(object sender, EventArgs e)
        {
            dispatchTimer.Stop();
            CompluterMoveParame();
        }

        private void CompluterMoveParame()
        {
            ListViewItem listviewitem = listView.ItemContainerGenerator.ContainerFromIndex(0) as ListViewItem;
            Border border = VisualTreeHelper.GetChild(listviewitem, 0) as Border;
            double height = canvas.ActualHeight - border.ActualHeight * listView.Items.Count;
            if (height > 0)
            {
                return;
            }
            //创建动画资源
            if (storyboard == null)
            {
                storyboard = new Storyboard();
            }
            else
            {
                storyboard.Stop();
                storyboard.Children.Clear();
            }
            //移动动画
            {
                DoubleAnimationUsingKeyFrames yAnimation = new DoubleAnimationUsingKeyFrames();
                Storyboard.SetTarget(yAnimation, listView);
                DependencyProperty[] propertyChain = new DependencyProperty[]
                {
                    ListBox.RenderTransformProperty,
                    TransformGroup.ChildrenProperty,
                    TranslateTransform.YProperty
                };
                Storyboard.SetTargetProperty(yAnimation, new PropertyPath("(0).(1)[3].(2)", propertyChain));
                yAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(canvas.Height, KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0))));
                yAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(height, KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 50))));
                storyboard.Children.Add(yAnimation);
            }
            storyboard.RepeatBehavior = RepeatBehavior.Forever;
            storyboard.Begin();
        }

        void VerAniControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            canvas.Width = MoveUserControl.ActualWidth;
            canvas.Clip = new RectangleGeometry(new Rect(0, 0, MoveUserControl.ActualWidth, MoveUserControl.ActualHeight));
        }

        public void SetTexts(List<MoveStringData> datas)
        {
            this.listView.ItemsSource = datas;
            if (datas.Count > 0)
            {
                CompluterMoveParame();
            }
            
        }




    }

    public class MoveStringData
    {
        private string txt = "";
        private Brush color = Brushes.Blue;

        private string department;
        public string Department
        {
            get { return this.department; }
        }
        private string personCount;
        public string PersonCount
        {
            get { return this.personCount; }
        }

        public MoveStringData(string txt, Brush color)
        {
            this.txt = txt;
            this.color = color;
        }

        public MoveStringData(string department, string personCount, Brush color)
        {
            this.department = department;
            this.personCount = personCount;
            this.color = color;
        }

        public string Text
        {
            get
            {
                return txt;
            }
            set
            {
                txt = value;
            }
        }

        public Brush FontColor
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
            }
        }
    }
}
