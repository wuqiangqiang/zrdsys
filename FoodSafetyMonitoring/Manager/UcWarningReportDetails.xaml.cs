﻿using System;
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

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// UcWarningReportDetails.xaml 的交互逻辑
    /// </summary>
    public partial class UcWarningReportDetails : UserControl
    {
        private IDBOperation dbOperation;
        private Dictionary<string, MyColumn> MyColumns = new Dictionary<string, MyColumn>();
        public string Kssj { get; set; }
        public string Jssj { get; set; }
        public string DeptId { get; set; }
        public string DeptName { get; set; }
        public string ItemId { get; set; }
        public string ReviewFlag { get; set; }
        public string DetectType { get; set; }

        public UcWarningReportDetails(IDBOperation dbOperation, string kssj, string jssj, string dept_id, string item_id, string review_id, string dept_name, string detecttype)
        {
            InitializeComponent();

            this.dbOperation = dbOperation;

            this.Kssj = kssj;
            this.Jssj = jssj;
            this.DeptId = dept_id;
            this.ItemId = item_id;
            this.ReviewFlag = review_id;
            this.DeptName = dept_name;
            this.DetectType = detecttype;

            MyColumns.Add("orderid", new MyColumn("orderid", "检测单编号") { BShow = true, Width = 8 });
            MyColumns.Add("reviewflagname", new MyColumn("reviewflagname", "复核标志") { BShow = false, Width = 8 });
            MyColumns.Add("detecttypename", new MyColumn("detecttypename", "信息来源") { BShow = true, Width = 8 });
            MyColumns.Add("detectdate", new MyColumn("detectdate", "检测时间") { BShow = true, Width = 16 });
            MyColumns.Add("partname", new MyColumn("partname", DeptName) { BShow = true, Width = 16 });
            MyColumns.Add("itemname", new MyColumn("itemname", "检测项目") { BShow = true, Width = 12 });
            MyColumns.Add("objectname", new MyColumn("objectname", "检测对象") { BShow = true, Width = 8 });
            MyColumns.Add("samplename", new MyColumn("samplename", "检测样本") { BShow = true, Width = 8 });
            MyColumns.Add("sensitivityname", new MyColumn("sensitivityname", "检测灵敏度") { BShow = true, Width = 8 });
            MyColumns.Add("reagentname", new MyColumn("reagentname", "检测方法") { BShow = true, Width = 10 });
            MyColumns.Add("resultname", new MyColumn("resultname", "检测结果") { BShow = true, Width = 8 });
            MyColumns.Add("detectusername", new MyColumn("detectusername", "检测师") { BShow = true, Width = 8 });
            MyColumns.Add("areaname", new MyColumn("areaname", "来源产地") { BShow = false});
            MyColumns.Add("companyname", new MyColumn("companyname", "被检单位") { BShow = true, Width = 16 });
            MyColumns.Add("sum_num", new MyColumn("sum_num", "总行数") { BShow = false });

            _tableview.MyColumns = MyColumns;
            _tableview.BShowState = true;
            _tableview.GetDataByPageNumberEvent += new UcTableOperableView_NoTitle.GetDataByPageNumberEventHandler(_tableview_GetDataByPageNumberEvent);
            _tableview.StateRowEnvent += new UcTableOperableView_NoTitle.StateRowEventHandler(_tableview_StateRowEnvent);
            _tableview.PageIndex = 1;
            GetData();
        }

        private void GetData()
        {
            DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_warning_report_details_hb('{0}','{1}','{2}','{3}','{4}','{5}',{6},{7})",
                               Kssj, Jssj, DeptId, ItemId, ReviewFlag, DetectType,
                              (_tableview.PageIndex - 1) * _tableview.RowMax,
                              _tableview.RowMax)).Tables[0];

            _tableview.Table = table;
        }

        void _tableview_GetDataByPageNumberEvent()
        {
            GetData();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        void _tableview_StateRowEnvent(string id)
        {
            int orderid = int.Parse(id);
            DataTable table =  dbOperation.GetDbHelper().GetDataSet(string.Format("select ReviewFlag,DetectFlag from t_detect_report where ORDERID = '{0}'", orderid)).Tables[0];
            string reviewflag = table.Rows[0][0].ToString();
            string DetectFlag = table.Rows[0][1].ToString();
            if (reviewflag == "1")
            {
                if (DetectFlag == "0")
                {
                    Culture_DetectDetailsReview det = new Culture_DetectDetailsReview(dbOperation, orderid);
                    det.ShowDialog();
                }
                else if (DetectFlag == "1")
                {
                    Certificate_DetectDetailsReview det = new Certificate_DetectDetailsReview(dbOperation, orderid);
                    det.ShowDialog();
                }
                else if (DetectFlag == "2")
                {
                    Slaughter_DetectDetailsReview det = new Slaughter_DetectDetailsReview(dbOperation, orderid);
                    det.ShowDialog();
                }
                else if (DetectFlag == "3")
                {
                    Feed_DetectDetailsReview det = new Feed_DetectDetailsReview(dbOperation, orderid);
                    det.ShowDialog();
                }
                else if (DetectFlag == "")
                {
                    detectDetailsReview det = new detectDetailsReview(dbOperation, orderid);
                    det.ShowDialog();
                }      
            }
            else
            {
                if (DetectFlag == "0")
                {
                    culture_detectdetails det = new culture_detectdetails(dbOperation, orderid);
                    det.ShowDialog();
                }
                else if (DetectFlag == "1")
                {
                    Certificate_detectdetails det = new Certificate_detectdetails(dbOperation, orderid);
                    det.ShowDialog();
                }
                else if (DetectFlag == "2")
                {
                    Slaughter_detectdetails det = new Slaughter_detectdetails(dbOperation, orderid);
                    det.ShowDialog();
                }
                else if (DetectFlag == "3")
                {
                    Feed_detectdetails det = new Feed_detectdetails(dbOperation, orderid);
                    det.ShowDialog();
                }
                else if (DetectFlag == "")
                {
                    detectdetails det = new detectdetails(dbOperation, orderid);
                    det.ShowDialog();
                }      
            }
            
        }
    }
}
