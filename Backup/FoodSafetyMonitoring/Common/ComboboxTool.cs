using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Controls;

namespace FoodSafetyMonitoring.Common
{
    public class ComboboxTool
    {
        /// <summary>
        /// 初始化Combobox，绑定数据源
        /// </summary>
        /// <param name="cmb">Combobox</param>
        /// <param name="strSql">sql语句</param>
        public static void InitComboboxSource(ComboBox cmb, string strSql ,string type)
        {
            try
            {
                DBUtility.DbHelperMySQL dbHelper = DBUtility.DbHelperMySQL.CreateDbHelper();
                DataTable dt = dbHelper.GetDataSet(strSql).Tables[0];
                if (cmb != null)
                {
                    if (cmb.Items.Count > 0)
                    {
                        cmb.Items.Clear();
                    }
                }
                Label item = new Label();
                item.Tag = -1;
                if (type == "cxtj")
                {
                    item.Content = "-全部-";
                }
                else
                {
                    item.Content = "-请选择-";
                }
                
                cmb.Items.Add(item);
                cmb.SelectedItem = item;
                if (dt != null)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string strValue = dt.Rows[i][0].ToString();//隐藏值
                        string strItem = dt.Rows[i][1].ToString();//显示值
                        Label item1 = new Label();
                        item1.Tag = strValue;
                        item1.Content = strItem;
                        cmb.Items.Add(item1);
                    }
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        /// <summary>
        /// 初始化Combobox，绑定数据源
        /// </summary>
        /// <param name="cmb">Combobox</param>
        /// <param name="strSql">sql语句</param>
        /// <param name="dataSource">返回数据源（DataTable类型）</param>
        public static void InitComboboxSource(ComboBox cmb, string strSql, out DataTable dataSource)
        {
            try
            {
                DBUtility.DbHelperMySQL dbHelper = DBUtility.DbHelperMySQL.CreateDbHelper();
                DataTable dt = dbHelper.GetDataSet(strSql).Tables[0];
                dataSource = dt;
                if (cmb != null)
                {
                    if (cmb.Items.Count > 0)
                    {
                        cmb.Items.Clear();
                    }
                }
                Label item = new Label();
                item.Tag = -1;
                item.Content = "-请选择-";
                cmb.Items.Add(item);
                cmb.SelectedItem = item;
                if (dt != null)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string strValue = dt.Rows[i][0].ToString();//隐藏值
                        string strItem = dt.Rows[i][1].ToString();//显示值
                        Label item1 = new Label();
                        item1.Tag = strValue;
                        item1.Content = strItem;
                      
                        cmb.Items.Add(item1);
                    }
                }
            }
            catch (Exception)
            {
                dataSource = null;
                return;
            }
        }

        /// <summary>
        /// 初始化Combobox，绑定数据源
        /// </summary>
        /// <param name="cmb">Combobox</param>
        /// <param name="dataSource">数据源</param>
        public static void InitComboboxSource(ComboBox cmb, DataTable dataSource, string type)
        {
            try
            {
                if (cmb != null)
                {
                    if (cmb.Items.Count > 0)
                    {
                        cmb.Items.Clear();
                    }
                }
                Label item = new Label();
                item.Tag = -1;
                if (type == "cxtj")
                {
                    item.Content = "-全部-";
                }
                else
                {
                    item.Content = "-请选择-";
                }
                cmb.Items.Add(item);
                cmb.SelectedItem = item;
                if (dataSource != null)
                {
                    for (int i = 0; i < dataSource.Rows.Count; i++)
                    {
                        string strValue = dataSource.Rows[i][0].ToString();//隐藏值
                        string strItem = dataSource.Rows[i][1].ToString();//显示值
                        Label item1 = new Label();
                        item1.Tag = strValue;
                        item1.Content = strItem;
                        cmb.Items.Add(item1);
                    }
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        /// <summary>
        /// 初始化Combobox，绑定数据源
        /// </summary>
        /// <param name="cmb">Combobox</param>
        /// <param name="dataSource">数据源</param>
        public static void InitComboboxSource(ComboBox cmb, DataRow[] dataSource,string type)
        {
            try
            {
                if (cmb != null)
                {
                    if (cmb.Items.Count > 0)
                    {
                        cmb.Items.Clear();
                    }
                }
                Label item = new Label();
                item.Tag = -1;
                if (type == "cxtj")
                {
                    item.Content = "-全部-";
                }
                else
                {
                    item.Content = "-请选择-";
                }
                cmb.Items.Add(item);
                cmb.SelectedItem = item;
                if (dataSource != null)
                {
                    for (int i = 0; i < dataSource.Length; i++)
                    {
                        string strValue = dataSource[i][0].ToString();//隐藏值
                        string strItem = dataSource[i][1].ToString();//显示值
                        Label item1 = new Label();
                        item1.Tag = strValue;
                        item1.Content = strItem;
                        cmb.Items.Add(item1);
                    }
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        /// <summary>
        /// 初始化Combobox，绑定数据源
        /// </summary>
        /// <param name="cmb">Combobox</param>
        /// <param name="dataSource">数据源</param>
        public static void InitComboboxSource(ComboBox cmb, DataRow[] dataSource, int index)
        {
            try
            {
                if (cmb != null)
                {
                    if (cmb.Items.Count > 0)
                    {
                        cmb.Items.Clear();
                    }
                }
                Label item = new Label();
                item.Tag = -1;
                item.Content = "-请选择-";
                cmb.Items.Add(item);
                cmb.SelectedItem = item;
                if (dataSource != null)
                {
                    for (int i = 0; i < dataSource.Length; i++)
                    {
                        string strValue = dataSource[i][index].ToString();//隐藏值
                        string strItem = dataSource[i][index + 1].ToString();//显示值
                        Label item1 = new Label();
                        item1.Tag = strValue;
                        item1.Content = strItem;
                        cmb.Items.Add(item1);
                    }
                }
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}
