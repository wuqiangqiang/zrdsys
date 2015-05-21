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
using System.IO;
using MySql.Data.MySqlClient;

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// SysPersonManager.xaml 的交互逻辑
    /// </summary>
    public partial class SysPersonManager : UserControl
    {
        private DbHelperMySQL dbHelper = null;
        private DataTable currentTable = new DataTable();
        private DataTable bedTable = new DataTable();
        private DataTable areainfoTable = new DataTable();
        private bool Flag = true;//设置标记，控制区域下拉框的改变

        public SysPersonManager()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(SysPersonManager_Loaded);
        }

        void SysPersonManager_Loaded(object sender, RoutedEventArgs e)
        {
            dbHelper = DbHelperMySQL.CreateDbHelper();
            this.txtPersonCode.Text = CreateNewNumber();
            InitComboboxs();
            BindData();
            Clear();
        }

        private void BindData()
        {
            string strSql = "SELECT * FROM sys_client_person";
            try
            {
                currentTable = dbHelper.GetDataSet(strSql).Tables[0];
                this.lvlist.DataContext = currentTable.DefaultView;
            }
            catch (Exception)
            {
                return;
            }
        }

        private void InitComboboxs()
        {
            ////初始化部门 下拉框
            //ComboboxTool.InitComboboxSource(this.cmbDept, "SELECT INFO_CODE,INFO_NAME from sys_client_sysdept", "lr");

            //try
            //{
            //    DataTable dt = dbHelper.GetDataSet("SELECT INFO_CODE,INFO_NAME,INFO_GBCODE from sys_client_datadict where INFO_GBCODE in ('109','110') and INFO_USE=1").Tables[0];
            //    //初始化人员类型 下拉框
            //    ComboboxTool.InitComboboxSource(this.cmbPersonType, dt.Select("INFO_GBCODE='109'"));
            //    ComboboxTool.InitComboboxSource(this.cmbPersonTypeForSelect, dt.Select("INFO_GBCODE='109'"));
            //    //初始化人员属性 下拉框
            //    ComboboxTool.InitComboboxSource(this.cmbPersonProperty, dt.Select("INFO_GBCODE='110'"));

            //    //初始化区域
            //    areainfoTable = dbHelper.GetDataSet("SELECT INFO_AREA,INFO_NAME,FK_CODE_AREA,FK_NAME_AREA,FK_INFO_CODE,FK_INFO_NAME from sys_client_sysarea").Tables[0];
            //    ComboboxTool.InitComboboxSource(cmbArea, areainfoTable.Select(" FK_INFO_NAME like '%区域%'"));
            //    ComboboxTool.InitComboboxSource(cmbBuildingNo, areainfoTable.Select("  FK_INFO_NAME like '%建筑%'"));
            //    ComboboxTool.InitComboboxSource(cmbFloor, areainfoTable.Select("  FK_INFO_NAME like '%楼层%'"));
            //    ComboboxTool.InitComboboxSource(cmbLocation, areainfoTable.Select("  FK_INFO_NAME not like '%区域%' and FK_INFO_NAME not like '%建筑%' and FK_INFO_NAME not like '%楼层%'"));

            //    //初始化床位
            //    ComboboxTool.InitComboboxSource(cmbBedNo, "SELECT INFO_CODE,INFO_NAME,FK_INFO_AREA,FK_INFO_NAME from sys_client_bed", out bedTable);

            //}
            //catch (Exception)
            //{
            //    return;
            //}
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //string personTypeCode = string.Empty;
            //string personTypeName = string.Empty;
            //if (this.cmbPersonType.SelectedIndex <= 0)
            //{
            //    txtMsg.Text = "*未选择人员类型！";
            //    txtMsg.Tag = "cmbPersonType";
            //    return;
            //}
            //else
            //{
            //    personTypeCode = (this.cmbPersonType.SelectedItem as Label).Tag.ToString();
            //    personTypeName = (this.cmbPersonType.SelectedItem as Label).Content.ToString();
            //}

            //if (this.txtPersonName.Text.Trim() == "")
            //{
            //    txtMsg.Text = "*未选择人员类型！";
            //    txtMsg.Tag = "txtPersonName";
            //    return;
            //}


            //string roomCode = string.Empty;
            //string roomName = string.Empty;
            //if (this.cmbLocation.SelectedIndex <= 0)
            //{
            //    txtMsg.Text = "*未选择在园位置！";
            //    txtMsg.Tag = "cmbLocation";
            //    return;
            //}
            //else
            //{
            //    roomCode = (this.cmbLocation.SelectedItem as Label).Tag.ToString();
            //    roomName = (this.cmbLocation.SelectedItem as Label).Content.ToString();
            //}

            //string deptCode = string.Empty;
            //string deptName = string.Empty;
            //if (this.cmbDept.SelectedIndex > 0)
            //{
            //    deptCode = (this.cmbDept.SelectedItem as Label).Tag.ToString();
            //    deptName = (this.cmbDept.SelectedItem as Label).Content.ToString();
            //}

            //string postCode = string.Empty;
            //string postName = string.Empty;
            //if (this.cmbPersonProperty.SelectedIndex > 0)
            //{
            //    postCode = (this.cmbPersonProperty.SelectedItem as Label).Tag.ToString();
            //    postName = (this.cmbPersonProperty.SelectedItem as Label).Content.ToString();
            //}

            //string bedCode = string.Empty;
            //string bedName = string.Empty;
            //if (this.cmbBedNo.SelectedIndex > 0)
            //{
            //    bedCode = (this.cmbBedNo.SelectedItem as Label).Tag.ToString();
            //    bedName = (this.cmbBedNo.SelectedItem as Label).Content.ToString();
            //}

            //string sex = rdbMale.IsChecked == true ? "男" : "女";

            //byte[] array = null;
            //if (this.imgPhoto.Source != null && this.imgPhoto.Tag != null)
            //{
            //    array = SetImageToByteArray((BitmapImage)this.imgPhoto.Tag);
            //    if (array.Length > 64000)
            //    {
            //        Toolkit.MessageBox.Show("图片过大！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
            //        return;
            //    }
            //}

            //if (IsBedUsed(bedCode, this.btnSave.Tag))
            //{
            //    txtMsg.Text = "*床位已被占用！";
            //    txtMsg.Tag = "cmbBedNo";
            //    return;
            //}

            //int result = 0;
            //if (this.btnSave.Tag == null)
            //{
            //    if (PersonCodeIsUsed(txtPersonCode.Text))
            //    {
            //        txtMsg.Text = "*人员编号已被占用！";
            //        txtMsg.Tag = "txtPersonCode";
            //        return;
            //    }
            //    result = Insert(txtPersonCode.Text, txtPersonName.Text.Trim(), personTypeCode, personTypeName, 0, "", txtBirthday.Value, deptCode, deptName,
            //                 txtHomeAddress.Text.Trim(), sex, postCode, postName, txtPersonPhone.Text.Trim(), roomCode, roomName, bedCode, bedName, array, txtComment.Text.Trim(),
            //                 txtContactName.Text.Trim(), txtContactPhone.Text.Trim(), txtContactRelationship.Text.Trim());
            //}
            //else
            //{
            //    result = Update(txtPersonCode.Text, txtPersonName.Text.Trim(), personTypeCode, personTypeName, 0, "", txtBirthday.Value, deptCode, deptName,
            //                 txtHomeAddress.Text.Trim(), sex, postCode, postName, txtPersonPhone.Text.Trim(), roomCode, roomName, bedCode, bedName, array, txtComment.Text.Trim(),
            //                 txtContactName.Text.Trim(), txtContactPhone.Text.Trim(), txtContactRelationship.Text.Trim());
            //}

            //if (result == 1)
            //{
            //    Toolkit.MessageBox.Show("保存成功！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
            //    if (btnSave.Tag == null)
            //    {
            //        Common.SysLogEntry.WriteLog("系统基础信息管理", Application.Current.Resources["UserName"].ToString(), OperationType.Add, "添加系统基础信息");
            //    }
            //    else
            //    {
            //        Common.SysLogEntry.WriteLog("系统基础信息管理", Application.Current.Resources["UserName"].ToString(), OperationType.Modify, "修改系统基础信息");
            //    }
            //    Clear();
            //    BindData();
            //}
            //else
            //{
            //    Toolkit.MessageBox.Show("保存失败！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
            //}
        }

        private bool PersonCodeIsUsed(string personCode)
        {
            string strSql = string.Format("select PERSON_CODE from sys_client_person where PERSON_CODE='{0}'", personCode);
            try
            {
                return dbHelper.Exists(strSql);
            }
            catch (Exception)
            {
                return true;
            }
        }

        private bool IsBedUsed(string bedCode, object tag)
        {
            string strSql = string.Empty;
            if (tag == null)
            {
                strSql = string.Format("select FK_BED_CODE from sys_client_person where FK_BED_CODE='{0}'", bedCode);
            }
            else
            {
                strSql = string.Format("select FK_BED_CODE from sys_client_person where FK_BED_CODE='{0}' and PERSON_CODE!='{1}'", bedCode, this.btnSave.Tag.ToString());
            }
            try
            {
                return dbHelper.Exists(strSql);
            }
            catch (Exception)
            {
                return true;
            }
        }


        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Clear();
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            string strType = "";
            if (this.cmbPersonTypeForSelect.SelectedIndex > 0)
            {
                strType = (cmbPersonTypeForSelect.SelectedItem as Label).Content.ToString();
            }
            Select(this.txtPersonCodeForSelect.Text.Trim(), strType);
        }

        private void FrameworkElement_GotFocus(object sender, RoutedEventArgs e)
        {
            ClearTip(sender);
        }

        #region 删除
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (Toolkit.MessageBox.Show("确定要删除该信息吗？", "系统询问", MessageBoxButton.YesNoCancel, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                string id = (sender as Button).Tag.ToString();
                if (Exists(id))
                {
                    Toolkit.MessageBox.Show("删除项存在引用，不能删除！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    if (Delete(id))
                    {
                        Toolkit.MessageBox.Show("删除成功！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        BindData();
                        Common.SysLogEntry.WriteLog("系统基础数据", Application.Current.Resources["UserName"].ToString(), OperationType.Delete, "删除系统基础数据");
                    }
                    else
                    {
                        Toolkit.MessageBox.Show("删除失败！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            else
            {
                e.Handled = false;
            }
        }


        private bool Exists(string id)
        {
            bool result = false;
            string strSql = string.Format("select FK_PERSON_CODE from sys_personalarm_set where FK_PERSON_CODE='{0}'", id);
            try
            {
                result = dbHelper.Exists(strSql);
            }
            catch (Exception)
            {
                result = true;
            }
            return result;
        }

        private bool Delete(string id)
        {
            bool result = false;
            string strSql = string.Format("delete from sys_client_person where PERSON_CODE='{0}'", id);
            try
            {
                int num = dbHelper.ExecuteSql(strSql);
                if (num == 1)
                {
                    result = true;
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }
        #endregion

        private void btnModify_Click(object sender, RoutedEventArgs e)
        {
            this.cmbPersonType.IsEnabled = true; 
            this.txtPersonName.IsEnabled = true;
            this.cmbDept.IsEnabled = true;
            this.cmbPersonProperty.IsEnabled = true;
            this.txtBirthday.IsEnabled = true;
            this.txtHomeAddress.IsEnabled = true;
            this.txtPersonPhone.IsEnabled = true;
            this.txtComment.IsEnabled = true;
            this.imgPhoto.IsEnabled = true;

            this.cmbArea.IsEnabled = true;
            this.cmbBuildingNo.IsEnabled = true;
            this.cmbFloor.IsEnabled = true;
            this.cmbLocation.IsEnabled = true;
            this.cmbBedNo.IsEnabled = true;
            this.txtContactName.IsEnabled = true;
            this.txtContactRelationship.IsEnabled = true;
            this.txtContactPhone.IsEnabled = true;
            Flag = false;
            DataRow[] drs = currentTable.Select("PERSON_CODE='" + (sender as Button).Tag.ToString() + "'");
            if (drs.Length == 1)
            {
                this.txtPersonCode.Text = drs[0]["PERSON_CODE"].ToString();

                for (int i = 0; i < this.cmbPersonType.Items.Count; i++)
                {
                    if ((this.cmbPersonType.Items[i] as Label).Tag.ToString() == drs[0]["FK_TYPE_CODE"].ToString())
                    {
                        this.cmbPersonType.SelectedItem = this.cmbPersonType.Items[i];
                        break;
                    }
                }

                this.txtPersonName.Text = drs[0]["PERSON_NAME"].ToString();
 
                for (int i = 0; i < this.cmbDept.Items.Count; i++)
                {
                    if ((this.cmbDept.Items[i] as Label).Tag.ToString() == drs[0]["FK_CODE_DEPT"].ToString())
                    {
                        this.cmbDept.SelectedItem = this.cmbDept.Items[i];
                        break;
                    }
                }

                for (int i = 0; i < this.cmbPersonProperty.Items.Count; i++)
                {
                    if ((this.cmbPersonProperty.Items[i] as Label).Tag.ToString() == drs[0]["FK_POST_CODE"].ToString())
                    {
                        this.cmbPersonProperty.SelectedItem = this.cmbPersonProperty.Items[i];
                        break;
                    }
                }
                if (drs[0]["PERSON_SEX"].ToString() == "男")
                    rdbMale.IsChecked = true;
                else
                    this.rdbFemale.IsChecked = true;

                this.txtBirthday.Value = Convert.ToDateTime(drs[0]["PERSON_BIRTHDAY"]);
                this.txtHomeAddress.Text = drs[0]["HOME_ADDRESS"].ToString();
                this.txtPersonPhone.Text = drs[0]["PERSON_PHONE"].ToString();
                this.txtComment.Text = drs[0]["INFO_NOTE"].ToString();
                if (drs[0]["PERSON_PHOTO"] == DBNull.Value)
                {
                    this.imgPhoto.Source = null;
                }
                else
                {
                    try
                    {
                        Byte[] imgArray = (Byte[])drs[0]["PERSON_PHOTO"];
                        BitmapImage bi = new BitmapImage();
                        bi.BeginInit();
                        bi.StreamSource = new MemoryStream(imgArray);
                        bi.EndInit();
                        this.imgPhoto.Source = bi;
                    }
                    catch (Exception)
                    {
                        imgPhoto.Source = null;
                    }
                }

                if (drs[0]["FK_ROOM_CODE"].ToString() != "")
                {
                    for (int i = 0; i < this.cmbLocation.Items.Count; i++)
                    {
                        if ((cmbLocation.Items[i] as Label).Tag.ToString() == drs[0]["FK_ROOM_CODE"].ToString())
                        {
                            cmbLocation.SelectedItem = cmbLocation.Items[i];
                            break;
                        }
                    }
                }
                else
                {
                    cmbLocation.SelectedIndex = 0;
                }

                if (drs[0]["FK_BED_CODE"].ToString() != "")
                {

                    ComboboxTool.InitComboboxSource(cmbBedNo, "SELECT INFO_CODE,INFO_NAME,FK_INFO_AREA,FK_INFO_NAME from sys_client_bed WHERE INFO_CODE not IN(select FK_BED_CODE from sys_client_person) and FK_INFO_AREA = '" + drs[0]["FK_ROOM_CODE"].ToString() + "'", "lr");
                    string strValue = drs[0]["FK_BED_CODE"].ToString();//隐藏值
                    string strItem = drs[0]["FK_BED_CODE"].ToString();//显示值
                    Label item1 = new Label();
                    item1.Tag = strValue;
                    item1.Content = strItem;
                    cmbBedNo.Items.Add(item1);
                    for (int i = 0; i < this.cmbBedNo.Items.Count; i++)
                    {
                        if ((cmbBedNo.Items[i] as Label).Tag.ToString() == drs[0]["FK_BED_CODE"].ToString())
                        {
                            cmbBedNo.SelectedItem = cmbBedNo.Items[i];
                            break;
                        }
                    }
                }
                else
                {
                    cmbBedNo.SelectedIndex = 0;
                }

                if (cmbLocation.SelectedIndex != 0)
                {
                    string parentCode = areainfoTable.Select("INFO_AREA='" + (cmbLocation.SelectedItem as Label).Tag.ToString() + "'")[0]["FK_CODE_AREA"].ToString();
                    for (int i = 0; i < this.cmbFloor.Items.Count; i++)
                    {
                        if ((cmbFloor.Items[i] as Label).Tag.ToString() == parentCode)
                        {
                            cmbFloor.SelectedItem = cmbFloor.Items[i];
                            break;
                        }
                    }
                }
                else
                {
                    cmbFloor.SelectedIndex = 0;
                }


                if (cmbFloor.SelectedIndex != 0)
                {
                    string parentCode = areainfoTable.Select("INFO_AREA='" + (cmbFloor.SelectedItem as Label).Tag.ToString() + "'")[0]["FK_CODE_AREA"].ToString();
                    for (int i = 0; i < this.cmbBuildingNo.Items.Count; i++)
                    {
                        if ((cmbBuildingNo.Items[i] as Label).Tag.ToString() == parentCode)
                        {
                            cmbBuildingNo.SelectedItem = cmbBuildingNo.Items[i];
                            break;
                        }
                    }
                }
                else
                {
                    cmbBuildingNo.SelectedIndex = 0;
                }


                if (cmbBuildingNo.SelectedIndex != 0)
                {
                    string parentCode = areainfoTable.Select("INFO_AREA='" + (cmbBuildingNo.SelectedItem as Label).Tag.ToString() + "'")[0]["FK_CODE_AREA"].ToString();
                    for (int i = 0; i < this.cmbArea.Items.Count; i++)
                    {
                        if ((cmbArea.Items[i] as Label).Tag.ToString() == parentCode)
                        {
                            cmbArea.SelectedItem = cmbArea.Items[i];
                            break;
                        }
                    }
                }
                else
                {
                    cmbArea.SelectedIndex = 0;
                }

                this.txtContactName.Text = drs[0]["PERSON_ICE"].ToString();
                this.txtContactRelationship.Text = drs[0]["ICE_RELATIONS"].ToString();
                this.txtContactPhone.Text = drs[0]["ICE_PHONE"].ToString();
                this.container.IsEnabled = false;
                this.btnSave.Tag = (sender as Button).Tag.ToString();
                Flag = true;
            }
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            Clear();
            this.cmbPersonType.IsEnabled = true;
      
            this.txtPersonName.IsEnabled = true;
            this.cmbDept.IsEnabled = true;
            this.cmbPersonProperty.IsEnabled = true;
            this.txtBirthday.IsEnabled = true;
            this.txtHomeAddress.IsEnabled = true;
            this.txtPersonPhone.IsEnabled = true;
            this.txtComment.IsEnabled = true;
            this.imgPhoto.IsEnabled = true;

            this.cmbArea.IsEnabled = true;
            this.cmbBuildingNo.IsEnabled = true;
            this.cmbFloor.IsEnabled = true;
            this.cmbLocation.IsEnabled = true;
            this.cmbBedNo.IsEnabled = true;
            this.txtContactName.IsEnabled = true;
            this.txtContactRelationship.IsEnabled = true;
            this.txtContactPhone.IsEnabled = true;
        }

        private void cmbArea_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (Flag)
            //{
            //    if (cmbArea.SelectedIndex > 0)
            //    {
            //        string code = (cmbArea.SelectedItem as Label).Tag.ToString();
            //        DataRow[] drs = areainfoTable.Select("FK_CODE_AREA='" + code + "'");
            //        ComboboxTool.InitComboboxSource(cmbBuildingNo, drs);
            //    }
            //}
        }

        private void cmbBuildingNo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (Flag)
            //{
            //    if (cmbBuildingNo.SelectedIndex > 0)
            //    {
            //        string code = (cmbBuildingNo.SelectedItem as Label).Tag.ToString();
            //        DataRow[] drs = areainfoTable.Select("FK_CODE_AREA='" + code + "'");
            //        ComboboxTool.InitComboboxSource(cmbFloor, drs);
            //    }
            //}
        }

        private void cmbFloor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (Flag)
            //{
            //    if (cmbFloor.SelectedIndex > 0)
            //    {
            //        string code = (cmbFloor.SelectedItem as Label).Tag.ToString();
            //        DataRow[] drs = areainfoTable.Select("FK_CODE_AREA='" + code + "'");
            //        ComboboxTool.InitComboboxSource(cmbLocation, drs);
            //    }
            //}
        }

        private void cmbLocation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (Flag)
            //{
            //    if (cmbLocation.SelectedIndex > 0)
            //    {
            //        string code = (cmbLocation.SelectedItem as Label).Tag.ToString();

            //        ComboboxTool.InitComboboxSource(cmbBedNo, "SELECT INFO_CODE,INFO_NAME,FK_INFO_AREA,FK_INFO_NAME from sys_client_bed WHERE INFO_CODE not IN(select FK_BED_CODE from sys_client_person) and FK_INFO_AREA = '" + code + "'", "lr");
            //    }
            //}
        }

        private void cmbPersonType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (cmbPersonType.SelectedIndex > 0)
            //{
            //    if ((cmbPersonType.SelectedItem as Label).Content.ToString().Contains("老"))
            //    {
            //        container.IsEnabled = true;
            //    }
            //    else
            //    {
            //        container.IsEnabled = false;
            //    }
            //}
        }

        private void TextboxSearchControl_ImageClick(object sender, EventArgs e)
        {
            Select(txtSearch.Text.Trim(), "");
        }

        private void btnAddPhoto_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Title = "选择照片";
            openFileDialog.Filter = "jpg文件|*.jpg|bmp文件|*.bmp|gif文件|*.gif|png文件|*.png";
            openFileDialog.FileName = string.Empty;
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.DefaultExt = "jpg";

            System.Windows.Forms.DialogResult dr = openFileDialog.ShowDialog();

            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    string filename = openFileDialog.FileName;
                    BitmapImage bi = new BitmapImage(new Uri(filename));
                    BitmapImage biBak = bi.Clone();
                    this.imgPhoto.Source = bi;
                    this.imgPhoto.Tag = biBak;
                }
                catch (Exception)
                {
                    return;
                }
            }
        }

        private void btnDeletePhoto_Click(object sender, RoutedEventArgs e)
        {
            this.imgPhoto.Source = null;
            this.imgPhoto.Tag = null;
        }

        /// <summary>
        /// 生成序列号
        /// </summary>
        /// <returns>序列号</returns>
        private string CreateNewNumber()
        {
            string code = "LR0001";
            try
            {
                string temp = dbHelper.GetSingle("SELECT MAX(PERSON_CODE) from sys_client_person").ToString().ToUpper();
                if (!string.IsNullOrEmpty(temp))
                {
                    int num = (Convert.ToInt32(temp.Replace("LR", "")) + 1);
                    string value = string.Empty;
                    if (num.ToString().Length < 4)
                    {
                        for (int i = 0; i < 4 - num.ToString().Length; i++)
                        {
                            value += "0";
                        }
                    }
                    code = "LR" + value + num.ToString();
                }
            }
            catch (Exception)
            {
                code = "LR0001";
            }
            return code;
        }

        private void Clear()
        {
            this.txtPersonCode.Text = CreateNewNumber();
            this.cmbPersonType.SelectedIndex = 0;
            this.txtPersonName.Text = "";
            this.cmbDept.SelectedIndex = 0;
            this.cmbPersonProperty.SelectedIndex = 0;
            this.rdbMale.IsChecked = true;
            this.txtBirthday.Value = DateTime.Now;
            this.txtHomeAddress.Text = "";
            this.txtPersonPhone.Text = "";
            this.txtComment.Text = "";
            this.imgPhoto.Source = null;
            this.imgPhoto.Tag = null;

            this.cmbPersonType.IsEnabled = false;
          
            this.txtPersonName.IsEnabled = false;
            this.cmbDept.IsEnabled = false;
            this.cmbPersonProperty.IsEnabled = false;
            this.txtBirthday.IsEnabled = false;
            this.txtHomeAddress.IsEnabled = false;
            this.txtPersonPhone.IsEnabled = false;
            this.txtComment.IsEnabled = false;
            this.imgPhoto.IsEnabled = false;

            this.cmbArea.IsEnabled = false;
            this.cmbBuildingNo.IsEnabled = false;
            this.cmbFloor.IsEnabled = false;
            this.cmbLocation.IsEnabled = false;
            this.cmbBedNo.IsEnabled = false;
            this.txtContactName.IsEnabled = false;
            this.txtContactRelationship.IsEnabled = false;
            this.txtContactPhone.IsEnabled = false;


            this.cmbArea.SelectedIndex = 0;
            this.cmbBuildingNo.SelectedIndex = 0;
            this.cmbFloor.SelectedIndex = 0;
            this.cmbLocation.SelectedIndex = 0;
            this.cmbBedNo.SelectedIndex = 0;
            this.txtContactName.Text = "";
            this.txtContactRelationship.Text = "";
            this.txtContactPhone.Text = "";
            this.container.IsEnabled = false;
            this.txtMsg.Text = "";
            this.btnSave.Tag = null;
            Flag = true;
        }


        /// <summary>
        /// 清除提示信息
        /// </summary>
        /// <param name="sender">控件</param>
        private void ClearTip(object sender)
        {
            string name = (sender as FrameworkElement).Name;
            if (txtMsg.Tag != null)
            {
                if (name == txtMsg.Tag.ToString())
                {
                    txtMsg.Text = "";
                }
            }
        }

        private void Select(string Name, string Type)
        {
            StringBuilder sbSql = new StringBuilder();
            if (Name != "")
            {
                sbSql.Append("PERSON_NAME like '%" + Name + "%'");
                if (Type != "")
                {
                    sbSql.Append(" AND FK_TYPE_NAME = '" + Type + "'");
                }
            }
            else
            {
                if (Type != "")
                {
                    sbSql.Append(" FK_TYPE_NAME = '" + Type + "'");
                }
            }

            if (sbSql.ToString() != "")
            {
                DataRow[] drs = currentTable.Select(sbSql.ToString());
                DataTable temp = currentTable.Clone();
                foreach (DataRow row in drs)
                {
                    DataRow dr = temp.NewRow();
                    dr.ItemArray = row.ItemArray;
                    temp.Rows.Add(dr);
                }
                lvlist.DataContext = temp;
            }
            else
            {
                lvlist.DataContext = currentTable;
            }
        }

        //将图片转BYTE[]数组
        private byte[] SetImageToByteArray(BitmapImage bi)
        {
            byte[] bytearray = null;

            try
            {
                Stream smarket = new StreamReader(bi.UriSource.LocalPath).BaseStream;

                if (smarket != null && smarket.Length > 0)
                {
                    smarket.Position = 0;
                    using (BinaryReader br = new BinaryReader(smarket))
                    {
                        bytearray = br.ReadBytes((int)smarket.Length);
                    }
                }
            }
            catch
            {
            }
            return bytearray;
        }

        public int Insert(string PERSON_CODE, string PERSON_NAME, string FK_TYPE_CODE, string FK_TYPE_NAME, int FK_NUMB_DEVICE, string FK_DEVICE_CODE, DateTime? PERSON_BIRTHDAY,
            string FK_CODE_DEPT, string FK_NAME_DEPT, string HOME_ADDRESS, string PERSON_SEX, string FK_POST_CODE, string FK_POST_NAME, string PERSON_PHONE, string FK_ROOM_CODE,
            string FK_ROOM_NAME, string FK_BED_CODE, string FK_BED_NAME, Byte[] PERSON_PHOTO, string INFO_NOTE, string PERSON_ICE, string ICE_PHONE, string ICE_RELATIONS)
        {
            string strSql = @"insert into sys_client_person (PERSON_CODE,PERSON_NAME,FK_TYPE_CODE,FK_TYPE_NAME,FK_NUMB_DEVICE,FK_DEVICE_CODE,PERSON_BIRTHDAY,FK_CODE_DEPT,
                         FK_NAME_DEPT,HOME_ADDRESS,PERSON_SEX,FK_POST_CODE,FK_POST_NAME,PERSON_PHONE,FK_ROOM_CODE,FK_ROOM_NAME,FK_BED_CODE,FK_BED_NAME,PERSON_PHOTO,INFO_NOTE,PERSON_ICE,
                         ICE_PHONE,ICE_RELATIONS) values (?PERSON_CODE,?PERSON_NAME,?FK_TYPE_CODE,?FK_TYPE_NAME,?FK_NUMB_DEVICE,?FK_DEVICE_CODE,?PERSON_BIRTHDAY,?FK_CODE_DEPT,
                         ?FK_NAME_DEPT,?HOME_ADDRESS,?PERSON_SEX,?FK_POST_CODE,?FK_POST_NAME,?PERSON_PHONE,?FK_ROOM_CODE,?FK_ROOM_NAME,?FK_BED_CODE,?FK_BED_NAME,?PERSON_PHOTO,
                         ?INFO_NOTE,?PERSON_ICE,?ICE_PHONE,?ICE_RELATIONS)";

            MySqlParameter param1 = new MySqlParameter("?PERSON_CODE", MySqlDbType.VarChar);
            param1.Value = PERSON_CODE;

            MySqlParameter param2 = new MySqlParameter("?PERSON_NAME", MySqlDbType.VarChar);
            param2.Value = PERSON_NAME;

            MySqlParameter param3 = new MySqlParameter("?FK_TYPE_CODE", MySqlDbType.VarChar);
            param3.Value = FK_TYPE_CODE;

            MySqlParameter param4 = new MySqlParameter("?FK_TYPE_NAME", MySqlDbType.VarChar);
            param4.Value = FK_TYPE_NAME;

            MySqlParameter param5 = new MySqlParameter("?FK_NUMB_DEVICE", MySqlDbType.Int32);
            param5.Value = FK_NUMB_DEVICE;

            MySqlParameter param6 = new MySqlParameter("?FK_DEVICE_CODE", MySqlDbType.VarChar);
            param6.Value = FK_DEVICE_CODE;

            MySqlParameter param7 = new MySqlParameter("?PERSON_BIRTHDAY", MySqlDbType.Date);
            param7.Value = PERSON_BIRTHDAY;

            MySqlParameter param8 = new MySqlParameter("?FK_CODE_DEPT", MySqlDbType.VarChar);
            param8.Value = FK_CODE_DEPT;

            MySqlParameter param9 = new MySqlParameter("?FK_NAME_DEPT", MySqlDbType.VarChar);
            param9.Value = FK_NAME_DEPT;

            MySqlParameter param10 = new MySqlParameter("?HOME_ADDRESS", MySqlDbType.VarChar);
            param10.Value = HOME_ADDRESS;

            MySqlParameter param11 = new MySqlParameter("?PERSON_SEX", MySqlDbType.VarChar);
            param11.Value = PERSON_SEX;

            MySqlParameter param12 = new MySqlParameter("?FK_POST_CODE", MySqlDbType.VarChar);
            param12.Value = FK_POST_CODE;

            MySqlParameter param13 = new MySqlParameter("?FK_POST_NAME", MySqlDbType.VarChar);
            param13.Value = FK_POST_NAME;

            MySqlParameter param14 = new MySqlParameter("?PERSON_PHONE", MySqlDbType.VarChar);
            param14.Value = PERSON_PHONE;

            MySqlParameter param15 = new MySqlParameter("?FK_ROOM_CODE", MySqlDbType.VarChar);
            param15.Value = FK_ROOM_CODE;

            MySqlParameter param16 = new MySqlParameter("?FK_ROOM_NAME", MySqlDbType.VarChar);
            param16.Value = FK_ROOM_NAME;

            MySqlParameter param17 = new MySqlParameter("?FK_BED_CODE", MySqlDbType.VarChar);
            param17.Value = FK_BED_CODE;

            MySqlParameter param18 = new MySqlParameter("?FK_BED_NAME", MySqlDbType.VarChar);
            param18.Value = FK_BED_NAME;

            MySqlParameter param19 = new MySqlParameter("?PERSON_PHOTO", MySqlDbType.VarBinary);
            if (PERSON_PHOTO == null)
            {
                param19.Value = DBNull.Value;
            }
            else
            {
                param19.Value = PERSON_PHOTO;
            }

            MySqlParameter param20 = new MySqlParameter("?INFO_NOTE", MySqlDbType.VarChar);
            param20.Value = INFO_NOTE;

            MySqlParameter param21 = new MySqlParameter("?PERSON_ICE", MySqlDbType.VarChar);
            param21.Value = PERSON_ICE;

            MySqlParameter param22 = new MySqlParameter("?ICE_PHONE", MySqlDbType.VarChar);
            param22.Value = ICE_PHONE;

            MySqlParameter param23 = new MySqlParameter("?ICE_RELATIONS", MySqlDbType.VarChar);
            param23.Value = ICE_RELATIONS;

            MySqlParameter[] parammeters ={param1,param2,param3,param4,param5,param6,param7,param8, param9,param10,param11,
                                          param12,param13,param14,param15,param16,param17,param18,param19,param20,param21,param22,param23};

            try
            {
                int num = dbHelper.ExecuteSql(strSql, parammeters);
                return num;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public int Update(string PERSON_CODE, string PERSON_NAME, string FK_TYPE_CODE, string FK_TYPE_NAME, int FK_NUMB_DEVICE, string FK_DEVICE_CODE, DateTime? PERSON_BIRTHDAY,
            string FK_CODE_DEPT, string FK_NAME_DEPT, string HOME_ADDRESS, string PERSON_SEX, string FK_POST_CODE, string FK_POST_NAME, string PERSON_PHONE, string FK_ROOM_CODE,
            string FK_ROOM_NAME, string FK_BED_CODE, string FK_BED_NAME, Byte[] PERSON_PHOTO, string INFO_NOTE, string PERSON_ICE, string ICE_PHONE, string ICE_RELATIONS)
        {

            string strSql = @"update sys_client_person set PERSON_NAME=?PERSON_NAME,FK_TYPE_CODE=?FK_TYPE_CODE,FK_TYPE_NAME=?FK_TYPE_NAME,FK_NUMB_DEVICE=?FK_NUMB_DEVICE,FK_DEVICE_CODE=?FK_DEVICE_CODE,
            PERSON_BIRTHDAY=?PERSON_BIRTHDAY,FK_CODE_DEPT=?FK_CODE_DEPT,FK_NAME_DEPT=?FK_NAME_DEPT,HOME_ADDRESS=?HOME_ADDRESS,PERSON_SEX=?PERSON_SEX,FK_POST_CODE=?FK_POST_CODE,FK_POST_NAME=?FK_POST_NAME,
            PERSON_PHONE=?PERSON_PHONE,FK_ROOM_CODE=?FK_ROOM_CODE,FK_ROOM_NAME=?FK_ROOM_NAME,FK_BED_CODE=?FK_BED_CODE,FK_BED_NAME=?FK_BED_NAME,PERSON_PHOTO=?PERSON_PHOTO,INFO_NOTE=?INFO_NOTE, 
            PERSON_ICE=?PERSON_ICE,PERSON_ICE=?PERSON_ICE,ICE_RELATIONS=?ICE_RELATIONS where PERSON_CODE=?PERSON_CODE";
            MySqlParameter param1 = new MySqlParameter("?PERSON_CODE", MySqlDbType.VarChar);
            param1.Value = PERSON_CODE;

            MySqlParameter param2 = new MySqlParameter("?PERSON_NAME", MySqlDbType.VarChar);
            param2.Value = PERSON_NAME;

            MySqlParameter param3 = new MySqlParameter("?FK_TYPE_CODE", MySqlDbType.VarChar);
            param3.Value = FK_TYPE_CODE;

            MySqlParameter param4 = new MySqlParameter("?FK_TYPE_NAME", MySqlDbType.VarChar);
            param4.Value = FK_TYPE_NAME;

            MySqlParameter param5 = new MySqlParameter("?FK_NUMB_DEVICE", MySqlDbType.Int32);
            param5.Value = FK_NUMB_DEVICE;

            MySqlParameter param6 = new MySqlParameter("?FK_DEVICE_CODE", MySqlDbType.VarChar);
            param6.Value = FK_DEVICE_CODE;

            MySqlParameter param7 = new MySqlParameter("?PERSON_BIRTHDAY", MySqlDbType.Date);
            param7.Value = PERSON_BIRTHDAY;

            MySqlParameter param8 = new MySqlParameter("?FK_CODE_DEPT", MySqlDbType.VarChar);
            param8.Value = FK_CODE_DEPT;

            MySqlParameter param9 = new MySqlParameter("?FK_NAME_DEPT", MySqlDbType.VarChar);
            param9.Value = FK_NAME_DEPT;

            MySqlParameter param10 = new MySqlParameter("?HOME_ADDRESS", MySqlDbType.VarChar);
            param10.Value = HOME_ADDRESS;

            MySqlParameter param11 = new MySqlParameter("?PERSON_SEX", MySqlDbType.VarChar);
            param11.Value = PERSON_SEX;

            MySqlParameter param12 = new MySqlParameter("?FK_POST_CODE", MySqlDbType.VarChar);
            param12.Value = FK_POST_CODE;

            MySqlParameter param13 = new MySqlParameter("?FK_POST_NAME", MySqlDbType.VarChar);
            param13.Value = FK_POST_NAME;

            MySqlParameter param14 = new MySqlParameter("?PERSON_PHONE", MySqlDbType.VarChar);
            param14.Value = PERSON_PHONE;

            MySqlParameter param15 = new MySqlParameter("?FK_ROOM_CODE", MySqlDbType.VarChar);
            param15.Value = FK_ROOM_CODE;

            MySqlParameter param16 = new MySqlParameter("?FK_ROOM_NAME", MySqlDbType.VarChar);
            param16.Value = FK_ROOM_NAME;

            MySqlParameter param17 = new MySqlParameter("?FK_BED_CODE", MySqlDbType.VarChar);
            param17.Value = FK_BED_CODE;

            MySqlParameter param18 = new MySqlParameter("?FK_BED_NAME", MySqlDbType.VarChar);
            param18.Value = FK_BED_NAME;

            MySqlParameter param19 = new MySqlParameter("?PERSON_PHOTO", MySqlDbType.VarBinary);
            if (PERSON_PHOTO == null)
            {
                param19.Value = DBNull.Value;
            }
            else
            {
                param19.Value = PERSON_PHOTO;
            }

            MySqlParameter param20 = new MySqlParameter("?INFO_NOTE", MySqlDbType.VarChar);
            param20.Value = INFO_NOTE;

            MySqlParameter param21 = new MySqlParameter("?PERSON_ICE", MySqlDbType.VarChar);
            param21.Value = PERSON_ICE;

            MySqlParameter param22 = new MySqlParameter("?ICE_PHONE", MySqlDbType.VarChar);
            param22.Value = ICE_PHONE;

            MySqlParameter param23 = new MySqlParameter("?ICE_RELATIONS", MySqlDbType.VarChar);
            param23.Value = ICE_RELATIONS;

            MySqlParameter[] parammeters ={param1,param2,param3,param4,param5,param6,param7,param8, param9,param10,param11,
                                          param12,param13,param14,param15,param16,param17,param18,param19,param20,param21,param22,param23};

            try
            {
                int num = dbHelper.ExecuteSql(strSql, parammeters);
                return num;
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
