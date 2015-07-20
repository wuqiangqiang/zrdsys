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
        public IDBOperation dbOperation = null;
        private Dictionary<string, MyColumn> MyColumns = new Dictionary<string, MyColumn>();
        string userId = (Application.Current.Resources["User"] as UserInfo).ID;
        private DataTable current_table;

        public UcCreateCertificate(IDBOperation dbOperation)
        {
            InitializeComponent();
            this.dbOperation = dbOperation;
            ComboboxTool.InitComboboxSource(_source_company, string.Format(" call p_user_dept('{0}') ", userId), "lr");
            _source_company.SelectionChanged += new SelectionChangedEventHandler(_source_company_SelectionChanged);
        }

        void _source_company_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //来源单位下拉选择的是有效内容，则将自动带出抽检信息
            if (_source_company.SelectedIndex >= 1)
            {
                //清空列表
                lvlist.DataContext = null;

                //根据条件查询出数据
                DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_certificate_main('{0}')", (_source_company.SelectedItem as Label).Tag.ToString())).Tables[0];
                current_table = table;
                lvlist.DataContext = table;
            }
        }

        private void Object_Lable_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void Object_Count_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!isNumberic(text))
                { e.CancelCommand(); }
            }
            else { e.CancelCommand(); }
        }

        private void Object_Count_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void Object_Count_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!isNumberic(e.Text))
            {
                e.Handled = true;
            }
            else
                e.Handled = false;
        }

        //isDigit是否是数字
        public static bool isNumberic(string _string)
        {
            if (string.IsNullOrEmpty(_string))

                return false;
            foreach (char c in _string)
            {
                if (!char.IsDigit(c))
                    //if(c<'0' c="">'9')//最好的方法,在下面测试数据中再加一个0，然后这种方法效率会搞10毫秒左右
                    return false;
            }
            return true;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            foreach (var row in lvlist.Items)
            {
                //CheckBox chkTemp = (CheckBox)row.FindControl("_chk");
                //if (chkTemp != null)
                //{
                //    if (chkTemp.IsChecked == true)
                //    {
                //    }
                //}
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {

        }

        private void _btn_details_Click(object sender, RoutedEventArgs e)
        {
            int zj = int.Parse((sender as Button).Tag.ToString());
            DataRow[] rows = current_table.Select("ZJ ="+ zj);
            string company_id = rows[0]["COMPANYID"].ToString();
            string batch_no = rows[0]["batchno"].ToString();
            string item_id = rows[0]["ItemID"].ToString();
            string dept_id = rows[0]["DeptId"].ToString();
            string result_id = rows[0]["RESULTID"].ToString();

            grid_info.Children.Add(new UcCreateCertificatedetails(dbOperation, company_id, batch_no, item_id, dept_id,result_id));

        }

        
    }
}
