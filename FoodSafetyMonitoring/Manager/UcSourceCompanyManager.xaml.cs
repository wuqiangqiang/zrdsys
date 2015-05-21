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
using System.Data;
using FoodSafetyMonitoring.dao;
using FoodSafetyMonitoring.Manager.UserControls;
using Toolkit = Microsoft.Windows.Controls;
 

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// UcSourceCompanyManager.xaml 的交互逻辑
    /// </summary>
    public partial class UcSourceCompanyManager : UserControl 
    {
        private IDBOperation dbOperation;

        private Dictionary<string, MyColumn> MyColumns = new Dictionary<string, MyColumn>();
        private DataTable ProvinceCityTable;
        private DataTable Table;

        public UcSourceCompanyManager(IDBOperation dbOperation)
        {
            InitializeComponent();
            this.dbOperation = dbOperation;

            MyColumns.Add("companyid", new MyColumn("companyid", "单位编号") { BShow = false });
            MyColumns.Add("companyname", new MyColumn("companyname", "来源单位名称") { Width = 2 });
            MyColumns.Add("areaid", new MyColumn("areaid", "地区编号"));
            MyColumns.Add("address", new MyColumn("address", "详细地址") { Width = 3 });
            MyColumns.Add("man", new MyColumn("man", "联系人姓名"));
            MyColumns.Add("phone", new MyColumn("phone", "固话"));
            MyColumns.Add("mobile", new MyColumn("mobile", "手机"));
            MyColumns.Add("fax", new MyColumn("fax", "传真"));
            MyColumns.Add("openflag", new MyColumn("openflag", "是否启用"));
            MyColumns.Add("deptid", new MyColumn("deptid", "所属部门"));
            MyColumns.Add("cuserid", new MyColumn("cuserid", "创建者ID") { BShow = false });
            MyColumns.Add("cdate", new MyColumn("cdate", "创建日期"));

            _tableview.MyColumns = MyColumns;

            ProvinceCityTable = Application.Current.Resources["省市表"] as DataTable;
            DataRow[] rows = ProvinceCityTable.Select("pid = '0001'");
            List<string> provinces = new List<string>();
            provinces.Add("请选择");
            for (int i = 0; i < rows.Length; i++)
            {
                provinces.Add(rows[i]["name"].ToString());
            }
            _province.ItemsSource = provinces;
            _province.SelectedIndex = 0;
            _province.SelectionChanged += new SelectionChangedEventHandler(_province_SelectionChanged);

            _tableview.ModifyRowEnvent += new UcTableOperableView.ModifyRowEventHandler(_tableview_ModifyRowEnvent);
            _tableview.DeleteRowEnvent += new UcTableOperableView.DeleteRowEventHandler(_tableview_DeleteRowEnvent);
        }

        void _tableview_DeleteRowEnvent(string id)
        {
            try
            {
                string sql = string.Format("delete from t_company where companyid = {0}", id); ;
                if (dbOperation.GetDbHelper().ExecuteSql(sql) == 1)
                {

                    Table.Rows.Remove(Table.Select(string.Format("companyid = {0}", id).ToUpper())[0]);
                    Table = dbOperation.GetCompany();
                    _tableview.Table = Table;
                }
                else
                {
                      Toolkit.MessageBox.Show("数据库读写异常,稍后尝试!!!");
                    return;
                }

            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("UcSourceCompanyManager._tableview_DeleteRowEnvent插入数据库异常");
                  Toolkit.MessageBox.Show("数据库读写异常,稍后尝试!!!");
                return;
            }

        }

        void _tableview_ModifyRowEnvent(string id)
        {
            _tabControl.SelectedIndex = 0;
            DataRow row = Table.Select(string.Format("companyid = {0}", id).ToUpper())[0];
            _company_name.Text = row["companyname"].ToString();
            _company_name.Tag = row["companyid"].ToString();
            string areaid = row["areaid"].ToString();
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
            _principal_name.Text = row["man"].ToString();
            _telephone.Text = row["phone"].ToString();
            _mobilePhone.Text = row["mobile"].ToString();
            _fax.Text = row["fax"].ToString();
            _address.Text = row["address"].ToString();
            _isUse.SelectedIndex = Convert.ToInt32(row["openflag"].ToString());
            btnSave.Tag = "modify";
        }

        void _province_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<string> citys = new List<string>();

            if (_province.SelectedItem != null && _province.SelectedItem.ToString() != "请选择")
            {
                citys.Add("请选择");
                DataRow[] rows = ProvinceCityTable.Select("name = '" + _province.SelectedItem + "'");
                string id = rows[0]["id"].ToString();
                rows = ProvinceCityTable.Select("pid = '" + id + "'");

                for (int i = 0; i < rows.Length; i++)
                {
                    citys.Add(rows[i]["name"].ToString());
                }

                _city.SelectionChanged += new SelectionChangedEventHandler(_city_SelectionChanged);
            }
            _city.ItemsSource = citys;
            _city.SelectedIndex = 0;
        }

        void _city_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<string> regions = new List<string>();

            if (_city.SelectedItem != null && _city.SelectedItem.ToString() != "请选择")
            {
                regions.Add("请选择");
                DataRow[] rows = ProvinceCityTable.Select("name = '" + _city.SelectedItem + "'");
                string id = rows[0]["id"].ToString();
                rows = ProvinceCityTable.Select("pid = '" + id + "'");

                for (int i = 0; i < rows.Length; i++)
                {
                    regions.Add(rows[i]["name"].ToString());
                }
            }
            _region.ItemsSource = regions;
            _region.SelectedIndex = 0;
        }

        private bool CheckTextbox(TextBox textBox, int length, string describe)
        {
            if (textBox.Text.Length == 0)
            {
                Toolkit.MessageBox.Show(describe + "不能为空!");
                return false;
            }
            if (textBox.Text.Length > length)
            {
                Toolkit.MessageBox.Show(describe + "字符长太!");
                return false;
            }
            return true;
        }

        /*t_company表主键已改为自增长，无需手动生成
        private int GetMaxID()
        {
            int max = 0;
            for (int i = 0; i < Table.Rows.Count; i++)
            {
                if (Convert.ToInt32(Table.Rows[i][0].ToString()) > max)
                {
                    max = Convert.ToInt32(Table.Rows[i][0].ToString());
                }
            }
            return max;
        }
        */

        //用于清空画面内容
        private void clear()
        {
            _company_name.Text = "";
            _company_name.Tag = "";
            _province.Text = "请选择";
            _city.SelectedIndex = -1;
            _region.SelectedIndex = -1;
            _principal_name.Text = "";
            _telephone.Text = "";
            _mobilePhone.Text = "";
            _fax.Text = "";
            _address.Text = "";
            _isUse.SelectedIndex = 0;
            btnSave.Tag = "";
        }

        private string GetProvinceCityId()
        {
            string areaid = "";
            if (_region.Text == "请选择")
            {
                if (_city.Text == "请选择")
                {
                    if (_province.Text == "请选择")
                    {
                          Toolkit.MessageBox.Show("选择单位区域!!!");
                    }
                    else
                    {
                        areaid = ProvinceCityTable.Select(string.Format(" where name='{0}'", _province.Text))[0][0].ToString();
                    }

                }
                else
                {
                    areaid = ProvinceCityTable.Select(string.Format(" name='{0}'", _city.Text))[0][0].ToString();
                }
            }
            else
            {
                areaid = ProvinceCityTable.Select(string.Format("  name='{0}'", _region.Text))[0][0].ToString();
            }
            return areaid;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string cityId = GetProvinceCityId();
            if (cityId.Length == 0)
            {
                return;
            }
            if (btnSave.Tag != null && btnSave.Tag.ToString() == "modify")
            {
                string sql = string.Format("update t_company set companyname = '{0}',areaid = '{1}', man = '{2}',phone='{3}',mobile='{4}',fax='{5}',openflag='{6}',address='{7}' where companyid = '{8}'",
                    _company_name.Text, cityId, _principal_name.Text, _telephone.Text, _mobilePhone.Text, _fax.Text, _isUse.SelectedIndex, _address.Text,int.Parse( _company_name.Tag.ToString()));
                try
                {
                    if (dbOperation.GetDbHelper().ExecuteSql(sql) == 1)
                    {
                        Toolkit.MessageBox.Show("更新成功!");
                        Table = dbOperation.GetCompany();
                        _tableview.Table = Table;
                        clear();
                    }
                    else
                    {
                          Toolkit.MessageBox.Show("数据库读写异常,稍后尝试!!!");
                        return;
                    }

                }
                catch
                {
                    System.Diagnostics.Debug.WriteLine("UcSourceCompanyManager.btnSave_Click插入数据库异常");
                      Toolkit.MessageBox.Show("数据库读写异常,稍后尝试!!!");
                    return;
                }

            }
            else
            {

                if (CheckTextbox(_company_name, 20, "单位名称")
                    && CheckTextbox(_principal_name, 10, "单位负责人")
                    && CheckTextbox(_telephone, 12, "单位电话")
                    )
                {
                    string sql = string.Format("INSERT INTO t_company(companyname,areaid,man,phone,mobile,fax,openflag,deptid,cuserid,cdate,address) " +
                        " VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}')"
                        , _company_name.Text
                        , cityId
                        , _principal_name.Text
                        , _telephone.Text
                        , _mobilePhone.Text
                        , _fax.Text
                        , _isUse.SelectedIndex
                        , (Application.Current.Resources["User"] as UserInfo).DepartmentID
                        , (Application.Current.Resources["User"] as UserInfo).ID
                        , DateTime.Now.ToString()
                        , _address.Text);

                    try
                    {
                        if (dbOperation.GetDbHelper().ExecuteSql(sql) == 1)
                        {
                              Toolkit.MessageBox.Show("保存成功!");
                            Table = dbOperation.GetCompany();
                            _tableview.Table = Table;
                            clear();
                        }
                        else
                        {
                              Toolkit.MessageBox.Show("数据库读写异常,稍后尝试!!!");
                            return;
                        }

                    }
                    catch
                    {
                        System.Diagnostics.Debug.WriteLine("UcSourceCompanyManager.btnSave_Click插入数据库异常");
                          Toolkit.MessageBox.Show("数据库读写异常,稍后尝试!!!");
                        return;
                    }
                }
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            clear();

        }
 

        private void TabItem_GotFocus(object sender, RoutedEventArgs e)
        {
            Table = dbOperation.GetCompany();
            _tableview.Table = Table;
        }

    }
}
