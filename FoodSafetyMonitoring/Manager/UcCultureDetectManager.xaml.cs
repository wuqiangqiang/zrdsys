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
    /// UcCultureDetectManager.xaml 的交互逻辑
    /// </summary>
    public partial class UcCultureDetectManager : UserControl
    {
        private IDBOperation dbOperation;

        string userId = (Application.Current.Resources["User"] as UserInfo).ID;
        private string dept_id;

        public UcCultureDetectManager(IDBOperation dbOperation)
        {
            InitializeComponent();
            this.dbOperation = dbOperation;

            ComboboxTool.InitComboboxSource(_colony_no, string.Format("call p_user_colony_batch({0})", userId), "lr");
            _colony_no.SelectionChanged += new SelectionChangedEventHandler(_colony_no_SelectionChanged);
            //ComboboxTool.InitComboboxSource(_culture_file, "select FileNo,FileNo from v_user_culture_file where userid =" + userId, "lr");
            //_culture_file.SelectionChanged += new SelectionChangedEventHandler(_culture_file_SelectionChanged);
            //ComboboxTool.InitComboboxSource(_detect_trade, "select tradeId,tradeName from t_trade where openFlag = '1'", "lr");
            //_detect_trade.SelectionChanged += new SelectionChangedEventHandler(_detect_trade_SelectionChanged);
            //_detect_trade.SelectedIndex = 1;
            ComboboxTool.InitComboboxSource(_detect_item, string.Format("SELECT ItemID,ItemNAME FROM t_det_item_hb WHERE  (tradeId ='1'or ifnull(tradeId,'') = '') and OPENFLAG = '1' "), "lr");
            _detect_item.SelectionChanged += new SelectionChangedEventHandler(_detect_item_SelectionChanged);
            ComboboxTool.InitComboboxSource(_detect_result, "SELECT resultId,resultName FROM t_det_result where openFlag = '1' ORDER BY id", "lr");
            ComboboxTool.InitComboboxSource(_card_brand, "SELECT cardbrandid,cardbrandname FROM t_cardbrand where openFlag = '1'", "lr");
            _entering_datetime.Text = string.Format("{0:g}", System.DateTime.Now);
            _detect_person.Text = (Application.Current.Resources["User"] as UserInfo).ShowName;
        }

        private void clear()
        {
            this._culture_file.SelectedIndex = 0;
            //this._file_cdate.Text = "";
            this._colony_no.Text = "";
            this._detect_site.Text = "";
            //this._detect_trade.SelectedIndex = 1;
            this._card_brand.SelectedIndex = 0;
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
            if (_colony_no.Text.Trim().Length == 0 )
            {
                msg = "*请选择圈舍批次号";
            }
            else if ((_culture_file.SelectedIndex < 0 && _culture_file.Text == "") || _culture_file.SelectedIndex == 0)
            {
                msg = "*请选择档案编号";
            }
            else if (_detect_site.Text.Trim().Length == 0)
            {
                msg = "*养殖企业名称不能为空";
            }
            //else if (_file_cdate.Text.Trim().Length == 0 )
            //{
            //    msg = "*建档时间不能为空";
            //} 
            else if (_detect_item.SelectedIndex < 1)
            {
                msg = "*请选择检测项目";
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
                msg = "*检测师不能为空";
            }
            else if (_card_brand.SelectedIndex < 1)
            {
                msg = "*请选择检测用卡";
            }
            else
            {
                string sql = string.Format("call p_insert_culture_detect('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}')"
                              , (_colony_no.SelectedItem as Label).Tag.ToString(),
                              (_culture_file.SelectedItem as Label).Tag.ToString(),
                              (_card_brand.SelectedItem as Label).Tag.ToString(),
                              (_detect_item.SelectedItem as Label).Tag.ToString(),
                              (_detect_method1.IsChecked == true ? 1 : 0) + (_detect_method2.IsChecked == true ? 2 : 0) + (_detect_method3.IsChecked == true ? 3 : 0),
                              (_detect_object.SelectedItem as Label).Tag.ToString(),
                              (_detect_sample.SelectedItem as Label).Tag.ToString(),
                              (_detect_sensitivity.SelectedItem as Label).Tag.ToString(),
                              (_detect_result.SelectedItem as Label).Tag.ToString(),
                              dept_id,
                              (Application.Current.Resources["User"] as UserInfo).ID,
                              System.DateTime.Now);


                int i = dbOperation.GetDbHelper().ExecuteSql(sql);
                if (i == 1)
                {
                    Toolkit.MessageBox.Show("添加成功！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    clear();
                    //ComboboxTool.InitComboboxSource(_culture_file, "select FileNo,FileNo from v_user_culture_file where userid =" + userId, "lr");
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

        void _colony_no_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_colony_no.SelectedIndex > 0)
            {
                ComboboxTool.InitComboboxSource(_culture_file, string.Format("call p_user_culture_file({0},'{1}')", userId, (_colony_no.SelectedItem as Label).Tag), "lr");
                //_culture_file.SelectionChanged += new SelectionChangedEventHandler(_culture_file_SelectionChanged);
                DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_culturefile('{0}')", (_colony_no.SelectedItem as Label).Tag)).Tables[0];
                this._detect_site.Text = table.Rows[0][3].ToString();
                dept_id = table.Rows[0][2].ToString();
            }

        }

        //void _culture_file_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    string culture_no;
        //    //if (_culture_file.SelectedIndex > 0)
        //    //{
        //    //    culture_no = (_culture_file.SelectedItem as Label).Tag.ToString();
        //    //}
        //    //else
        //    //{
        //    //    culture_no = _culture_file.Text;
        //    //}

        //    if (_culture_file.SelectedIndex > 0)
        //    //if (_culture_file.SelectedIndex > 0 || (_culture_file.SelectedIndex < 0 && _culture_file.Text != "" && _culture_file.Text != "-请选择-"))
        //    {
        //        culture_no = (_culture_file.SelectedItem as Label).Tag.ToString();

        //        DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_culturefile('{0}')", culture_no)).Tables[0];
        //        //if (table.Rows.Count == 0)
        //        //{
        //        //    Toolkit.MessageBox.Show("档案编码输入有误！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
        //        //    return;
        //        //}
        //        //else
        //        //{
        //            this._file_cdate.Text = table.Rows[0][0].ToString();
        //            //this._colony_no.Text = table.Rows[0][1].ToString(); 
        //            this._detect_site.Text = table.Rows[0][3].ToString();
        //            dept_id = table.Rows[0][2].ToString(); 
        //        //}
        //    }
        //    else if (_culture_file.SelectedIndex == 0)
        //    {
        //        this._file_cdate.Text = "";
        //        //this._colony_no.Text = "";
        //        this._detect_site.Text = "";
        //        dept_id = ""; 
        //    }
        //}

        //void _detect_trade_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (_detect_trade.SelectedIndex > 0)
        //    {
        //        //ComboboxTool.InitComboboxSource(_detect_item, string.Format("SELECT itemid,ItemNAME FROM v_user_item WHERE userid = '{0}' and (tradeId ='{1}'or ifnull(tradeId,'') = '')", userId, (_detect_trade.SelectedItem as Label).Tag), "lr");
        //        ComboboxTool.InitComboboxSource(_detect_item, string.Format("SELECT ItemID,ItemNAME FROM t_det_item WHERE  (tradeId ='{0}'or ifnull(tradeId,'') = '') and OPENFLAG = '1' order by orderId", (_detect_trade.SelectedItem as Label).Tag), "lr");
        //        _detect_item.SelectionChanged += new SelectionChangedEventHandler(_detect_item_SelectionChanged);
        //    }
        //    //else
        //    //{
        //    //    _detect_item.Items.Clear();
        //    //    _detect_object.Items.Clear();
        //    //    _detect_sample.Items.Clear();
        //    //    _detect_sensitivity.Items.Clear();
        //    //}
        //}

        void _detect_item_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_detect_item.SelectedIndex > 0)
            {
                //ComboboxTool.InitComboboxSource(_detect_object, string.Format("call p_detect_object( '{0}','{1}')", userId, (_detect_item.SelectedItem as Label).Tag), "lr");
                //ComboboxTool.InitComboboxSource(_detect_object, string.Format("SELECT objectId,objectName FROM v_item_object WHERE itemId = '{0}' and tradeId = '{1}'", (_detect_item.SelectedItem as Label).Tag, (_detect_trade.SelectedItem as Label).Tag), "lr");
                ComboboxTool.InitComboboxSource(_detect_object, string.Format("SELECT objectId,objectName FROM t_det_object WHERE OPENFLAG = '1'"), "lr");
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


        
    }
}
