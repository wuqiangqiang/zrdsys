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
    /// UcCertificateDetails.xaml 的交互逻辑
    /// </summary>
    public partial class UcCertificateDetails : UserControl
    {
        List<string> Cer_details;
        public UcCertificateDetails(List<string> cer_details)
        {
            InitializeComponent();
            Cer_details = cer_details;

            _card_id.Text = Cer_details[0];
            _company.Text = Cer_details[1];
            _detect_object.Text = Cer_details[2];
            _object_count.Text = Cer_details[3] + Cer_details[4];
            _phone.Text = Cer_details[5];
            _for_use.Text = Cer_details[6];
            _city_ks.Text = Cer_details[7];
            _region_ks.Text = Cer_details[8];
            _town_ks.Text = Cer_details[9];
            _village_ks.Text = Cer_details[10];
            _city_js.Text = Cer_details[11];
            _region_js.Text = Cer_details[12];
            _town_js.Text = Cer_details[13];
            _village_js.Text = Cer_details[14];
            _object_lable.Text = Cer_details[15];
            _user_name.Text = Cer_details[16];
            _nian.Text = Cer_details[17];
            _yue.Text = Cer_details[18];
            _day.Text = Cer_details[19];
        }
    }
}
