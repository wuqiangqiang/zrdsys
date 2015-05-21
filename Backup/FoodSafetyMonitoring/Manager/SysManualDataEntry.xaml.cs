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
using FoodSafetyMonitoring.Common;
using FoodSafetyMonitoring.dao;
using System.Data;

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// UcManualDataEntry.xaml 的交互逻辑
    /// </summary>
    public partial class UcManualDataEntry : UserControl
    {

        DataTable ProvinceCityTable;
        IDBOperation dbOperation = new DBOperation_MySQL();
        public UcManualDataEntry()
        {
            //InitializeComponent();
            //string userId = (Application.Current.Resources["User"] as UserInfo).ID;
            //ComboboxTool.InitComboboxSource(_source_company, "SELECT COMPANYID,COMPANYNAME FROM v_user_company WHERE userid =  " + userId);
            //ComboboxTool.InitComboboxSource(_detect_item, "SELECT itemid,ItemNAME FROM v_user_item WHERE userid = " + userId);
            //ComboboxTool.InitComboboxSource(_detect_object, " SELECT objectId,objectName FROM v_user_object WHERE userid = " + userId);
            //ComboboxTool.InitComboboxSource(_detect_sample, "  SELECT sampleId,sampleName FROM v_user_sample WHERE userid = " + userId);
            //ComboboxTool.InitComboboxSource(_source_company, "SELECT COMPANYID,COMPANYNAME FROM v_user_company WHERE userid =  " + userId);
            //ComboboxTool.InitComboboxSource(_detect_sensitivity, "SELECT sensitivityId,sensitivityName FROM t_det_sensitivity");
            //ComboboxTool.InitComboboxSource(_detect_result, "SELECT resultId,resultName FROM t_det_result");

            //_entering_datetime.Text = string.Format("{0:g}", System.DateTime.Now);
            //_source_company.SelectionChanged += new SelectionChangedEventHandler(_source_company_SelectionChanged);
            //ProvinceCityTable = Application.Current.Resources["省市表"] as DataTable;

            //_detect_site.Text = dbOperation.GetDbHelper().GetDataSet("SELECT INFO_NAME from  sys_client_sysdept WHERE INFO_CODE = " + (Application.Current.Resources["User"] as UserInfo).DepartmentID).Tables[0].Rows[0][0].ToString();

        }

        void _source_company_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string areaid = dbOperation.GetDbHelper().GetDataSet("SELECT AREAID from t_company where COMPANYID = " + (_source_company.SelectedItem as Label).Tag.ToString()).Tables[0].Rows[0][0].ToString();

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


        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string msg = "";
            if (_source_company.SelectedIndex < 1)
            {
                msg = "请选择来源单位";
            }
            else if (_detect_number.Text.Trim().Length == 0)
            {
                msg = "检疫证号不能为空";
            }
            else if (_detect_item.SelectedIndex < -1)
            {
                msg = "请选择检查项目";
            }
            else if (_detect_method1.IsChecked != true && _detect_method2.IsChecked != true && _detect_method3.IsChecked != true)
            {
                msg = "请选择检测方法";
            }
            else if (_detect_object.SelectedIndex < -1)
            {
                msg = "请选择检测对象";
            }
            else if (_detect_sample.SelectedIndex < -1)
            {
                msg = "请选择检测样本";
            }
            else if (_detect_sensitivity.SelectedIndex < -1)
            {
                msg = "请选择检测灵敏度";
            }
            else if (_detect_result.SelectedIndex < -1)
            {
                msg = "请选择检测结果";
            }
            else if (_detect_person.Text.Trim().Length == 0)
            {
                msg = "请输入检测师";
            }
            else
            {
                //string sql = "";


                return;
            }
            txtMsg.Text = msg;

        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {

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
    }
}
