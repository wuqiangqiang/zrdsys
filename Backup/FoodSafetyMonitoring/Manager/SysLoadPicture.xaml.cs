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
using FoodSafetyMonitoring.dao;
using System.IO;
using System.Drawing;

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// SysLoadPicture.xaml 的交互逻辑
    /// </summary>
    public partial class SysLoadPicture : UserControl
    {
        private IDBOperation dbOperation;

        public SysLoadPicture(IDBOperation dbOperation)
        {
            InitializeComponent();
            this.dbOperation = dbOperation;
        }

        private void btBrowse_Click(object sender, RoutedEventArgs e)
        {
            //创建＂打开文件＂对话框
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            //设置文件类型过滤
            dlg.Filter = "图片|*.jpg;*.png;*.gif;*.bmp;*.jpeg";

            // 调用ShowDialog方法显示＂打开文件＂对话框
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                //获取所选文件名并在FileNameTextBox中显示完整路径
                string filename = dlg.FileName;
                FileNameTextBox.Text = filename;

                //在image1中预览所选图片
                BitmapImage image = new BitmapImage(new Uri(filename));
                image1.Source = image;
                //image1.Width = image.Width;
                //image1.Height = image.Height;
            }
        }

        private void btUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(FileNameTextBox.Text))
            {
                UploadIMG();
                FileNameTextBox.Text = string.Empty;
                image1.Source = null;

            }
            else
            {
                Toolkit.MessageBox.Show("请选择图片！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
        }

        //上传图片至数据库
         private void UploadIMG()
        {
             //将所选文件的读入字节数组img
            byte[] img = File.ReadAllBytes(FileNameTextBox.Text);
            //FileInfo fi = new FileInfo( FileNameTextBox.Text);
            if (img.Length > 65 * 1024)
            {
                Toolkit.MessageBox.Show("上传图片大小不能大于65KB！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            //string fileName = System.IO.Path.GetFileNameWithoutExtension(FileNameTextBox.Text);

            //string sql = String.Format("update sys_client_sysdept set image = '{0}'  where INFO_CODE = '{1}'", img, (Application.Current.Resources["User"] as UserInfo).DepartmentID);
            //int count = dbOperation.GetDbHelper().ExecuteSql(sql);
            int count = dbOperation.GetDbHelper().Load_Picture((Application.Current.Resources["User"] as UserInfo).DepartmentID, img);
            if (count == 1)
            {
                Toolkit.MessageBox.Show("图片上传成功！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                Toolkit.MessageBox.Show("图片上传失败！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
         }

         //private void btShow_Click(object sender, RoutedEventArgs e)
         //{
         //    DataTable dt = dbOperation.GetDbHelper().GetDataSet("select image from sys_client_sysdept where INFO_CODE= '" + (Application.Current.Resources["User"] as UserInfo).DepartmentID + "'").Tables[0];
         //    //this.tb_jobdesc.Text = ds.Tables[0].Rows[0]["job_desc"].ToString();
         //    byte[] img = (byte[])dt.Rows[0][0];    //从数据库中获取图片数据转换为字节数组（注意：不用用这种方式转换为字节数组，这种转换有问题，我之前一直出不来效果 byte[] img = System.Text.ASCIIEncoding.ASCII.GetBytes(ds.Tables[0].Rows[0]["pic"].ToString()); 现在修改了，就能出来效果了，这个问题还挺让人纠结的呢，所以大家要注意哦！）
         //    ShowSelectedIMG(img);                //以流的方式显示图片的方法
         //}

         //private void ShowSelectedIMG(byte[] img)
         //{
         //    MemoryStream ms = new MemoryStream(img);//img是从数据库中读取出来的字节数组
         //    ms.Seek(0, System.IO.SeekOrigin.Begin);

         //    BitmapImage newBitmapImage = new BitmapImage();
         //    newBitmapImage.BeginInit();
         //    newBitmapImage.StreamSource = ms;
         //    newBitmapImage.EndInit();
         //    image2.Source = newBitmapImage;
         //}
    }
}
