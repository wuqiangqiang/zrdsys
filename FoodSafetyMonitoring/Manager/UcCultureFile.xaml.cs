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
using FoodSafetyMonitoring.Manager.UserControls;
using FoodSafetyMonitoring.Common;
using Toolkit = Microsoft.Windows.Controls;
using System.Data;
using System.IO;

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// UcCultureFile.xaml 的交互逻辑
    /// </summary>
    public partial class UcCultureFile : UserControl
    {
        private IDBOperation dbOperation;
        string userId = (Application.Current.Resources["User"] as UserInfo).ID;
        private string file_num;

        public UcCultureFile(IDBOperation dbOperation)
        {
            this.dbOperation = dbOperation;
            InitializeComponent();
            //画面初始化：养殖品种、建档时间、建档人、养殖企业
            ComboboxTool.InitComboboxSource(_object_type, "SELECT ObjectTypeId,ObjectTypeName FROM t_culture_type where OpenFlag = '1'");
            _file_datetime.Text = string.Format("{0:g}", System.DateTime.Now);
            _file_person.Text = (Application.Current.Resources["User"] as UserInfo).ShowName;
            _culture_company.Text = dbOperation.GetDbHelper().GetSingle("SELECT INFO_NAME from sys_client_sysdept WHERE INFO_CODE = " + (Application.Current.Resources["User"] as UserInfo).DepartmentID).ToString();
            //起始档案编号
            file_num = dbOperation.GetDbHelper().GetSingle(string.Format("select f_get_fileno('{0}')",
                              (Application.Current.Resources["User"] as UserInfo).DepartmentID)).ToString();
            _file_num.Text = file_num;
            _file_num_js.Text = file_num;
        }

        private void clear()
        {
            this._colony_house.Text = "";
            this._object_sum.Text = "";
            file_num = dbOperation.GetDbHelper().GetSingle(string.Format("select f_get_fileno('{0}')",
                              (Application.Current.Resources["User"] as UserInfo).DepartmentID)).ToString();
            this._file_num.Text = file_num;
            this._file_num_js.Text = file_num;
            this._object_type.SelectedIndex = 0;
            this._file_datetime.Text = string.Format("{0:g}", System.DateTime.Now);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string msg = "";
            if (_colony_house.Text.Trim().Length == 0)
            {
                msg = "*请输入圈舍号";
            }
            else if ((_object_type.SelectedIndex < 0 && _object_type.Text == "") || _object_type.SelectedIndex == 0) 
            {
                msg = "*请选择养殖品种";
            }
            else if (_object_sum.Text.Trim().Length == 0)
            {
                msg = "*请输入圈舍养殖数量";
            }
            else
            {
                string object_type_id;

                //判断养殖品种是否存在，若不存在则插入数据库
                bool exit_flag = dbOperation.GetDbHelper().Exists(string.Format("SELECT count(ObjectTypeId) from t_culture_type where ObjectTypeName ='{0}'", _object_type.Text));
                if (!exit_flag)
                {
                    int n = dbOperation.GetDbHelper().ExecuteSql(string.Format("INSERT INTO t_culture_type(ObjectTypeName,OPENFLAG,CreateUserId,CreateTime) VALUES('{0}','{1}','{2}','{3}')",
                                                                  _object_type.Text,'1',(Application.Current.Resources["User"] as UserInfo).ID, DateTime.Now));
                    if (n == 1)
                    {
                        object_type_id = dbOperation.GetDbHelper().GetSingle(string.Format("SELECT ObjectTypeId from t_culture_type where ObjectTypeName ='{0}'", _object_type.Text)).ToString();
                    }
                    else
                    {
                        Toolkit.MessageBox.Show("养殖品种添加失败！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                }
                else
                {
                    object_type_id = dbOperation.GetDbHelper().GetSingle(string.Format("SELECT ObjectTypeId from t_culture_type where ObjectTypeName ='{0}'", _object_type.Text)).ToString();
                }

                string sql = string.Format("call p_insert_culture_file('{0}','{1}','{2}','{3}','{4}','{5}','{6}')"
                              , object_type_id,
                              _colony_house.Text,
                              _object_sum.Text,
                              _file_num.Text,
                              (Application.Current.Resources["User"] as UserInfo).DepartmentID,
                              userId,
                              System.DateTime.Now);


                int i = dbOperation.GetDbHelper().ExecuteSql(sql);
                if (i == 1)
                {
                    Toolkit.MessageBox.Show("添加成功！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    clear();
                    ComboboxTool.InitComboboxSource(_object_type, "SELECT ObjectTypeId,ObjectTypeName FROM t_culture_type where OpenFlag = '1'", "lr");
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

        private void Colony_House_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void Object_Sum_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!isNumberic(text))
                { e.CancelCommand(); }
            }
            else { e.CancelCommand(); }
        }

        private void Object_Sum_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void Object_Sum_PreviewTextInput(object sender, TextCompositionEventArgs e)
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

        private void _object_sum_TextChanged(object sender, TextChangedEventArgs e)
        {
            int value;
            if ((sender as TextBox).Text == "")
            {
                value = 0;
            }
            else
            {
                value = Convert.ToInt32((sender as TextBox).Text) - 1;
            }
            int num = int.Parse(file_num.Substring(8, 5));
            string file_num_1 = file_num.Substring(0, 8);
            string file_num_2 = "00000" + Convert.ToString(num + value);
            _file_num_js.Text = file_num_1 + file_num_2.Substring(file_num_2.Length - 5, 5);
        }
    }
}
