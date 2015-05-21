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
using FoodSafetyMonitoring.Common;
using System.Data;
using FoodSafetyMonitoring.Manager.UserControls;

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class SysDesignReport : UserControl
    {
        private IDBOperation dbOperation;
        private Dictionary<string, MyColumn> MyColumns = new Dictionary<string, MyColumn>();
        private string user_flag_tier;
        private string dept_name;

        public SysDesignReport(IDBOperation dbOperation)
        {
            InitializeComponent();

            this.dbOperation = dbOperation;
            user_flag_tier = (Application.Current.Resources["User"] as UserInfo).FlagTier;

            reportDate_kssj.Value = DateTime.Now.AddDays(-1);
            reportDate_jssj.Value = DateTime.Now;
            //检测站点
            switch (user_flag_tier)
            {
                case "0": _dept_name.Text = "选择省:";
                    dept_name = "省名称";
                    break;
                case "1": _dept_name.Text = "选择地市:";
                    dept_name = "地市名称";
                    break;
                case "2": _dept_name.Text = "选择区县:";
                    dept_name = "区县名称";
                    break;
                case "3": _dept_name.Text = "选择检测站点:";
                    dept_name = "检测站点名称";
                    break;
                case "4": _dept_name.Text = "选择检测站点:";
                    dept_name = "检测站点名称";
                    break;
                default: break;
            }
            ComboboxTool.InitComboboxSource(_detect_dept, "call p_dept_cxtj(" + (Application.Current.Resources["User"] as UserInfo).ID + ")", "cxtj");
            //检测点属性
            //ComboboxTool.InitComboboxSource(_detect_type, "SELECT typeId,typeName FROM t_dept_type where openFlag = '1'", "cxtj");
            //检测项目
            ComboboxTool.InitComboboxSource(_detect_item, "SELECT ItemID,ItemNAME FROM t_det_item WHERE  (tradeId ='1'or tradeId ='2' or tradeId ='3' or ifnull(tradeId,'') = '') and OPENFLAG = '1' order by orderId", "cxtj");
            //检测对象
            ComboboxTool.InitComboboxSource(_detect_object, "SELECT objectId,objectName FROM t_det_object WHERE  (tradeId ='1'or tradeId ='2' or tradeId ='3' or ifnull(tradeId,'') = '') and OPENFLAG = '1'", "cxtj");
            //检测结果
            ComboboxTool.InitComboboxSource(_detect_result, "SELECT resultId,resultName FROM t_det_result where openFlag='1' ORDER BY id", "cxtj");

            //如果登录用户的部门是站点级别，则将查询条件检测站点赋上默认值
            if (isDept())
            {
                _detect_dept.SelectedIndex = 1;
            }

            SetColumns();

        }

        //判断登录用户的部门级别
        private bool isDept()
        {
            string flag = dbOperation.GetDbHelper().GetSingle("select FLAG_TIER from sys_client_sysdept where INFO_CODE =" + (Application.Current.Resources["User"] as UserInfo).DepartmentID).ToString();

            if (flag == "4")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void _query_Click(object sender, RoutedEventArgs e)
        {
            _tableview.GetDataByPageNumberEvent += new UcTableOperableView.GetDataByPageNumberEventHandler(_tableview_GetDataByPageNumberEvent);
            GetData();
            _tableview.Title = string.Format("数据统计时间:{0}年{1}月{2}日到{3}年{4}月{5}日", reportDate_kssj.Value.Value.Year, reportDate_kssj.Value.Value.Month, reportDate_kssj.Value.Value.Day,
                               reportDate_jssj.Value.Value.Year, reportDate_jssj.Value.Value.Month, reportDate_jssj.Value.Value.Day);
            _tableview.PageIndex = 1;
        }

        private void SetColumns()
        {
            MyColumns.Add("part_id", new MyColumn("part_id", "检测站点id") { BShow = false });
            MyColumns.Add("part_name", new MyColumn("part_name", dept_name) { BShow = true, Width = 18 });
            //MyColumns.Add("type_id", new MyColumn("type_id", "检测点属性id") { BShow = false });
            //MyColumns.Add("type_name", new MyColumn("type_name", "检测点属性") { BShow = true, Width = 12 });
            MyColumns.Add("itemid", new MyColumn("itemid", "检测项目id") { BShow = false });
            MyColumns.Add("itemname", new MyColumn("itemname", "检测项目") { BShow = true, Width = 14 });
            MyColumns.Add("objectid", new MyColumn("objectid", "检测对象id") { BShow = false });
            MyColumns.Add("objectname", new MyColumn("objectname", "检测对象") { BShow = true, Width = 14 });
            MyColumns.Add("resultid", new MyColumn("resultid", "检测结果id") { BShow = false });
            MyColumns.Add("resultname", new MyColumn("resultname", "检测结果") { BShow = true, Width = 14 });
            MyColumns.Add("count", new MyColumn("count", "数量(单位：份次)") { BShow = true, Width = 16 });
            MyColumns.Add("sum_num", new MyColumn("sum_num", "总行数") { BShow = false });

            _tableview.MyColumns = MyColumns;
            _tableview.BShowModify = false;
            _tableview.BShowDelete = false;
            
            
        }


        private void GetData()
        {
            DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_report_custom('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}',{8})",
                              (Application.Current.Resources["User"] as UserInfo).ID, reportDate_kssj.Value, reportDate_jssj.Value,
                               _detect_dept.SelectedIndex < 1 ? "" : (_detect_dept.SelectedItem as Label).Tag,
                               _detect_item.SelectedIndex < 1 ? "" : (_detect_item.SelectedItem as Label).Tag,
                               _detect_object.SelectedIndex < 1 ? "" : (_detect_object.SelectedItem as Label).Tag,
                               _detect_result.SelectedIndex < 1 ? "" : (_detect_result.SelectedItem as Label).Tag,
                               (_tableview.PageIndex - 1) * _tableview.RowMax,
                               _tableview.RowMax)).Tables[0];
             
            //table.Columns.Remove("PART_ID");
            //table.Columns.Remove("type_id");
            //table.Columns.Remove("ITEMID");
            //table.Columns.Remove("objectid");
            //table.Columns.Remove("RESULTID");

            //table.Columns[0].ColumnName = "检测站点";
            //table.Columns[1].ColumnName = "检测点属性";
            //table.Columns[2].ColumnName = "检测项目";
            //table.Columns[3].ColumnName = "检测对象";
            //table.Columns[4].ColumnName = "检测结果";
            //table.Columns[5].ColumnName = "数量(单位：份次)";


            ////表格最后添加合计行
            //table.Rows.Add(table.NewRow()[1] = "合计");
            //int sum = 0;
            //for (int i = 0; i < table.Rows.Count - 1; i++)
            //{
            //    sum += Convert.ToInt32(table.Rows[i][10].ToString());
            //}
            ////sum_column += sum;
            //table.Rows[table.Rows.Count - 1][10] = sum;

            ////表格的标题
            //string title = "";
            //title = string.Format("▪ 数据统计时间:{0}年{1}月{2}日到{3}年{4}月{5}日", reportDate_kssj.Value.Value.Year, reportDate_kssj.Value.Value.Month, reportDate_kssj.Value.Value.Day,
            //              reportDate_jssj.Value.Value.Year, reportDate_jssj.Value.Value.Month, reportDate_jssj.Value.Value.Day);
            //_tableview.SetDataTable(table, title, new List<int>());

            _tableview.Table = table;
        }

        void _tableview_GetDataByPageNumberEvent()
        {
            GetData();
        }

        private void _export_Click(object sender, RoutedEventArgs e)
        {
            _tableview.ExportExcel();
        }
    }
}
