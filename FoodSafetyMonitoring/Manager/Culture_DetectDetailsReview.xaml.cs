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
using FoodSafetyMonitoring.dao;
using System.Data;
using DBUtility;
using FoodSafetyMonitoring.Common;
using Toolkit = Microsoft.Windows.Controls;

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// Culture_DetectDetailsReview.xaml 的交互逻辑
    /// </summary>
    public partial class Culture_DetectDetailsReview : Window
    {
        private IDBOperation dbOperation;
        private DbHelperMySQL dbHelper = null;
        int orderid;

        public Culture_DetectDetailsReview(IDBOperation dbOperation, int id)
        {
            InitializeComponent();

            this.dbOperation = dbOperation;
            dbHelper = DbHelperMySQL.CreateDbHelper();

            orderid = id;

            DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_culture_detect_details('{0}')", id)).Tables[0];

            //给画面上的控件赋值
            //图片地址改为从数据库中获取
            string picture_url = dbOperation.GetDbHelper().GetSingle("select pictureurl from t_url ").ToString();
            if (picture_url == "")
            {
                picture_url = "http://www.zrodo.com:8080/xmjc/";
            }
            _img.Source = new BitmapImage(new Uri(picture_url + table.Rows[0][17].ToString()));
            // _img.Source = new BitmapImage(new Uri("http://www.zrodo.com:8080/xmjc/" + table.Rows[0][20].ToString()));
            _culture_fileno.Text = table.Rows[0][10].ToString();
            _itemName.Text = table.Rows[0][3].ToString();
            _objectName.Text = table.Rows[0][4].ToString();
            _sampleName.Text = table.Rows[0][5].ToString();
            _reangetName.Text = table.Rows[0][7].ToString();
            _sensitivityName.Text = table.Rows[0][6].ToString();
            _resultName.Text = table.Rows[0][8].ToString();
            _cardbrand.Text = table.Rows[0][18].ToString();
            _deptName.Text = table.Rows[0][2].ToString();
            _detectDate.Text = table.Rows[0][1].ToString();
            _detectUserName.Text = table.Rows[0][9].ToString();
            _detectTypeName.Text = table.Rows[0][0].ToString();
            _reviewBz.Text = table.Rows[0][16].ToString();
            _reviewUserid.Text = table.Rows[0][11].ToString();
            _reviewReagent_text.Text = table.Rows[0][12].ToString();
            _reviewResult_text.Text = table.Rows[0][13].ToString();
            _reviewDate.Text = table.Rows[0][14].ToString();
        }

        private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            this.Left += e.HorizontalChange;
            this.Top += e.VerticalChange;
        }

        private void exit_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.Close();
            }
        }

        private void exit_MouseEnter(object sender, MouseEventArgs e)
        {
            exit.Source = new BitmapImage(new Uri("pack://application:,," + "/res/close_on.png"));
        }

        private void exit_MouseLeave(object sender, MouseEventArgs e)
        {
            exit.Source = new BitmapImage(new Uri("pack://application:,," + "/res/close.png"));
        }
    }
}