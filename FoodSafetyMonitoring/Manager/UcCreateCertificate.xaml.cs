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
                DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_certificate_main('{0}')", (_source_company.SelectedItem as Label).Tag.ToString())).Tables[0];


            }
        }
    }
}
