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
using FoodSafetyMonitoring.Common;
using Toolkit = Microsoft.Windows.Controls;
using FoodSafetyMonitoring.Manager.UserControls;

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// CertificatePreview.xaml 的交互逻辑
    /// </summary>
    public partial class CertificateProductPreview : Window
    {
        public IDBOperation dbOperation = null;
        public string cardId;
        public CertificateProductPreview(IDBOperation dbOperation, string card_id)
        {
            InitializeComponent();
            this.dbOperation = dbOperation;
            this.cardId = card_id;

            DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_certificate_product_main('{0}')", cardId)).Tables[0];
            if (table.Rows.Count > 0)
            {
                _card_id.Text = table.Rows[0][0].ToString();
                _company.Text = table.Rows[0][2].ToString();
                _product_name.Text = table.Rows[0][4].ToString();
                _object_count.Text = table.Rows[0][5].ToString();
                _cz_cardid.Text = table.Rows[0][3].ToString();
                _product_area.Text = table.Rows[0][6].ToString();
                _dept_name.Text = table.Rows[0][7].ToString() + "              " + table.Rows[0][8].ToString();
                _mdd.Text = table.Rows[0][9].ToString();
                _bz.Text = table.Rows[0][10].ToString();
                _user_name.Text = table.Rows[0][11].ToString();
                _nian.Text = table.Rows[0][12].ToString();
                _yue.Text = table.Rows[0][13].ToString();
                _day.Text = table.Rows[0][14].ToString();
            }
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
