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
using System.Windows.Forms.Integration;
using System.Data;
using FoodSafetyMonitoring.Common;
using FoodSafetyMonitoring.Manager.UserControls;
using Toolkit = Microsoft.Windows.Controls;

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// SysCityGrade.xaml 的交互逻辑
    /// </summary>
    public partial class SysCityGrade : UserControl
    {
        private IDBOperation dbOperation;
        public SysCityGrade(IDBOperation dbOperation)
        {
            InitializeComponent();

            this.dbOperation = dbOperation;

            dtpStartDate.SelectedDate = DateTime.Now.AddDays(-1);
            dtpEndDate.SelectedDate = DateTime.Now;

            DataTable table_detect_type = new DataTable();
            table_detect_type.Columns.Add("id", Type.GetType("System.String"));
            table_detect_type.Columns.Add("name", Type.GetType("System.String"));
            table_detect_type.Rows.Add(new object[] { "3", "饲料检测" });
            table_detect_type.Rows.Add(new object[] { "0", "养殖检测" });
            table_detect_type.Rows.Add(new object[] { "1", "出证检测" });
            table_detect_type.Rows.Add(new object[] { "4", "宰前检测" });
            table_detect_type.Rows.Add(new object[] { "2", "屠宰检测" });
            ComboboxTool.InitComboboxSource(_detect_type, table_detect_type, "cxtj");

            load();
        }

        private void load()
        {
            //创建DataTable
            DataTable tabledisplay = new DataTable();

            //表中第一行第一列交叉处一般显示为第1列标题
            tabledisplay.Columns.Add(new DataColumn("id"));
            tabledisplay.Columns.Add(new DataColumn("city"));
            tabledisplay.Columns.Add(new DataColumn("num1"));
            tabledisplay.Columns.Add(new DataColumn("num2"));
            tabledisplay.Columns.Add(new DataColumn("num3"));
            tabledisplay.Columns.Add(new DataColumn("num4"));

            DataTable table = dbOperation.GetDbHelper().GetDataSet("select name from sys_city where pid = '42'").Tables[0];
            


            //为表中各行生成数据
            for (int i = 0; i < table.Rows.Count; i++)
            {
                var row = tabledisplay.NewRow();
                //每行第0列为行分组关键字
                row[0] = i + 1;
                row[1] = table.Rows[i][0];
                row[2] = "";
                row[3] = "";
                row[4] = "";
                row[5] = "";

                tabledisplay.Rows.Add(row);
            }
            lvlist.DataContext = tabledisplay;
        }

        private void _query_Click(object sender, RoutedEventArgs e)
        {

        }

        private void _export_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
