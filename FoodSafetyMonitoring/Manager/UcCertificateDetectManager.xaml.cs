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
using DBUtility;
using FoodSafetyMonitoring.Manager.UserControls;

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// UcCertificateDetectManager.xaml 的交互逻辑
    /// </summary>
    public partial class UcCertificateDetectManager : UserControl
    {
        private IDBOperation dbOperation;
        string userId = (Application.Current.Resources["User"] as UserInfo).ID;
        string supplierId = (Application.Current.Resources["User"] as UserInfo).SupplierId;
        public UcCertificateDetectManager(IDBOperation dbOperation)
        {
            InitializeComponent();
            this.dbOperation = dbOperation;

            ComboboxTool.InitComboboxSource(_source_company, string.Format("call p_user_dept('{0}')", userId), "lr");
            _source_company.SelectionChanged += new SelectionChangedEventHandler(_source_company_SelectionChanged);
            ComboboxTool.InitComboboxSource(_detect_trade, "select tradeId,tradeName from t_trade where openFlag = '1'", "lr");
            _detect_trade.SelectionChanged += new SelectionChangedEventHandler(_detect_trade_SelectionChanged);
            _detect_trade.SelectedIndex = 1;

            ComboboxTool.InitComboboxSource(_detect_result, "SELECT resultId,resultName FROM t_det_result where openFlag = '1' ORDER BY id", "lr");
            _entering_datetime.Text = string.Format("{0:g}", System.DateTime.Now);
            _detect_person.Text = (Application.Current.Resources["User"] as UserInfo).ShowName;
            _detect_site.Text = dbOperation.GetDbHelper().GetSingle("SELECT INFO_NAME  from  sys_client_sysdept WHERE INFO_CODE = " + (Application.Current.Resources["User"] as UserInfo).DepartmentID).ToString();

        }
        private void clear()
        {
            this._province.Text = "";
            this._city.Text = "";
            this._region.Text = "";
            this._source_company.SelectedIndex = 0;
            this._phone_number.Text = "";
            this._object_count.Text = "";
            this._batch_number.Text = "";
            this._detect_trade.SelectedIndex = 1;
            this._detect_item.SelectedIndex = 0;
            this._detect_method1.IsChecked = false;
            this._detect_method2.IsChecked = false;
            this._detect_method3.IsChecked = false;
            this._detect_object.SelectedIndex = 0;
            this._detect_sample.SelectedIndex = 0;
            this._detect_sensitivity.SelectedIndex = 0;
            this._detect_result.SelectedIndex = 0;
            this._entering_datetime.Text = string.Format("{0:g}", System.DateTime.Now);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string msg = "";
            if (_source_company.SelectedIndex == 0 || _source_company.Text == "")
            {
                msg = "*请选择来源单位";
            }
            //else if (_phone_number.Text.Trim().Length == 0)
            //{
            //    msg = "*联系电话不能为空";
            //}
            else if (_province.Text.Trim().Length == 0)
            {
                msg = "*省不能为空";
            }
            else if (_city.Text.Trim().Length == 0)
            {
                msg = "*市不能为空";
            }
            else if (_region.Text.Trim().Length == 0)
            {
                msg = "*区不能为空";
            }
            else if (_object_count.Text.Trim().Length == 0)
            {
                msg = "*批次头数不能为空";
            }
            else if (_detect_item.SelectedIndex < 1)
            {
                msg = "*请选择检查项目";
            }
            else if (_detect_method1.IsChecked != true && _detect_method2.IsChecked != true && _detect_method3.IsChecked != true)
            {
                msg = "*请选择检测方法";
            }
            else if (_detect_object.SelectedIndex < 1)
            {
                msg = "*请选择检测对象";
            }
            else if (_detect_sample.SelectedIndex < 1)
            {
                msg = "*请选择检测样本";
            }
            else if (_detect_sensitivity.SelectedIndex < 1)
            {
                msg = "*请选择检测灵敏度";
            }
            else if (_detect_result.SelectedIndex < 1)
            {
                msg = "*请选择检测结果";
            }
            else if (_detect_person.Text.Trim().Length == 0)
            {
                msg = "*请输入检测师";
            }
            else
            {
                //批次编码为空时，自动生成编码
                if(_batch_number.Text == "")
                {
                    string batch_no = dbOperation.GetDbHelper().GetSingle("select f_create_batchno()").ToString();
                    _batch_number.Text = batch_no;
                }
                string sql = string.Format("call p_insert_certificate_detect('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}')"
                              , (_source_company.SelectedItem as Label).Tag.ToString(),
                              _batch_number.Text,
                              (_detect_item.SelectedItem as Label).Tag.ToString(),
                              (_detect_method1.IsChecked == true ? 1 : 0) + (_detect_method2.IsChecked == true ? 2 : 0) + (_detect_method3.IsChecked == true ? 3 : 0),
                              (_detect_object.SelectedItem as Label).Tag.ToString(),
                              (_detect_sample.SelectedItem as Label).Tag.ToString(),
                              (_detect_sensitivity.SelectedItem as Label).Tag.ToString(),
                              (_detect_result.SelectedItem as Label).Tag.ToString(),
                              (Application.Current.Resources["User"] as UserInfo).DepartmentID,
                              (Application.Current.Resources["User"] as UserInfo).ID,
                              System.DateTime.Now, _object_count.Text);


                int i = dbOperation.GetDbHelper().ExecuteSql(sql);
                if (i == 1)
                {
                    //Toolkit.MessageBox.Show("添加成功！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    if (Toolkit.MessageBox.Show("添加成功！批次编码为(" + _batch_number.Text + ")，是否继续录入该批次的检测单?", "系统询问", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        this._detect_trade.SelectedIndex = 1;
                        this._detect_item.SelectedIndex = 0;
                        this._detect_method1.IsChecked = false;
                        this._detect_method2.IsChecked = false;
                        this._detect_method3.IsChecked = false;
                        this._detect_object.SelectedIndex = 0;
                        this._detect_sample.SelectedIndex = 0;
                        this._detect_sensitivity.SelectedIndex = 0;
                        this._detect_result.SelectedIndex = 0;
                        this._entering_datetime.Text = string.Format("{0:g}", System.DateTime.Now);
                    }
                    else
                    {
                        clear();
                    }
                    //ComboboxTool.InitComboboxSource(_source_company, string.Format("call p_user_dept('{0}')", userId), "lr");
                }
                else
                {
                    Toolkit.MessageBox.Show("添加失败！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
            }

            txtMsg.Text = msg;

        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            clear();
        }

        private void _detect_method1_Checked(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).Name == "_detect_method1")
            {
                _detect_method2.IsChecked = false;
                _detect_method3.IsChecked = false;
            }
            else if ((sender as CheckBox).Name == "_detect_method2")
            {
                _detect_method1.IsChecked = false;
                _detect_method3.IsChecked = false;
            }
            else if ((sender as CheckBox).Name == "_detect_method3")
            {
                _detect_method1.IsChecked = false;
                _detect_method2.IsChecked = false;
            }
        }


        void _source_company_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //来源单位下拉选择的是有效内容，则将省市区自动赋值
            if (_source_company.SelectedIndex >= 1)
            {
                DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_dept_details('{0}')", (_source_company.SelectedItem as Label).Tag.ToString())).Tables[0];

                _province.Text =table.Rows[0][1].ToString();
                _city.Text = table.Rows[0][3].ToString();
                _region.Text = table.Rows[0][5].ToString();
                _phone_number.Text = table.Rows[0][6].ToString();

            }
        }

        void _detect_trade_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_detect_trade.SelectedIndex > 0)
            {
                //ComboboxTool.InitComboboxSource(_detect_item, string.Format("SELECT itemid,ItemNAME FROM v_user_item WHERE userid = '{0}' and (tradeId ='{1}'or ifnull(tradeId,'') = '')", userId, (_detect_trade.SelectedItem as Label).Tag), "lr");
                ComboboxTool.InitComboboxSource(_detect_item, string.Format("SELECT ItemID,ItemNAME FROM t_det_item WHERE  (tradeId ='{0}'or ifnull(tradeId,'') = '') and OPENFLAG = '1' order by orderId", (_detect_trade.SelectedItem as Label).Tag), "lr");
                _detect_item.SelectionChanged += new SelectionChangedEventHandler(_detect_item_SelectionChanged);
            }
            //else
            //{
            //    _detect_item.Items.Clear();
            //    _detect_object.Items.Clear();
            //    _detect_sample.Items.Clear();
            //    _detect_sensitivity.Items.Clear();
            //}
        }

        void _detect_item_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_detect_item.SelectedIndex > 0)
            {
                //ComboboxTool.InitComboboxSource(_detect_object, string.Format("call p_detect_object( '{0}','{1}')", userId, (_detect_item.SelectedItem as Label).Tag), "lr");
                ComboboxTool.InitComboboxSource(_detect_object, string.Format("SELECT objectId,objectName FROM v_item_object WHERE itemId = '{0}' and tradeId = '{1}'", (_detect_item.SelectedItem as Label).Tag, (_detect_trade.SelectedItem as Label).Tag), "lr");
                _detect_object.SelectionChanged += new SelectionChangedEventHandler(_detect_object_SelectionChanged);
            }
        }

        void _detect_object_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_detect_object.SelectedIndex > 0)
            {
                //ComboboxTool.InitComboboxSource(_detect_sample, string.Format("call p_detect_sample( '{0}','{1}')", userId, (_detect_object.SelectedItem as Label).Tag), "lr");
                ComboboxTool.InitComboboxSource(_detect_sample, string.Format("SELECT sampleId,sampleName FROM v_object_sample WHERE objectId = '{0}'", (_detect_object.SelectedItem as Label).Tag), "lr");
                _detect_sample.SelectionChanged += new SelectionChangedEventHandler(_detect_sensitivity_SelectionChanged);
            }
        }

        void _detect_sensitivity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_detect_sample.SelectedIndex > 0)
            {
                ComboboxTool.InitComboboxSource(_detect_sensitivity, string.Format("call p_detect_sensitivity( '{0}','{1}')", (_detect_item.SelectedItem as Label).Tag, (_detect_object.SelectedItem as Label).Tag), "lr");
            }
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
    }
}