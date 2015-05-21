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
using Toolkit = Microsoft.Windows.Controls;
using DBUtility;
using System.Data;
using FoodSafetyMonitoring.Common;
using System.ComponentModel;
using System.Collections.ObjectModel;
using FoodSafetyMonitoring.dao;
 

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// Test.xaml 的交互逻辑
    /// </summary>
    public partial class SysDeptManager : UserControl, IClickChildMenuInitUserControlUI
    {
        FamilyTreeViewModel departmentViewModel;
        private IDBOperation dbOperation;

        readonly Dictionary<string, string> cityLevelDictionary = new Dictionary<string, string>() { { "0", "国家" }, { "1", "省级" }, { "2", "地市" }, { "3", "区县" }, { "4", "检测站" } };
        private Department department;
        private DataTable ProvinceCityTable = null;
        private string user_flag_tier;
        private DataTable SupplierTable;

        public SysDeptManager(IDBOperation dbOperation)
        {
            InitializeComponent();
            this.dbOperation = dbOperation;
            ProvinceCityTable = Application.Current.Resources["省市表"] as DataTable;
            user_flag_tier = (Application.Current.Resources["User"] as UserInfo).FlagTier.ToString();
            SupplierTable = dbOperation.GetDbHelper().GetDataSet("select supplierId,supplierName from t_supplier").Tables[0];
            InitUserControlUI();
        }

        #region IClickChildMenuInitUserControlUI 成员

        private int flag_init = 0;//初始化,0未初始化,1已初始化

        public void InitUserControlUI()
        {
            if (flag_init == 1)
            {
                return;
            }
            flag_init = 1;

            DataTable table = dbOperation.GetDepartment();
            if (table != null)
            {
                department = new Department();
                //DataRow[] rows = table.Select("FK_CODE_DEPT='0'");
                //if (rows.Length == 0)
                //{
                //    return;
                //}
                DataRow[] rows = table.Select();
                department.Name = rows[0]["INFO_NAME"].ToString();
                department.Row = rows[0];
                rows = table.Select("FK_CODE_DEPT='" + rows[0]["INFO_CODE"].ToString() + "'");
                foreach (DataRow row1 in rows)
                {
                    Department department1 = new Department();
                    department1.Parent = department;
                    department1.Row = row1;
                    department1.Name = row1["INFO_NAME"].ToString();
                    //rows = table.Select("FK_CODE_DEPT='" + row1["INFO_CODE"].ToString() + "'");
                    //foreach (DataRow row2 in rows)
                    //{
                    //    Department department2 = new Department();
                    //    department2.Parent = department1;
                    //    department2.Row = row2;
                    //    department2.Name = row2["INFO_NAME"].ToString();
                    //    rows = table.Select("FK_CODE_DEPT='" + row2["INFO_CODE"].ToString() + "'");
                    //    foreach (DataRow row3 in rows)
                    //    {
                    //        Department department3 = new Department();
                    //        department3.Parent = department2;
                    //        department3.Row = row3;
                    //        department3.Name = row3["INFO_NAME"].ToString();
                    //        rows = table.Select("FK_CODE_DEPT='" + row3["INFO_CODE"].ToString() + "'");
                    //        foreach (DataRow row4 in rows)
                    //        {
                    //            Department department4 = new Department();
                    //            department4.Parent = department3;
                    //            department4.Row = row4;
                    //            department4.Name = row4["INFO_NAME"].ToString();
                    //            department3.Children.Add(department4);
                    //        }
                    //        department2.Children.Add(department3);
                    //    }
                    //    department1.Children.Add(department2);
                    //}
                    department.Children.Add(department1);
                }

                departmentViewModel = new FamilyTreeViewModel(department);
                _treeView.DataContext = departmentViewModel;
            }
        }

        #endregion

        private void _detect_method1_Checked(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).Name == "_direct_station")
            {
                _direct_station_2.IsChecked = false;
                _cultivate_station.IsChecked = false;
                _slaughter_station.IsChecked = false;
            }
            else if ((sender as CheckBox).Name == "_direct_station_2")
            {
                _direct_station.IsChecked = false;
                _cultivate_station.IsChecked = false;
                _slaughter_station.IsChecked = false;
            }
            else if ((sender as CheckBox).Name == "_cultivate_station")
            {
                _direct_station_2.IsChecked = false;
                _direct_station.IsChecked = false;
                _slaughter_station.IsChecked = false;
            }
            else if ((sender as CheckBox).Name == "_slaughter_station")
            {
                _direct_station_2.IsChecked = false;
                _direct_station.IsChecked = false;
                _cultivate_station.IsChecked = false;
            }
        }


        //保存按钮
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Department department = _add.Tag as Department;

            //新增
            if (state == "add")
            {
                DataTable dt = new DataTable();
                DataRow row = department.Row.Table.NewRow();
                row.ItemArray = (object[])department.Row.ItemArray.Clone();
                row["FK_CODE_DEPT"] = row["INFO_CODE"];

                int maxID = 0;

                if (department.Children.Count == 0)
                {
                    maxID = Convert.ToInt32(row["INFO_CODE"].ToString() + "01");
                    row["INFO_CODE"] = maxID;
                }
                else
                {
                    for (int i = 0; i < department.Children.Count; i++)
                    {
                        int v = Convert.ToInt32(department.Children[i].Row["INFO_CODE"].ToString());
                        if (maxID < v)
                        {
                            maxID = v;
                        }
                    }
                    row["INFO_CODE"] = maxID + 1;
                }

                row["INFO_NAME"] = _station.Text;
                row["FLAG_TIER"] = (Convert.ToInt32(department.Row["FLAG_TIER"].ToString()) + 1);
                if (_lower_area.SelectedIndex == -1 && row["FLAG_TIER"].ToString() != "4")
                {
                    Toolkit.MessageBox.Show("请选择所在地!", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                if (_station.Text == "")
                {
                    if (row["FLAG_TIER"].ToString() == "4")
                    {
                        Toolkit.MessageBox.Show("请输入检测站点名称!", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    else
                    {
                        Toolkit.MessageBox.Show("请输入部门名称!", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                }

                if (user_flag_tier == "0")
                {
                    if (_Supplier.SelectedIndex < 1)
                    {
                        Toolkit.MessageBox.Show("请选择供应商!", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                }

                string lower_area_id = "";

                if (_lower_area.Text != "")
                {
                    lower_area_id = ProvinceCityTable.Select("name='" + _lower_area.Text + "'")[0]["id"].ToString();
                }
                //根据当前部门的级别来赋省，市，区的值
                switch (row["FLAG_TIER"].ToString())
                {
                    case "0": row["Province"] = ""; 
                              row["City"] = ""; 
                              row["Country"] = ""; 
                              break;
                    case "1": row["Province"] = lower_area_id; 
                              row["City"] = ""; 
                              row["Country"] = ""; 
                              break;
                    case "2": row["Province"] = department.Row["Province"].ToString(); 
                              row["City"] = lower_area_id; 
                              row["Country"] = ""; 
                              break;
                    case "3": row["Province"] = department.Row["Province"].ToString(); 
                              row["City"] = department.Row["City"].ToString();
                              row["Country"] = lower_area_id;
                              break;
                    case "4": row["Province"] = department.Row["Province"].ToString(); 
                              row["City"] = department.Row["City"].ToString();
                              row["Country"] = department.Row["Country"].ToString();
                              break;
                    default: break;
                }
                row["INFO_NAME"] = _station.Text;
                row["address"] = _address.Text;
                row["CONTACTER"] = _principal_name.Text;
                row["tel"] = _phone.Text;
                row["phone"] = _contact_number.Text;
                row["supplierId"] = (_Supplier.SelectedItem as Label).Tag.ToString();
                //row["title"] = _title.Text;
                //row["INFO_NOTE"] = _note.Text;

                if (_direct_station.IsChecked == true)
                {
                    row["type"] = "2";
                }
                else if (_cultivate_station.IsChecked == true)
                {
                    row["type"] = "1";
                }
                else if (_slaughter_station.IsChecked == true)
                {
                    row["type"] = "0";
                }
                else if (_direct_station_2.IsChecked == true)
                {
                    row["type"] = "3";
                }

                if (row["type"].ToString() == ""   && row["FLAG_TIER"].ToString() == "4")
                {
                    Toolkit.MessageBox.Show("请选择检测点性质!", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                Department newDepartment = new Department();
                newDepartment.Parent = department;
                newDepartment.Name = _station.Text;
                newDepartment.Row = row;

                string sql = String.Format("insert into sys_client_sysdept (INFO_CODE,INFO_NAME,FLAG_TIER,FK_CODE_DEPT,PROVINCE,CITY,COUNTRY,ADDRESS,CONTACTER,TEL,PHONE,TYPE,supplierId) values " +
                    "('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}');"
              , row["INFO_CODE"], row["INFO_NAME"], row["FLAG_TIER"], row["FK_CODE_DEPT"]
              , row["PROVINCE"], row["CITY"], row["COUNTRY"], row["ADDRESS"], row["CONTACTER"], row["TEL"], row["PHONE"], row["TYPE"], row["supplierId"]);

                try
                {
                    int count = dbOperation.GetDbHelper().ExecuteSql(sql);
                    if (count == 1)
                    {
                        Toolkit.MessageBox.Show("保存成功！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        Common.SysLogEntry.WriteLog("部门管理", (Application.Current.Resources["User"] as UserInfo).ShowName, Common.OperationType.Modify, "新增部门信息");
                    }
                    else
                    {
                        Toolkit.MessageBox.Show("保存失败！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                }
                catch (Exception ee)
                {
                    System.Diagnostics.Debug.WriteLine("SysDeptManager.btnSave_Click" + ee.Message);
                    Toolkit.MessageBox.Show("数据更新失败!稍后尝试!");
                    return;
                }
                state = "view";

                department.Children.Add(newDepartment);
                departmentViewModel = new FamilyTreeViewModel(this.department);

                departmentViewModel.SearchText = _station.Text;
                departmentViewModel.SearchCommand.Execute(null);



            }//修改
            else if (state == "edit")
            {
                //获取画面上的检测点类型
                string type = "";
                if (_direct_station.IsChecked == true)
                {
                    type = "2";
                }
                else if (_cultivate_station.IsChecked == true)
                {
                    type = "1";
                }
                else if (_slaughter_station.IsChecked == true)
                {
                    type = "0";
                }

                string sql = String.Format("UPDATE sys_client_sysdept set INFO_NAME='{0}',ADDRESS='{1}',CONTACTER='{2}',TEL='{3}',PHONE='{4}',TYPE='{5}',supplierId = '{6}'  where INFO_CODE='{7}';"
                , _station.Text, _address.Text, _principal_name.Text, _phone.Text, _contact_number.Text,type ,(_Supplier.SelectedItem as Label).Tag, department.Row["INFO_CODE"]);

                try
                {
                    int count = dbOperation.GetDbHelper().ExecuteSql(sql);
                    if (count == 1)
                    {
                        Toolkit.MessageBox.Show("保存成功！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        Common.SysLogEntry.WriteLog("部门管理", (Application.Current.Resources["User"] as UserInfo).ShowName, Common.OperationType.Modify, "修改部门信息");
                    }
                    else
                    {
                        Toolkit.MessageBox.Show("保存失败！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
                catch (Exception ee)
                {
                    System.Diagnostics.Debug.WriteLine("SysDeptManager.btnSave_Click" + ee.Message);
                      Toolkit.MessageBox.Show("数据更新失败!稍后尝试!");
                    return;
                }
                state = "view";

                //更新树形表
                department.Row["INFO_NAME"] = _station.Text;
                department.Name = _station.Text;
                department.Row["address"] = _address.Text;
                department.Row["contacter"] = _principal_name.Text;
                department.Row["tel"] = _phone.Text;
                department.Row["phone"] = _contact_number.Text;
                _edit.IsEnabled = true;


            }
            else
            {
                return;
            }

            _treeView.DataContext = null;
            _treeView.DataContext = departmentViewModel;
            _detail_info.IsEnabled = false;
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {

        }

        //private void txtSearch_ImageClick(object sender, EventArgs e)
        //{
        //    departmentViewModel.SearchText = txtSearch.Text;
        //    departmentViewModel.SearchCommand.Execute(null);
        //}

        //根据代码获取所在地和下级城市
        private void GetCityByCode(string flag_tier, string province, string city, string country, ref string name, ref List<string> citys)
        {
            switch (flag_tier)
            {
                case "0": name = ""; citys = GetList("0001","0"); break;
                case "1": name = GetName(province); citys = GetList(province, "1"); break;
                case "2": name = GetName(province) + GetName(city); citys = GetList(city,"2"); break;
                case "3": name = GetName(province) + GetName(city) + GetName(country); citys = new List<string>(); break;
                case "4": name = GetName(province) + GetName(city) + GetName(country); citys = new List<string>(); break;
                default: break;
            }
        }

        //根据代码获取当前城市名
        private string GetName(string code)
        {
            DataRow[] rows = ProvinceCityTable.Select("id='" + code + "'");
            if (rows.Length > 0)
            {
                return rows[0]["name"].ToString();
            }
            else
            {
                return "";
            }
        }

        //根据代码获取下级所有城市
        private List<string> GetList(string code,string dept_flag)
        {
            List<string> list = new List<string>();
            //DataRow[] rows = ProvinceCityTable.Select("pid='" + code + "'");
            DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_area('{0}','{1}')", code, dept_flag)).Tables[0];
            DataRow[] rows = table.Select(); 
            foreach (DataRow row in rows)
            {
                list.Add(row["name"].ToString());
            }
            return list;
        }

        private string state = "view";
        private TextBlock text_treeView = null;

        //省市上双击事件
        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount > 0)
            {
                _contact_number.Text = "";
                _phone.Text = "";
                _principal_name.Text = "";
                _address.Text = "";
                _area_flag.Text = "";
                _station_flag.Text = "";
                _station_property_flag.Text = "";

                _detail_info_all.Visibility = Visibility.Visible;
                _detail_info.IsEnabled = false;
                text_treeView = sender as TextBlock;
                Department department = (sender as TextBlock).Tag as Department;
                _add.Tag = department;
                _edit.Tag = department;
                DataRow row = department.Row;
                _regional_level.Tag = row["FLAG_TIER"].ToString();
                _regional_level.Text = cityLevelDictionary[row["FLAG_TIER"].ToString()];
                _station_property.Visibility = Visibility.Hidden;
                _lower_area.Visibility = Visibility.Hidden;
                _edit.IsEnabled = true;
                _add.IsEnabled = true;
                if (row["FLAG_TIER"].ToString() == "4")
                {
                    _station_name.Text = "检测站点名称:";
                }
                else
                {
                    _station_name.Text = "部门名称:";
                }
                if (user_flag_tier == row["FLAG_TIER"].ToString())
                {
                    _add.Visibility = Visibility.Visible;
                    _delete.Visibility = Visibility.Hidden;
                    _edit.Visibility = Visibility.Hidden;
                }
                else
                {
                    _add.Visibility = Visibility.Hidden;
                    _delete.Visibility = Visibility.Visible;
                    _edit.Visibility = Visibility.Visible;
                }

                if (_regional_level.Text == "检测站")
                {
                    _add.Visibility = Visibility.Hidden;
                    _station_property.Visibility = Visibility.Visible;
                }

                if (user_flag_tier == "0" && row["FLAG_TIER"].ToString() != "0")
                {
                    _Supplier_name.Visibility = Visibility.Visible;
                    _Supplier.Visibility = Visibility.Visible;
                }
                else
                {
                    _Supplier_name.Visibility = Visibility.Hidden;
                    _Supplier.Visibility = Visibility.Hidden;
                }

                ComboboxTool.InitComboboxSource(_Supplier, "select supplierId,supplierName from t_supplier", "lr");
                DataRow[] rows = SupplierTable.Select("supplierId = '" + row["supplierId"] + "'");
                if (rows.Length > 0)
                {
                    _Supplier.Text = rows[0]["supplierName"].ToString();
                }

                string city = "";
                List<string> citys = null;
                GetCityByCode(row["FLAG_TIER"].ToString(), row["province"].ToString(), row["city"].ToString(), row["country"].ToString(), ref city, ref citys);

                _belong_to.Text = city;
                //if (citys.Count > 0)
                //{
                    _lower_area.ItemsSource = citys;
                //}

                _phone.Text = row["tel"].ToString();
                _contact_number.Text = row["phone"].ToString();
                _address.Text = row["address"].ToString();
                _principal_name.Text = row["contacter"].ToString();

                _station.Text = row["INFO_NAME"].ToString();
                if (department.Parent != null)
                {
                    _superior_department.Text = department.Parent.Row["INFO_NAME"].ToString();
                }
                else
                {
                    if (row["FLAG_TIER"].ToString() == "0")
                    {
                        _superior_department.Text = "无";
                    }
                    else
                    {
                        _superior_department.Text = dbOperation.GetDbHelper().GetSingle(string.Format("select INFO_NAME FROM sys_client_sysdept WHERE INFO_CODE = '{0}'", row["FK_CODE_DEPT"].ToString())).ToString();
                    }  
                }

                _direct_station.IsChecked = false;
                _cultivate_station.IsChecked = false;
                _slaughter_station.IsChecked = false;
                if (row["type"].ToString().Length != 0)
                {
                    if (Convert.ToInt32(row["type"].ToString()) == 2)
                    {
                        _direct_station.IsChecked = true;
                    }
                    else if (Convert.ToInt32(row["type"].ToString()) == 1)
                    {
                        _cultivate_station.IsChecked = true;
                    }
                    else if (Convert.ToInt32(row["type"].ToString()) == 0)
                    {
                        _slaughter_station.IsChecked = true;
                    }
                }

                //_delete.Visibility = Visibility.Hidden;
                //if (department.Children.Count == 0)
                //{
                //    _delete.Visibility = Visibility.Visible;
                //}
            }
        }

        private void _edit_Click(object sender, RoutedEventArgs e)
        {
            _detail_info.IsEnabled = true;
            state = "edit";
            _edit.IsEnabled = false;

            Department department = _edit.Tag as Department;
            bool result = dbOperation.GetDbHelper().Exists(string.Format("select count(INFO_CODE) from sys_client_sysdept where FK_CODE_DEPT ='{0}'", department.Row["INFO_CODE"]));
            if (result)
            {
                _Supplier.IsEnabled = false;
            }
            else
            {
                _Supplier.IsEnabled = true;
            }

            //修改的话，所在地下拉框显示
            //_lower_area.Visibility = Visibility.Visible;
            //_belong_to.Visibility = Visibility.Hidden;
        }

        private void _add_Click(object sender, RoutedEventArgs e)
        {
            if (_station.Text.Length == 0)
            {
                Toolkit.MessageBox.Show("检测点名称不能为空!");
                return;
            }
            state = "add";
            
            _add.IsEnabled = false;
            _detail_info.IsEnabled = true;
            Department department = _add.Tag as Department;

            //判断所在地下拉有无值，如果没有值，则显示文本框不显示下拉框；有值则显示下拉框不显示文本框
            if (_lower_area.Items.Count == 0)
            {
                _lower_area.Visibility = Visibility.Hidden;
                _belong_to.Visibility = Visibility.Visible;
            }
            else
            {
                _lower_area.Visibility = Visibility.Visible;
                //_belong_to.Visibility = Visibility.Hidden;
                //_belong_to.Text = "";
            }

            if (user_flag_tier == "0")
            {
                _Supplier_name.Visibility = Visibility.Visible;
                _Supplier.Visibility = Visibility.Visible;
                _Supplier.IsEnabled = true;
                ComboboxTool.InitComboboxSource(_Supplier, "select supplierId,supplierName from t_supplier", "lr");
            }

            string regional_level = (Convert.ToInt32(_regional_level.Tag.ToString()) + 1).ToString();
            _regional_level.Text = cityLevelDictionary[regional_level];
            _regional_level.Tag = regional_level;

            _station_flag.Text = "(必填)";
            
            //如果当前添加的是检测站，则显示检测点性质信息
            if (_regional_level.Text == "检测站")
            {
                _add.Visibility = Visibility.Hidden;
                _station_property.Visibility = Visibility.Visible;
                _station_property_flag.Text = "(必填)";
            }
            else
            {
                _area_flag.Text = "(必填)";
            }
            if (_regional_level.Text == "检测站")
            {
                _station_name.Text = "检测站点名称:";
            }
            else
            {
                _station_name.Text = "部门名称:";
            }

            //新增部门，画面上字段进行初始化
            _superior_department.Text = _station.Text;
            _station.Text = "";
            _principal_name.Text = "";
            _phone.Text = "";
            _contact_number.Text = "";
            _address.Text = "";

        }

        private void _delete_Click(object sender, RoutedEventArgs e)
        {
            Department department = _add.Tag as Department;
            bool result = dbOperation.GetDbHelper().Exists(string.Format("select count(ORDERID) from t_detect_report where DETECTID ='{0}'", department.Row["INFO_CODE"]));
            if (result)
            {
                Toolkit.MessageBox.Show("该检测站点已有对应的检测单数据，不能删除！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            bool result2 = dbOperation.GetDbHelper().Exists(string.Format("select count(RECO_PKID) from sys_client_user where fk_dept = '{0}'", department.Row["INFO_CODE"]));
            if (result2)
            {
                Toolkit.MessageBox.Show("该部门已被用户占用，不能删除！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            bool result3 = dbOperation.GetDbHelper().Exists(string.Format("select count(INFO_CODE) from sys_client_sysdept where FK_CODE_DEPT = '{0}'", department.Row["INFO_CODE"]));
            if (result3)
            {
                Toolkit.MessageBox.Show("该部门已有下级部门，不能删除！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (Toolkit.MessageBox.Show("确定要删除该部门吗？", "系统询问", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    string sql = String.Format("delete from sys_client_sysdept where INFO_CODE='{0}'", department.Row["INFO_CODE"]);
                    int count = dbOperation.GetDbHelper().ExecuteSql(sql);
                    if (count == 1)
                    {
                        Toolkit.MessageBox.Show("删除成功！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        Common.SysLogEntry.WriteLog("部门管理", (Application.Current.Resources["User"] as UserInfo).ShowName, Common.OperationType.Modify, "删除部门信息");
                    }
                    else
                    {
                        Toolkit.MessageBox.Show("删除失败！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
                catch (Exception ee)
                {
                    System.Diagnostics.Debug.WriteLine("SysDeptManager._delete_Click" + ee.Message);
                    Toolkit.MessageBox.Show("数据删除失败!稍后尝试!");
                    return;
                }
            }
            else
            {
                return;
            }
            department.Parent.Children.Remove(department);
            _treeView.DataContext = null;
            departmentViewModel = new FamilyTreeViewModel(this.department);
            _treeView.DataContext = departmentViewModel;

            string searchTxt = "";
            if (department.Parent.Children.Count == 0)
            {
                searchTxt = department.Name;
            }
            else
            {
                searchTxt = department.Parent.Children[0].Name;
            }
            departmentViewModel.SearchText = searchTxt;
            departmentViewModel.SearchCommand.Execute(null);
        }


        private void Phone_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!isNumberic(text))
                { e.CancelCommand(); }
            }
            else { e.CancelCommand(); }
        }

        private void Phone_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void Phone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!isNumberic(e.Text))
            {
                e.Handled = true;
            }
            else
               e.Handled = false;
        }

        private void Contact_Number_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!isNumberic(text))
                { e.CancelCommand(); }
            }
            else { e.CancelCommand(); }
        }

        private void Contact_Number_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void Contact_Number_PreviewTextInput(object sender, TextCompositionEventArgs e)
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



    public class FamilyTreeViewModel
    {
        #region Data

        readonly ReadOnlyCollection<DepartmentViewModel> _firstGeneration;
        readonly DepartmentViewModel _rootPerson;
        readonly ICommand _searchCommand;

        IEnumerator<DepartmentViewModel> _matchingPeopleEnumerator;
        string _searchText = String.Empty;

        #endregion // Data

        #region Constructor

        public FamilyTreeViewModel(Department rootPerson)
        {
            _rootPerson = new DepartmentViewModel(rootPerson);

            _firstGeneration = new ReadOnlyCollection<DepartmentViewModel>(
                new DepartmentViewModel[] 
                { 
                    _rootPerson 
                });

            _searchCommand = new SearchFamilyTreeCommand(this);
        }

        #endregion // Constructor

        #region Properties

        #region FirstGeneration

        /// <summary>
        /// Returns a read-only collection containing the first person 
        /// in the family tree, to which the TreeView can bind.
        /// </summary>
        public ReadOnlyCollection<DepartmentViewModel> FirstGeneration
        {
            get { return _firstGeneration; }
        }



        #endregion // FirstGeneration

        #region SearchCommand

        /// <summary>
        /// Returns the command used to execute a search in the family tree.
        /// </summary>
        public ICommand SearchCommand
        {
            get { return _searchCommand; }
        }

        private class SearchFamilyTreeCommand : ICommand
        {
            readonly FamilyTreeViewModel _familyTree;

            public SearchFamilyTreeCommand(FamilyTreeViewModel familyTree)
            {
                _familyTree = familyTree;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            event EventHandler ICommand.CanExecuteChanged
            {
                // I intentionally left these empty because
                // this command never raises the event, and
                // not using the WeakEvent pattern here can
                // cause memory leaks.  WeakEvent pattern is
                // not simple to implement, so why bother.
                add { }
                remove { }
            }

            public void Execute(object parameter)
            {
                _familyTree.PerformSearch();
            }
        }

        #endregion // SearchCommand

        #region SearchText

        /// <summary>
        /// Gets/sets a fragment of the name to search for.
        /// </summary>
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                if (value == _searchText)
                    return;

                _searchText = value;

                _matchingPeopleEnumerator = null;
            }
        }

        #endregion // SearchText

        #endregion // Properties

        #region Search Logic

        void PerformSearch()
        {
            if (_matchingPeopleEnumerator == null || !_matchingPeopleEnumerator.MoveNext())
                this.VerifyMatchingPeopleEnumerator();

            var person = _matchingPeopleEnumerator.Current;

            if (person == null)
                return;

            // Ensure that this person is in view.
            if (person.Parent != null)
                person.Parent.IsExpanded = true;

            person.IsSelected = true;
        }

        void VerifyMatchingPeopleEnumerator()
        {
            var matches = this.FindMatches(_searchText, _rootPerson);
            _matchingPeopleEnumerator = matches.GetEnumerator();

            if (!_matchingPeopleEnumerator.MoveNext())
            {
                MessageBox.Show(
                    "No matching names were found.",
                    "Try Again",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                    );
            }
        }

        IEnumerable<DepartmentViewModel> FindMatches(string searchText, DepartmentViewModel person)
        {
            if (person.NameContainsText(searchText))
                yield return person;

            foreach (DepartmentViewModel child in person.Children)
                foreach (DepartmentViewModel match in this.FindMatches(searchText, child))
                    yield return match;
        }

        #endregion // Search Logic
    }

    public class DepartmentViewModel : INotifyPropertyChanged
    {
        #region Data

        readonly ReadOnlyCollection<DepartmentViewModel> _children;
        readonly DepartmentViewModel _parent;
        readonly Department _department;

        bool _isExpanded;
        bool _isSelected;

        #endregion // Data

        #region Constructors

        public DepartmentViewModel(Department department)
            : this(department, null)
        {
        }

        private DepartmentViewModel(Department department, DepartmentViewModel parent)
        {
            _department = department;
            _parent = parent;

            _children = new ReadOnlyCollection<DepartmentViewModel>(
                    (from child in _department.Children
                     select new DepartmentViewModel(child, this))
                     .ToList<DepartmentViewModel>());
        }

        #endregion // Constructors

        #region Person Properties

        public ReadOnlyCollection<DepartmentViewModel> Children
        {
            get { return _children; }
        }

        public string Name
        {
            get { return _department.Name; }
        }

        public DataRow Row
        {
            get { return _department.Row; }
        }

        public Department Own
        {
            get { return _department.Own; }
        }

        public BitmapImage Img
        {
            get { return _department.Img; }
        }

        #endregion // Person Properties

        #region Presentation Members

        #region IsExpanded

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                    this.OnPropertyChanged("IsExpanded");
                }

                // Expand all the way up to the root.
                if (_isExpanded && _parent != null)
                    _parent.IsExpanded = true;
            }
        }

        #endregion // IsExpanded

        #region IsSelected

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is selected.
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    this.OnPropertyChanged("IsSelected");
                }
            }
        }

        #endregion // IsSelected

        #region NameContainsText

        public bool NameContainsText(string text)
        {
            if (String.IsNullOrEmpty(text) || String.IsNullOrEmpty(this.Name))
                return false;

            return this.Name.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) > -1;
        }

        #endregion // NameContainsText

        #region Parent

        public DepartmentViewModel Parent
        {
            get { return _parent; }
        }

        #endregion // Parent

        #endregion // Presentation Members

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion // INotifyPropertyChanged Members
    }

    public class Department
    {
        readonly List<Department> _children = new List<Department>();
        public IList<Department> Children
        {
            get { return _children; }
        }

        public string Name { get; set; }
        public Department Parent { get; set; }

        public BitmapImage Img
        {
            get
            {
                string uri = @"/Manager/Images/all.png";
                switch (Row["FLAG_TIER"].ToString())
                {
                    case "0": uri = @"/Manager/Images/all.png"; break;
                    case "1": uri = @"/Manager/Images/provice.png"; break;
                    case "2": uri = @"/Manager/Images/city.png"; break;
                    case "3": uri = @"/Manager/Images/country.png"; break;
                    case "4": uri = @"/Manager/Images/dept.png"; break;
                    default: break;
                }

                return new BitmapImage(new Uri("pack://application:,," + uri));
            }
        }
        public DataRow Row { get; set; }

        public Department Own { get { return this; } }
    }

}
