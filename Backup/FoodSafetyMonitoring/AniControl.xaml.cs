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
    /// AniControl.xaml 的交互逻辑
    /// </summary>
    public partial class AniControl : UserControl
    {
        private int speed = 10;
        public AniControl()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(AniControl_Loaded);
            this.SizeChanged += new SizeChangedEventHandler(AniControl_SizeChanged);
        }

        void AniControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            canvas.Width = MoveUserControl.ActualWidth;         
            canvas.Clip = new RectangleGeometry(new Rect(0, 0, MoveUserControl.ActualWidth, MoveUserControl.ActualHeight));
            Reset();
        }

        void AniControl_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

        private void SetTextBlockStyle(TextBlock textBlock)
        {
            textBlock.FontFamily = new FontFamily("微软雅黑");
            textBlock.Foreground = Brushes.Blue;
            textBlock.FontSize = 16;
            textBlock.Background = Brushes.Transparent;
        }



        //获取文字长度
        private double MeasureTextWidth(string text, double fontSize, string fontFamily)
        {
            FormattedText formattedText = new FormattedText(
                    text,
                    System.Globalization.CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    new Typeface(fontFamily.ToString()),
                    fontSize,
                    Brushes.Black
            );
            return formattedText.WidthIncludingTrailingWhitespace;
        }

        public void SetSpeed(int speed)
        {
            this.speed = speed;
        }

        public void SetText(string text)
        {
            if (dockPanel.Children.Count > 1)
            {
                dockPanel.Children.Clear();
            }
            if (dockPanel.Children.Count == 0)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.Text = text;
                SetTextBlockStyle(textBlock);
                dockPanel.Children.Add(textBlock);
            }
            ((TextBlock)dockPanel.Children[0]).Text = text;
            Start();          
        }

        public void SetText(List<MoveStringData> datas)
        {          
            dockPanel.Children.Clear();
            foreach (MoveStringData kv in datas)
            {
                TextBlock textBlock = new TextBlock();
                SetTextBlockStyle(textBlock);
                textBlock.Text = kv.Text+" ";
                textBlock.Foreground = kv.FontColor;
                dockPanel.Children.Add(textBlock);
            }
            Reset();
        }

        private void Reset()
        {

            double length = 0;
            foreach (UIElement kv in dockPanel.Children)
            {
                TextBlock textBlock = (TextBlock)kv;
                length += MeasureTextWidth(textBlock.Text, textBlock.FontSize, textBlock.FontFamily.Source);
            }
            if (length > canvas.Width)
            {
                Start();
            }
            else if (storyboard != null)
            {
                storyboard.Remove();
                storyboard = null;
            }
        }
      
        Storyboard storyboard = null;
        private void Start()
        {
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
            double length = 0;
            foreach (UIElement kv in dockPanel.Children)
            {
                TextBlock textBlock = (TextBlock)kv;
                length+=MeasureTextWidth(textBlock.Text, textBlock.FontSize, textBlock.FontFamily.Source);
            }
            DoubleAnimationUsingKeyFrames WidthMove = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTarget(WidthMove, dockPanel);
            DependencyProperty[] propertyChain = new DependencyProperty[]
                {
                    TextBlock.RenderTransformProperty,
                    TransformGroup.ChildrenProperty,
                    TranslateTransform.XProperty,
                };
            Storyboard.SetTargetProperty(WidthMove, new PropertyPath("(0).(1)[3].(2)", propertyChain));
            WidthMove.KeyFrames.Add(new LinearDoubleKeyFrame(canvas.Width, KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0))));
            WidthMove.KeyFrames.Add(new LinearDoubleKeyFrame(-length, KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, speed))));
            storyboard.Children.Add(WidthMove);
            storyboard.RepeatBehavior = RepeatBehavior.Forever;
            storyboard.Begin();
        }
    }
}
