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
    /// UcDetectBillManager.xaml 的交互逻辑
    /// </summary>
    public partial class UcDetectBillManager : UserControl
    {
        DataTable ProvinceCityTable;
        DbHelperMySQL dbOperation;

        string userId = (Application.Current.Resources["User"] as UserInfo).ID;
        string supplierId = (Application.Current.Resources["User"] as UserInfo).SupplierId;

        public UcDetectBillManager()
        {
            InitializeComponent();
            dbOperation = DBUtility.DbHelperMySQL.CreateDbHelper();
            ProvinceCityTable = Application.Current.Resources["省市表"] as DataTable;
            DataRow[] rows = ProvinceCityTable.Select("pid = '0001'");

            //画面初始化-新增检测单画面
            ComboboxTool.InitComboboxSource(_province, rows,"lr");
            _province.SelectionChanged += new SelectionChangedEventHandler(_province_SelectionChanged);

            ComboboxTool.InitComboboxSource(_source_company, "SELECT COMPANYID,COMPANYNAME FROM v_user_company WHERE userid =  " + userId,"lr");
            if (supplierId == "nkrx")
            {
                ComboboxTool.InitComboboxSource(_detect_trade, "select tradeId,tradeName from t_trade where openFlag = '1' order by orderId", "lr");
            }
            else
            {
                ComboboxTool.InitComboboxSource(_detect_trade, "select tradeId,tradeName from t_trade where openFlag = '1'", "lr");
            }
            _detect_trade.SelectionChanged += new SelectionChangedEventHandler(_detect_trade_SelectionChanged);
            //ComboboxTool.InitComboboxSource(_detect_item, "SELECT itemid,ItemNAME FROM v_user_item WHERE userid = " + userId);
            //ComboboxTool.InitComboboxSource(_detect_object, " SELECT objectId,objectName FROM v_user_object WHERE userid = " + userId);
            //ComboboxTool.InitComboboxSource(_detect_sample, "  SELECT sampleId,sampleName FROM v_user_sample WHERE userid = " + userId);
            //ComboboxTool.InitComboboxSource(_detect_sensitivity, "SELECT sensitivityId,sensitivityName FROM t_det_sensitivity where openFlag = '1'", "lr");
            ComboboxTool.InitComboboxSource(_detect_result, "SELECT resultId,resultName FROM t_det_result where openFlag = '1' ORDER BY id", "lr");
            _entering_datetime.Text = string.Format("{0:g}", System.DateTime.Now);
            _source_company.SelectionChanged += new SelectionChangedEventHandler(_source_company_SelectionChanged);
            _detect_person.Text = (Application.Current.Resources["User"] as UserInfo).ShowName;
            _detect_site.Text = dbOperation.GetSingle("SELECT INFO_NAME  from  sys_client_sysdept WHERE INFO_CODE = " + (Application.Current.Resources["User"] as UserInfo).DepartmentID).ToString();

        }

        private void clear()
        {
            this._province.SelectedIndex = 0;
            this._city.SelectedIndex = 0;
            this._region.SelectedIndex = 0;
            this._source_company.SelectedIndex = 0;
            this._detect_number.Text = "";
            this._detect_trade.SelectedIndex = 0;
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
            if (_province.SelectedIndex < 1)
            {
                msg = "*请选择省";
            }
            else if (_city.SelectedIndex < 1)
            {
                msg = "*请选择市";
            }
            else if (_region.SelectedIndex < 1)
            {
                msg = "*请选择区";
            }
            else if (_source_company.SelectedIndex == 0 || _source_company.Text == "")
            {
                msg = "*请选择来源单位";
            }
            else if (_detect_number.Text.Trim().Length == 0)
            {
                msg = "*检疫证号不能为空";
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
                string company_id;

                //判断来源单位是否存在，若不存在则插入数据库
                bool exit_flag = dbOperation.Exists(string.Format("SELECT count(COMPANYID) from t_company where COMPANYNAME ='{0}' and deptid = '{1}'", _source_company.Text, (Application.Current.Resources["User"] as UserInfo).DepartmentID));
                if (!exit_flag)
                {
                    int n = dbOperation.ExecuteSql(string.Format("INSERT INTO t_company (COMPANYNAME,AREAID,OPENFLAG,deptid,cuserid,cdate) VALUES('{0}','{1}','{2}','{3}','{4}','{5}')",
                                                                  _source_company.Text,
                                                                  (_region.SelectedItem as Label).Tag.ToString(),
                                                                  '1', (Application.Current.Resources["User"] as UserInfo).DepartmentID,
                                                                  (Application.Current.Resources["User"] as UserInfo).ID, DateTime.Now));
                    if (n == 1)
                    {
                        company_id = dbOperation.GetSingle(string.Format("SELECT COMPANYID from t_company where COMPANYNAME ='{0}' and deptid = '{1}'", _source_company.Text, (Application.Current.Resources["User"] as UserInfo).DepartmentID)).ToString();
                    }
                    else
                    {
                        Toolkit.MessageBox.Show("来源单位添加失败！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                }
                else
                {
                    company_id = dbOperation.GetSingle(string.Format("SELECT COMPANYID from t_company where COMPANYNAME ='{0}' and deptid = '{1}'", _source_company.Text, (Application.Current.Resources["User"] as UserInfo).DepartmentID)).ToString();
                }

                string sql = string.Format("call p_insert_detect('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}')"
                              , company_id,
                              _detect_number.Text,
                              (_detect_item.SelectedItem as Label).Tag.ToString(),
                              (_detect_method1.IsChecked == true ? 1 : 0) + (_detect_method2.IsChecked == true ? 2 : 0) + (_detect_method3.IsChecked == true ? 3 : 0),
                              (_detect_object.SelectedItem as Label).Tag.ToString(),
                              (_detect_sample.SelectedItem as Label).Tag.ToString(),
                              (_detect_sensitivity.SelectedItem as Label).Tag.ToString(),
                              (_detect_result.SelectedItem as Label).Tag.ToString(),
                              (Application.Current.Resources["User"] as UserInfo).DepartmentID,
                              (Application.Current.Resources["User"] as UserInfo).ID,
                              System.DateTime.Now);


                int i = dbOperation.ExecuteSql(sql);
                if (i == 1)
                {
                    Toolkit.MessageBox.Show("添加成功！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    clear();
                    ComboboxTool.InitComboboxSource(_source_company, "SELECT COMPANYID,COMPANYNAME FROM v_user_company WHERE userid =  " + userId, "lr");
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
            //获取变更前的状态
            bool flag = _province.IsEnabled ;
            
            //来源单位下拉选择的是有效内容，则将省市区的下拉灰显并且自动赋值
            if (_source_company.SelectedIndex >= 1)
            {
                _province.IsEnabled = false;
                _city.IsEnabled = false;
                _region.IsEnabled = false;

                string areaid = dbOperation.GetDataSet("SELECT AREAID from t_company where COMPANYID = " + (_source_company.SelectedItem as Label).Tag.ToString()).Tables[0].Rows[0][0].ToString();

                _source_company.Tag = areaid;
                if (areaid.Length > 0)
                {
                    string _areaid = areaid.Substring(0, 2);
                    _province.Text = ProvinceCityTable.Select("id = '" + _areaid + "'")[0]["name"].ToString();
                }
                if (areaid.Length > 2)
                {
                    string _areaid = areaid.Substring(0, 4);
                    _city.Text = ProvinceCityTable.Select("id = '" + _areaid + "'")[0]["name"].ToString();
                }
                if (areaid.Length > 4)
                {
                    _region.Text = ProvinceCityTable.Select("id = '" + areaid + "'")[0]["name"].ToString();
                }
            }
            //来源单位下拉选择的是“-请选择-”或是手动输入来源单位，则将省市区的下拉激活并且内容清空
            else if (_source_company.SelectedIndex < 1)
            {
                if (flag == false)
                {
                    _province.IsEnabled = true;
                    _city.IsEnabled = true;
                    _region.IsEnabled = true;

                    _province.SelectedIndex = 0;
                    _city.SelectedIndex = 0;
                    _region.SelectedIndex = 0;
                }
            }
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
                ComboboxTool.InitComboboxSource(_detect_object, string.Format("SELECT objectId,objectName FROM v_item_object WHERE itemId = '{0}' and tradeId = '{1}'", (_detect_item.SelectedItem as Label).Tag ,(_detect_trade.SelectedItem as Label).Tag), "lr");
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

        private void Detect_Number_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            //if (e.DataObject.GetDataPresent(typeof(String)))
            //{
            //    String text = (String)e.DataObject.GetData(typeof(String));
            //    if (!isNumberic(text))
            //    { e.CancelCommand(); }
            //}
            //else { e.CancelCommand(); }
        }

        private void Detect_Number_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }
    }
}
