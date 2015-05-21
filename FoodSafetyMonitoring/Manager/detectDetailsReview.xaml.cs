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
    /// detectDetailsReview.xaml 的交互逻辑
    /// </summary>
    public partial class detectDetailsReview : Window
    {
        private IDBOperation dbOperation;
        private DbHelperMySQL dbHelper = null;
        int orderid ;

        public detectDetailsReview(IDBOperation dbOperation, int id)
        {
            InitializeComponent();

            this.dbOperation = dbOperation;
            dbHelper = DbHelperMySQL.CreateDbHelper();

            orderid = id;

            DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_detect_details('{0}')", id)).Tables[0];

            //给画面上的控件赋值
            _areaName.Text = table.Rows[0][10].ToString();
            _companyName.Text = table.Rows[0][11].ToString();
            _cardId.Text = table.Rows[0][12].ToString();
            _itemName.Text = table.Rows[0][3].ToString();
            _objectName.Text = table.Rows[0][4].ToString();
            _sampleName.Text = table.Rows[0][5].ToString();
            _reagentName.Text = table.Rows[0][7].ToString();
            _sensitivityName.Text = table.Rows[0][6].ToString();
            _resultName.Text = table.Rows[0][8].ToString();
            _deptName.Text = table.Rows[0][2].ToString();
            _detectDate.Text = table.Rows[0][1].ToString();
            _detectUserName.Text = table.Rows[0][9].ToString();
            _detectTypeName.Text = table.Rows[0][0].ToString();
            _reviewBz.Text = table.Rows[0][19].ToString();
            _reviewUserid.Text = table.Rows[0][14].ToString();
            _reviewReagent_text.Text = table.Rows[0][15].ToString();
            _reviewResult_text.Text = table.Rows[0][16].ToString();
            _reviewDate.Text = table.Rows[0][17].ToString();
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
