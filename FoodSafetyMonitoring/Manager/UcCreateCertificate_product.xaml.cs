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
using FoodSafetyMonitoring.dao;
using System.Data;
using FoodSafetyMonitoring.Common;
using Toolkit = Microsoft.Windows.Controls;
using FoodSafetyMonitoring.Manager.UserControls;
using System.Printing;

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// UcCreateCertificate_product.xaml 的交互逻辑
    /// </summary>
    public partial class UcCreateCertificate_product : UserControl
    {
        public IDBOperation dbOperation = null;
        private string company_id;
        private string dept_name;
        private string dept_area;
        string userId = (Application.Current.Resources["User"] as UserInfo).ID;
        string username = (Application.Current.Resources["User"] as UserInfo).ShowName;


        public UcCreateCertificate_product(IDBOperation dbOperation)
        {
            InitializeComponent();

            this.dbOperation = dbOperation;

            ComboboxTool.InitComboboxSource(_source_company, string.Format(" call p_user_company_product_wcz('{0}') ", userId), "lr");
        }

        private void _query_Click(object sender, RoutedEventArgs e)
        {
            if (_source_company.SelectedIndex == 0)
            {
                Toolkit.MessageBox.Show("货主不能为空", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            //根据条件查询出数据
            DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_create_certificate_product({0},'{1}')",
                                             userId, _source_company.SelectedIndex < 1 ? "" : (_source_company.SelectedItem as Label).Tag)).Tables[0];
            if (table.Rows.Count == 0)
            {
                Toolkit.MessageBox.Show("该货主还未做过屠宰检测！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            else
            {
                company_id = (_source_company.SelectedItem as Label).Tag.ToString();
                _company.Text = table.Rows[0][4].ToString();
                _product_name.Text = table.Rows[0][3].ToString()+"胴体";
                _object_count.Text = table.Rows[0][1].ToString();
                _object_type.Text = "头";
                _cz_cardid.Text = table.Rows[0][0].ToString();
                _product_area.Text = table.Rows[0][7].ToString();
                _dept_name.Text = table.Rows[0][5].ToString() + "              " +table.Rows[0][6].ToString();
                dept_name = table.Rows[0][5].ToString();
                dept_area = table.Rows[0][6].ToString();
                _user_name.Text = username;
                _nian.Text = DateTime.Now.Year.ToString();
                _yue.Text = DateTime.Now.Month.ToString();
                _day.Text = DateTime.Now.Day.ToString();

            }
        }

        private void _create_Click(object sender, RoutedEventArgs e)
        {
            if (_card_id.Text.Trim().Length == 0)
            {
                Toolkit.MessageBox.Show("请输入检疫证号！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            bool exit_flag = dbOperation.GetDbHelper().Exists(string.Format("SELECT count(cardid) from t_certificate_product where productcardid ='{0}'", _card_id.Text));
            if (exit_flag)
            {
                Toolkit.MessageBox.Show("检疫证号已存在，请重新输入！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (_company.Text.Trim().Length == 0)
            {
                Toolkit.MessageBox.Show("请输入货主！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (_product_name.Text.Trim().Length == 0)
            {
                Toolkit.MessageBox.Show("请输入产品名称！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (_object_count.Text.Trim().Length == 0)
            {
                Toolkit.MessageBox.Show("请输入数量及单位！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (_product_area.Text.Trim().Length == 0)
            {
                Toolkit.MessageBox.Show("请输入产地！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (_dept_name.Text.Trim().Length == 0)
            {
                Toolkit.MessageBox.Show("请输入生产单位名称地址！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (_mdd.Text.Trim().Length == 0)
            {
                Toolkit.MessageBox.Show("请输入目的地！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (_cz_cardid.Text.Trim().Length == 0)
            {
                Toolkit.MessageBox.Show("请输入检疫标志号！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string sql = string.Format("call p_insert_certificate_product('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}')"
                            , _card_id.Text, company_id, _company.Text, _cz_cardid.Text, _product_name.Text, _object_count.Text + _object_type.Text, _product_area.Text,
                            dept_name, dept_area, _mdd.Text, _bz.Text,
                            (Application.Current.Resources["User"] as UserInfo).DepartmentID,
                            (Application.Current.Resources["User"] as UserInfo).ID,
                            System.DateTime.Now);

            int i = dbOperation.GetDbHelper().ExecuteSql(sql);
            if (i >= 0)
            {
                List<string> cer_details = new List<string>() {_card_id.Text,_company.Text,_cz_cardid.Text, _product_name.Text, _object_count.Text ,
                             _object_type.Text, _product_area.Text,dept_name, dept_area, _mdd.Text, _bz.Text,username,
                            System.DateTime.Now.Year.ToString(),System.DateTime.Now.Month.ToString(),System.DateTime.Now.Day.ToString() };

                UcCertificateProductDetails cer = new UcCertificateProductDetails(cer_details);

                PrintDialog dialog = new PrintDialog();
                if (dialog.ShowDialog() == true)
                {
                    Size printSize = new Size(dialog.PrintableAreaWidth, dialog.PrintableAreaHeight);
                    cer.Measure(printSize);
                    cer.Arrange(new Rect(0, 0, dialog.PrintableAreaWidth, dialog.PrintableAreaHeight));

                    dialog.PrintVisual(cer, "Print Test");
                }

                //Toolkit.MessageBox.Show("电子出证单生成成功！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                clear();
                return;
            }
            else
            {
                Toolkit.MessageBox.Show("电子出证单生成失败！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
        }

        private void clear()
        {
            ComboboxTool.InitComboboxSource(_source_company, string.Format(" call p_user_company_product_wcz('{0}') ", userId), "lr");
            _card_id.Text = "";
            _company.Text = "";
            _cz_cardid.Text = "";
            _object_count.Text = "";
            _product_name.Text = "";
            _dept_name.Text = "";
            _object_type.Text = "";
            _product_area.Text = "";
            _mdd.Text = "";
            _bz.Text = "";
        }

        private void _print_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
