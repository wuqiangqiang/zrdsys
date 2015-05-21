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
using Toolkit = Microsoft.Windows.Controls;
using System.IO;
using System.Threading;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// SysLogManager.xaml 的交互逻辑
    /// </summary>
    public partial class SysLogManager : UserControl
    {
        DBUtility.DbHelperMySQL dbHelper = null;
        private DataTable currentTable = new DataTable();
        public SysLogManager()
        {
            InitializeComponent();
            cmbOperationType.SelectedIndex = 0;
        }

        private void BindData()
        {
            StringBuilder sbSql = new StringBuilder("Select NUMB_SYSLOG,FK_NAME_MENU,FLAG_LOGSORT,FK_NAME_USER,INFO_CONT,INFO_DATE FROM sys_client_syslog WHERE 1=1 ");
            if (txtUserName.Text.Trim() != "")
            {
                sbSql.Append(" AND FK_NAME_USER='" + txtUserName.Text.Trim() + "' ");
            }

            if (cmbOperationType.SelectedIndex > 0)
            {
                sbSql.Append(" AND FLAG_LOGSORT='" + (cmbOperationType.SelectedItem as ComboBoxItem).Content.ToString() + "' ");
            }

            sbSql.Append(" AND INFO_DATE>='" + Convert.ToDateTime(dtpStartDate.Value).ToShortDateString() + " 00:00:01" + "' AND INFO_DATE<='" + Convert.ToDateTime(dtpEndDate.Value).ToShortDateString() + " 23:59:59" + "'");

            try
            {
                dbHelper = DBUtility.DbHelperMySQL.CreateDbHelper();
                lvlist.DataContext = dbHelper.GetDataSet(sbSql.ToString()).Tables[0];
                currentTable = dbHelper.GetDataSet(sbSql.ToString()).Tables[0];
            }
            catch (Exception)
            {
                return;
            }
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            BindData();
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            if (currentTable.Rows.Count == 0)
            {
                Toolkit.MessageBox.Show("没有任何数据供导出！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
                sfd.Filter = "导出文件 (*.xls)|*.xls";
                sfd.FilterIndex = 0;
                sfd.RestoreDirectory = true;
                sfd.Title = "导出文件保存路径";
                sfd.ShowDialog();
                string strFilePath = sfd.FileName;
                if (strFilePath != "")
                {
                    StringBuilder strValue = new StringBuilder();
                    if (File.Exists(strFilePath))
                    {
                        File.Delete(strFilePath);
                    }
                    Thread thread = new Thread(() =>
                    {
                        ExportToExcel(strFilePath);
                    });
                    thread.Start();
                }
                else
                {
                    return;
                }
            }
            catch (Exception)
            {
                Toolkit.MessageBox.Show("导出失败！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
        }

        private void ExportToExcel(string path)
        {
            try
            {
                HSSFWorkbook workbook = new HSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("系统日志报表");

                //第一行样式及内容
                IRow row = sheet.CreateRow(0);

                row.HeightInPoints = 40;

                ICell cell = row.CreateCell(0, CellType.String);

                cell.SetCellValue("系统日志报表");

                ICellStyle style = workbook.CreateCellStyle();

                IFont font = workbook.CreateFont();

                font.FontHeightInPoints = 18;

                font.FontName = "宋体";

                font.Boldweight = 800;

                style.SetFont(font);

                style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;

                style.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Justify;

                cell.CellStyle = style;

                sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 5));

                //设置列宽
                sheet.SetColumnWidth(0, 12 * 256);
                for (int i = 1; i < 6; i++)
                {
                    sheet.SetColumnWidth(i, 20 * 256);
                }

                ICellStyle style2 = workbook.CreateCellStyle();
                style2.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;
                style2.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Justify;
                IFont font2 = workbook.CreateFont();
                font2.FontHeightInPoints = 12;
                font2.FontName = "宋体";
                style2.SetFont(font2);

                //第二行(表头)
                IRow row2 = sheet.CreateRow(1);

                ICell headCell1 = row2.CreateCell(0, CellType.String);
                headCell1.CellStyle = style2;
                headCell1.SetCellValue("流水号");

                ICell headCell2 = row2.CreateCell(1, CellType.String);
                headCell2.CellStyle = style2;
                headCell2.SetCellValue("菜单名称");

                ICell headCell3 = row2.CreateCell(2, CellType.String);
                headCell3.CellStyle = style2;
                headCell3.SetCellValue("操作类型");

                ICell headCell4 = row2.CreateCell(3, CellType.String);
                headCell4.CellStyle = style2;
                headCell4.SetCellValue("操作时间");

                ICell headCell5 = row2.CreateCell(4, CellType.String);
                headCell5.SetCellValue("操作内容");
                headCell5.CellStyle = style2;

                ICell headCell6 = row2.CreateCell(5, CellType.String);
                headCell6.SetCellValue("操作用户");
                headCell6.CellStyle = style2;


                //填充数据到单元格
                for (int i = 0; i < currentTable.Rows.Count; i++)
                {
                    IRow irow = sheet.CreateRow(i + 2);
                    ICell cell1 = irow.CreateCell(0, CellType.Numeric);
                    cell1.SetCellValue(Convert.ToInt32(currentTable.Rows[i]["NUMB_SYSLOG"]));

                    ICell cell2 = irow.CreateCell(1, CellType.String);
                    cell2.SetCellValue(currentTable.Rows[i]["FK_NAME_MENU"].ToString());

                    ICell cell3 = irow.CreateCell(2, CellType.String);
                    cell3.SetCellValue(currentTable.Rows[i]["FLAG_LOGSORT"].ToString());

                    ICell cell4 = irow.CreateCell(3, CellType.String);
                    cell4.SetCellValue((Convert.ToDateTime(currentTable.Rows[i]["INFO_DATE"])).ToString("yyyy-MM-dd HH:mm:ss"));

                    ICell cell5 = irow.CreateCell(4, CellType.String);
                    cell5.SetCellValue(currentTable.Rows[i]["INFO_CONT"].ToString());

                    ICell cell6 = irow.CreateCell(5, CellType.String);
                    cell6.SetCellValue(currentTable.Rows[i]["FK_NAME_USER"].ToString());
                }

                FileStream file = new FileStream(path, FileMode.Create);

                workbook.Write(file);

                file.Close();

                this.Dispatcher.Invoke(new Action(() =>
                {
                    Toolkit.MessageBox.Show("导出成功！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }));

            }
            catch (Exception)
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    Toolkit.MessageBox.Show("导出失败！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }));
            }
        }
    }
}
