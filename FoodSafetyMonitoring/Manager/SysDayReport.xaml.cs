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
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class SysDayReport : UserControl
    {
        private IDBOperation dbOperation;
        private string user_flag_tier;
        private DataTable currenttable;
        private string depttype;
        private string detecttype;

        private List<DeptItemInfo> list = new List<DeptItemInfo>();

        public SysDayReport(IDBOperation dbOperation,string dept_type,string detect_type)
        {
            InitializeComponent();

            this.dbOperation = dbOperation;
            this.depttype = dept_type;
            this.detecttype = detect_type;
            user_flag_tier = (Application.Current.Resources["User"] as UserInfo).FlagTier;

            reportDate.SelectedDate = DateTime.Now.AddDays(-1);
            switch (user_flag_tier)
            {
                case "0": _dept_name.Text = "选择省:";
                    break;
                case "1": _dept_name.Text = "选择市(州):";
                    break;
                case "2": _dept_name.Text = "选择区县:";
                    break;
                case "3": _dept_name.Text = "选择检测单位:";
                    break;
                case "4": _dept_name.Text = "选择检测单位:";
                    break;
                default: break;
            }
            //检测单位
            ComboboxTool.InitComboboxSource(_detect_dept, string.Format("call p_dept_cxtj_hb({0},'{1}')", (Application.Current.Resources["User"] as UserInfo).ID, depttype), "cxtj");
            
            //2015-08-11 根据不同检测模块显示不同的检测项目
            if (detecttype == "3")//饲料检测
            {
                ComboboxTool.InitComboboxSource(_detect_item, "SELECT ItemID,ItemNAME FROM t_det_item_hb WHERE  (tradeId ='0' or ifnull(tradeId,'') = '') and OPENFLAG = '1'", "cxtj");
            }
            else if (detecttype == "2")//同步检测
            {
                ComboboxTool.InitComboboxSource(_detect_item, "SELECT ItemID,ItemNAME FROM t_det_item_hb WHERE  (tradeId ='2' or ifnull(tradeId,'') = '') and OPENFLAG = '1'", "cxtj");
            }
            else//其余检测
            {
                ComboboxTool.InitComboboxSource(_detect_item, "SELECT ItemID,ItemNAME FROM t_det_item_hb WHERE  (tradeId ='1' or ifnull(tradeId,'') = '') and OPENFLAG = '1'", "cxtj");
            }

            //2015-08-11 根据不同检测模块显示不同的检测结果
            if (detecttype == "2")//同步检测
            {
                ComboboxTool.InitComboboxSource(_detect_result, "SELECT resultId,resultName FROM t_det_result_hb  where tradeid = '1' and openFlag='1' ORDER BY id", "cxtj");
            }
            else//其余检测
            {
                ComboboxTool.InitComboboxSource(_detect_result, "SELECT resultId,resultName FROM t_det_result_hb  where tradeid = '0' and openFlag='1' ORDER BY id", "cxtj");
            }

            

            //如果登录用户的部门是站点级别，则将查询条件检测单位赋上默认值
            if (isDept())
            {
                _detect_dept.SelectedIndex = 1;
            }

            _tableview.DetailsRowEnvent += new UcTableView_NoTitle.DetailsRowEventHandler(_tableview_DetailsRowEnvent);
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
            DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_report_day_hb('{0}','{1}','{2}','{3}','{4}','{5}','{6}')",
                              (Application.Current.Resources["User"] as UserInfo).ID, reportDate.SelectedDate,
                               _detect_dept.SelectedIndex < 1 ? "" : (_detect_dept.SelectedItem as Label).Tag,
                               _detect_item.SelectedIndex < 1 ? "" : (_detect_item.SelectedItem as Label).Tag,
                               _detect_result.SelectedIndex < 1 ? "" : (_detect_result.SelectedItem as Label).Tag,
                               depttype, detecttype)).Tables[0];

            currenttable = table;
            list.Clear();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                DeptItemInfo info = new DeptItemInfo();
                //info.DeptId = table.Rows[i][0].ToString();
                info.DeptName = table.Rows[i][1].ToString();
                //info.ItemId = table.Rows[i][2].ToString();
                info.ItemName = table.Rows[i][3].ToString();
                info.Count = table.Rows[i][4].ToString();
                info.Sum = table.Rows[i][5].ToString();
                info.Yin = table.Rows[i][6].ToString();
                info.Yang = table.Rows[i][7].ToString();
                info.Yisi = table.Rows[i][8].ToString();
                list.Add(info);
            }

            //得到行和列标题 及数量            
            string[] DeptNames = list.Select(t => t.DeptName).Distinct().ToArray();
            string[] ItemNames = list.Select(t => t.ItemName).Distinct().ToArray();

            //创建DataTable
            DataTable tabledisplay = new DataTable();

            //表中第一行第一列交叉处一般显示为第1列标题
            tabledisplay.Columns.Add(new DataColumn("序号"));
            switch (user_flag_tier)
            {
                case "0": tabledisplay.Columns.Add(new DataColumn("省名称"));
                    break;
                case "1": tabledisplay.Columns.Add(new DataColumn("市(州)名称"));
                    break;
                case "2": tabledisplay.Columns.Add(new DataColumn("区县名称"));
                    break;
                case "3": tabledisplay.Columns.Add(new DataColumn("检测单位名称"));
                    break;
                case "4": tabledisplay.Columns.Add(new DataColumn("检测单位名称"));
                    break;
                default: break;
            }

            //表中后面每列的标题其实是列分组的关键字
            for (int i = 0; i < ItemNames.Length; i++)
            {
                DataColumn column = new DataColumn(ItemNames[i]);
                tabledisplay.Columns.Add(column);
            }
            //表格后面为合计列
            tabledisplay.Columns.Add(new DataColumn("合计"));
            //屠宰检测结果为有无
            //if (detecttype == "2")
            //{
            //    tabledisplay.Columns.Add(new DataColumn("无"));
            //    tabledisplay.Columns.Add(new DataColumn("有"));
            //    //tabledisplay.Columns.Add(new DataColumn("阳性样本"));
            //}
            //else
            //{
                tabledisplay.Columns.Add(new DataColumn("阴性样本"));
                tabledisplay.Columns.Add(new DataColumn("疑似阳性样本"));
                tabledisplay.Columns.Add(new DataColumn("阳性样本"));
            //}
            

            //为表中各行生成数据
            for (int i = 0; i < DeptNames.Length; i++)
            {
                var row = tabledisplay.NewRow();
                //每行第0列为行分组关键字
                row[0] = i + 1 ;
                row[1] = DeptNames[i];
                //每行的其余列为行列交叉对应的汇总数据
                for (int j = 0; j < ItemNames.Length; j++)
                {
                    string num = list.Where(t => t.DeptName == DeptNames[i] && t.ItemName == ItemNames[j]).Select(t => t.Count).FirstOrDefault();
                    if (num == null ||  num == "")
                    {
                        num = '0'.ToString();
                    }
                    row[ItemNames[j]] = num;
                }
                row[ItemNames.Length + 2] = list.Where(t => t.DeptName == DeptNames[i] ).Select(t => t.Sum).FirstOrDefault();
                row[ItemNames.Length + 3] = list.Where(t => t.DeptName == DeptNames[i] ).Select(t => t.Yin).FirstOrDefault();
                row[ItemNames.Length + 4] = list.Where(t => t.DeptName == DeptNames[i] ).Select(t => t.Yisi).FirstOrDefault();
                row[ItemNames.Length + 5] = list.Where(t => t.DeptName == DeptNames[i]).Select(t => t.Yang).FirstOrDefault();

                tabledisplay.Rows.Add(row);
            }

            //计算报表总条数
            int row_count = 0 ;

            if (table.Rows.Count != 0)
            {
                //表格最后添加合计行
                tabledisplay.Rows.Add(tabledisplay.NewRow()[1] = "合计");
                for (int j = 2; j < tabledisplay.Columns.Count; j++)
                {
                    int sum = 0;
                    for (int i = 0; i < tabledisplay.Rows.Count - 1 ; i++)
                    {
                        sum += Convert.ToInt32(tabledisplay.Rows[i][j].ToString());
                    }
                    //sum_column += sum;
                    tabledisplay.Rows[tabledisplay.Rows.Count - 1][j] = sum;
                }

                row_count =  tabledisplay.Rows.Count - 1;
            }
            else
            {
                row_count = 0 ;
            }
            
            

            //表格的标题
            string title = "";
            //title = string.Format("{0}年{1}月{2}日  检测数据日报表（单位：份次） 合计{3}条数据", reportDate.SelectedDate.Value.Year, reportDate.SelectedDate.Value.Month, reportDate.SelectedDate.Value.Day, row_count);

            _tableview.BShowDetails = true;
            _tableview.SetDataTable(tabledisplay,title, new List<int>());
            _sj.Visibility = Visibility.Visible;
            _hj.Visibility = Visibility.Visible;
            _title.Text = row_count.ToString();

            if (row_count == 0)
            {
                Toolkit.MessageBox.Show("没有查询到数据！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

        }

        void _tableview_DetailsRowEnvent(string id)
        {
            string dept_id;
            string item_id;
            string result_id;

            DataRow[] rows = currenttable.Select("PART_NAME = '" + id + "'");
            dept_id = rows[0]["PART_ID"].ToString();
            item_id = _detect_item.SelectedIndex < 1 ? "" : (_detect_dept.SelectedItem as Label).Tag.ToString();
            result_id = _detect_result.SelectedIndex < 1 ? "" : (_detect_result.SelectedItem as Label).Tag.ToString();

            grid_info.Children.Add(new UcDayReportDetails(dbOperation, reportDate.SelectedDate.ToString(), dept_id, item_id, result_id, detecttype));
        }

        private void _export_Click(object sender, RoutedEventArgs e)
        {
            _tableview.ExportExcel();
        }

        [Serializable]
        public class DeptItemInfo
        {
            //public string DeptId { get; set; }
            
            public string DeptName { get; set; }

            //public string ItemId { get; set; }

            public string ItemName { get; set; }

            public string Count { get; set; }

            public string Sum { get; set; }

            public string Yin { get; set; }

            public string Yang { get; set; }

            public string Yisi { get; set; }


        }
    }
}
