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

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// UcCertificateProductDetails.xaml 的交互逻辑
    /// </summary>
    public partial class UcCertificateProductDetails : UserControl
    {
        List<string> Cer_details;
        public UcCertificateProductDetails(List<string> cer_details)
        {
            InitializeComponent();
            Cer_details = cer_details;

            _card_id.Text = Cer_details[0];
            _company.Text = Cer_details[1];
            _product_name.Text = Cer_details[3];
            _object_count.Text = Cer_details[4] + Cer_details[5];
            _product_area.Text = Cer_details[6];
            _dept_name.Text = Cer_details[7];
            _dept_area.Text = Cer_details[8];
            _mdd.Text = Cer_details[9];
            _cz_cardid.Text = Cer_details[2];
            _bz.Text = Cer_details[10];
            _user_name.Text = Cer_details[11];
            _nian.Text = Cer_details[12];
            _yue.Text = Cer_details[13];
            _day.Text = Cer_details[14];
        }
    }
}
