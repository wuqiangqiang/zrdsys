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

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// UcCreateCertificate.xaml 的交互逻辑
    /// </summary>
    public partial class UcCreateCertificate : UserControl
    {
        DataTable ProvinceCityTable;
        public IDBOperation dbOperation = null;
        private Dictionary<string, MyColumn> MyColumns = new Dictionary<string, MyColumn>();
        string userId = (Application.Current.Resources["User"] as UserInfo).ID;
        private DataTable current_table;
        private List<string> selectdetect = new List<string>();

        public UcCreateCertificate(IDBOperation dbOperation)
        {
            InitializeComponent();
            this.dbOperation = dbOperation;

            ProvinceCityTable = Application.Current.Resources["省市表"] as DataTable;
            DataRow[] rows = ProvinceCityTable.Select("pid = '0001'");

            ComboboxTool.InitComboboxSource(_province, rows, "lr");
            _province.SelectionChanged += new SelectionChangedEventHandler(_province_SelectionChanged);
            //ComboboxTool.InitComboboxSource(_source_company, string.Format(" call p_provice_dept_hb('{0}','yz') ", userId), "lr");
            //ComboboxTool.InitComboboxSource(_source_company, "SELECT COMPANYID,COMPANYNAME FROM t_company", "lr");
            //_source_company.SelectionChanged += new SelectionChangedEventHandler(_source_company_SelectionChanged);
            //_cdatetime.Text = string.Format("{0:g}", System.DateTime.Now);
            //_cperson.Text = (Application.Current.Resources["User"] as UserInfo).ShowName;
            //_cdept.Text = dbOperation.GetDbHelper().GetSingle("SELECT INFO_NAME  from  sys_client_sysdept WHERE INFO_CODE = " + (Application.Current.Resources["User"] as UserInfo).DepartmentID).ToString();

        }

        void _province_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_province.SelectedIndex > 0)
            {
                DataRow[] rows = ProvinceCityTable.Select("pid = '" + (_province.SelectedItem as Label).Tag.ToString() + "'");
                ComboboxTool.InitComboboxSource(_city, rows, "lr");
                _city.SelectionChanged += new SelectionChangedEventHandler(_city_SelectionChanged);
            }
        }


        void _city_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_city.SelectedIndex > 0)
            {
                DataRow[] rows = ProvinceCityTable.Select("pid = '" + (_city.SelectedItem as Label).Tag.ToString() + "'");
                ComboboxTool.InitComboboxSource(_region, rows, "lr");
                _region.SelectionChanged += new SelectionChangedEventHandler(_region_SelectionChanged);
            }
        }

        void _region_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_region.SelectedIndex > 0)
            {
                ComboboxTool.InitComboboxSource(_source_company, "SELECT COMPANYID,COMPANYNAME FROM t_company where AREAID =" + (_region.SelectedItem as Label).Tag.ToString(), "lr");

            }
        }

        //void _source_company_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    //来源单位下拉选择的是有效内容，则将自动带出抽检信息
        //    if (_source_company.SelectedIndex >= 1)
        //    {
        //        //清空列表
        //        lvlist.DataContext = null;

        //        //根据条件查询出数据
        //        DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_certificate_main('{0}')", (_source_company.SelectedItem as Label).Tag.ToString())).Tables[0];
        //        current_table = table;
        //        lvlist.DataContext = table;
        //    }
        //}

        private void _query_Click(object sender, RoutedEventArgs e)
        {
            //if (_source_company.SelectedIndex <= 0)
            //{
            //    Toolkit.MessageBox.Show("被检单位不能为空", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
            //    return;
            //}

            //清空列表
            lvlist.DataContext = null;

            //根据条件查询出数据
            DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_certificate_main({0},'{1}')",
                                             userId, _source_company.SelectedIndex < 1 ? "" : (_source_company.SelectedItem as Label).Tag)).Tables[0];
            current_table = table;
            lvlist.DataContext = table;
        }

        //private void btnSave_Click(object sender, RoutedEventArgs e)
        //{
        //    if (selectdetect.Count > 0)
        //    {
        //        string msg = "";
        //        if (_object_count.Text.Trim().Length == 0)
        //        {
        //            msg = "*批次头数不能为空";
        //        }
        //        else if (_object_label.Text.Trim().Length == 0)
        //        {
        //            msg = "*耳标号不能为空";
        //        }
        //        else
        //        {
        //            //生成检疫证号
        //            string card_id = dbOperation.GetDbHelper().GetSingle(string.Format("select f_create_cardid('{0}')", (Application.Current.Resources["User"] as UserInfo).DepartmentID)).ToString();
        //            _cardId.Text = card_id;

        //            string detect_id = "";
        //            foreach (var detectid in selectdetect)
        //            {
        //                detect_id = detect_id + "," + detectid;
        //            }

        //            string sql = string.Format("call p_insert_certificate('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')"
        //                          , card_id, (_source_company.SelectedItem as Label).Tag.ToString(),
        //                          _object_count.Text, _object_label.Text,
        //                          (Application.Current.Resources["User"] as UserInfo).DepartmentID,
        //                          (Application.Current.Resources["User"] as UserInfo).ID,
        //                          System.DateTime.Now,
        //                          detect_id);


        //            int i = dbOperation.GetDbHelper().ExecuteSql(sql);
        //            if (i == 1)
        //            {
        //                Toolkit.MessageBox.Show("电子出证单生成成功！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
        //                clear();
        //            }
        //            else
        //            {
        //                Toolkit.MessageBox.Show("电子出证单生成失败！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
        //                return;
        //            }
        //        }
        //        txtMsg.Text = msg;
        //    }  
        //    else
        //    {
        //        Toolkit.MessageBox.Show("请先选择检测数据！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
        //        return;
        //    }
        //}

        private void _btn_details_Click(object sender, RoutedEventArgs e)
        {
            string batch_no = (sender as Button).Tag.ToString();

            grid_info.Children.Add(new UcCreateCertificatedetails(dbOperation, batch_no));

        }

        private void _btn_create_Click(object sender, RoutedEventArgs e)
        {
            //生成检疫证号
            string card_id = dbOperation.GetDbHelper().GetSingle(string.Format("select f_create_cardid('{0}')", (Application.Current.Resources["User"] as UserInfo).DepartmentID)).ToString();

            string batch_no = (sender as Button).Tag.ToString();

            string sql = string.Format("call p_insert_certificate('{0}','{1}','{2}','{3}','{4}','{5}')"
                            , card_id, batch_no,(_source_company.SelectedItem as Label).Tag.ToString(),
                            (Application.Current.Resources["User"] as UserInfo).DepartmentID,
                            (Application.Current.Resources["User"] as UserInfo).ID,
                            System.DateTime.Now);


            int i = dbOperation.GetDbHelper().ExecuteSql(sql);
            if (i == 1)
            {
                Toolkit.MessageBox.Show("电子出证单生成成功！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            else
            {
                Toolkit.MessageBox.Show("电子出证单生成失败！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
        }

        //private void _chk_Click(object sender, RoutedEventArgs e)
        //{
        //    CheckBox cb = sender as CheckBox;
        //    string detectorder = cb.Tag.ToString(); //获取该行detectid   
        //    if (cb.IsChecked == true)
        //    {
        //        selectdetect.Add(detectorder);  //如果选中就保存detectid   
        //    }
        //    else
        //    {
        //        selectdetect.Remove(detectorder);   //如果选中取消就删除里面的detectid   
        //    }  
        //}
        
    }
}
