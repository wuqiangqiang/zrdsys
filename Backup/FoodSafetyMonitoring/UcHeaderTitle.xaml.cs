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
using System.Xml.Linq;

namespace FoodSafetyMonitoring
{
    /// <summary>
    /// UcHeaderTitle.xaml 的交互逻辑
    /// </summary>
    public partial class UcHeaderTitle : UserControl
    {
        private MainWindow mainWindow;
        private Rect rcnormal;//定义一个全局rect记录还原状态下窗口的位置和大小。

        public UcHeaderTitle(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            this.mainWindow.SizeChanged += new SizeChangedEventHandler(mainWindow_SizeChanged);
            //showMSMap = new Dictionary<string, MoveStringData>();
            //List<MoveStringData> datas = new List<MoveStringData>();
            //datas.Add(new MoveStringData("信息提示", Brushes.Blue));
            //datas.Add(new MoveStringData("信息提示", Brushes.Red));
            //datas.Add(new MoveStringData("信息提示", Brushes.Yellow));
            //datas.Add(new MoveStringData("信息提示", Brushes.Pink));
            //datas.Add(new MoveStringData("信息提示", Brushes.Orange));
            //datas.Add(new MoveStringData("信息提示", Brushes.Blue));
            //datas.Add(new MoveStringData("信息提示", Brushes.Red));
            //datas.Add(new MoveStringData("信息提示", Brushes.Yellow));
            //datas.Add(new MoveStringData("信息提示", Brushes.Pink));
            //datas.Add(new MoveStringData("信息提示", Brushes.Blue));
            //datas.Add(new MoveStringData("信息提示", Brushes.Red));
            //datas.Add(new MoveStringData("信息提示", Brushes.Pink));
            //datas.Add(new MoveStringData("信息提示", Brushes.Blue));
            //datas.Add(new MoveStringData("信息提示", Brushes.Red));
            //datas.Add(new MoveStringData("信息提示", Brushes.Yellow));
            //datas.Add(new MoveStringData("信息提示", Brushes.Pink));
            //aniControl.SetSpeed(100);
            //aniControl.SetText(datas);
          
        }

        void mainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (mainWindow.ActualHeight > SystemParameters.WorkArea.Height || mainWindow.ActualWidth > SystemParameters.WorkArea.Width)
            {
                mainWindow.WindowState = System.Windows.WindowState.Normal;
                max_MouseDown(null, null);
            }
        }
        //Dictionary<string, MoveStringData> showMSMap = null;
        //public bool isChange = false;
        //public void SetAlarmList(List<AlarmData> alarmDatas)
        //{
        //    //this.tbAlarmCount.Text = alarmDatas.Count.ToString();
        //    //List<MoveStringData> datas = new List<MoveStringData>();
        //    List<string> alarmIdList = new List<string>();
        //    foreach (AlarmData kv in alarmDatas)
        //    {
        //        alarmIdList.Add(kv.Alarmid);
        //        if (!showMSMap.ContainsKey(kv.Alarmid))
        //        {
        //            isChange = true;
        //            string showMsg = kv.Name + "在" + kv.Areaname + "于" + kv.Time + "发生" + kv.Alarmname + "    ";
        //            showMSMap.Add(kv.Alarmid, new MoveStringData(showMsg, Brushes.Red));
        //        }
        //    }

        //    foreach (string alarmId in showMSMap.Keys)
        //    {
        //        if (!alarmIdList.Contains(alarmId))
        //        {
        //            showMSMap.Remove(alarmId);
        //            isChange = true;
        //        }
        //    }
        //    if (isChange)
        //    {
        //        aniControl.SetText(showMSMap.Values.ToList());
        //        isChange = false;
        //    }
        //    this.tbAlarmCount.Text = showMSMap.Keys.Count.ToString();
        //}

        public void SetUserName(string name)
        {
            this._user.Text = name;
        }

        public void SetDate(DateTime date)
        {
            this._date.Text = date.ToString();
        }

        //public void UpdateTime()
        //{
        //    this.text_nowTime.Text = DateTime.Now.ToLongTimeString();
        //}

        private void exit_MouseDown(object sender, MouseButtonEventArgs e)
        {
            mainWindow.CloseWindow();
        }

        private void min_MouseDown(object sender, MouseButtonEventArgs e)
        {
            mainWindow.WindowState = WindowState.Minimized;
        }
 
        private void max_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (max.ToolTip.ToString() == "最大化")
            {
                MaxWindow();
            }
            else if (max.ToolTip.ToString() == "正常")
            {
                NormalWindow();
            }
        }

        private void MaxWindow()
        {
            max.ToolTip = "正常";
            rcnormal = new Rect(mainWindow.Left, mainWindow.Top, mainWindow.Width, mainWindow.Height);//保存下当前位置与大小
            mainWindow.Left = 0;//设置位置
            mainWindow.Top = 0;
            Rect rc = SystemParameters.WorkArea;//获取工作区大小
            mainWindow.Width = rc.Width;
            mainWindow.Height = rc.Height;

        }

        private void NormalWindow()
        {
            max.ToolTip = "最大化";
            mainWindow.Left = rcnormal.Left;
            mainWindow.Top = rcnormal.Top;
            mainWindow.Width = rcnormal.Width;
            mainWindow.Height = rcnormal.Height;
        }

        private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            mainWindow.Left += e.HorizontalChange;
            mainWindow.Top += e.VerticalChange;
        }
    }
}
