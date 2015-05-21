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
using System.Windows.Shapes;
using System.Windows.Navigation;
using System.IO;
using FoodSafetyMonitoring.dao;
using System.Security.Cryptography;
using System.Configuration;
using Toolkit = Microsoft.Windows.Controls;
using System.Diagnostics;

namespace FoodSafetyMonitoring
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window
    {
        //定义用户名密码保存的文件
        string rememberPasswordPath = AppDomain.CurrentDomain.BaseDirectory + "userinfo.txt";
        
        private List<UserLoginInfo> list = new List<UserLoginInfo>();

        public LoginWindow()
        {
            InitializeComponent();
            //获取文件中的用户信息
            list = Common.SerializerTool.DeserializeFromFile<List<UserLoginInfo>>(rememberPasswordPath);
            InitCombobox();
        }

        private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            this.Left += e.HorizontalChange;
            this.Top += e.VerticalChange;
        }


        private void InitCombobox()
        {
            if (list != null)
            {
                var query = list.OrderByDescending(user => user.LastLoginTime);
                foreach (UserLoginInfo user in query)
                {
                    Label lbl = new Label();
                    lbl.Width = 155;
                    lbl.Content = user.UserName;
                    lbl.Tag = user.Password;
                    lbl.HorizontalContentAlignment = HorizontalAlignment.Left;
                    lbl.VerticalContentAlignment = VerticalAlignment.Center;
                    lbl.VerticalAlignment = VerticalAlignment.Center;
                    lbl.Style = (Style)this.TryFindResource("ImageLabel");
                    lbl.MouseDown += new MouseButtonEventHandler(lbl_MouseDown);
                    this.cmbName.Items.Add(lbl);
                }
            }

        }

        Label selectLabel = null;
        void lbl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var obj = e.OriginalSource;
                if (obj is Image)
                {
                    selectLabel = this.cmbName.SelectedItem as Label;
                    this.cmbName.Items.Remove(sender as Label);
                    list.Remove(list.Find(user => user.UserName == (sender as Label).Content.ToString()));
                    //将用户列表写入文件
                    Common.SerializerTool.SerializeToFile(list, rememberPasswordPath);
                    e.Handled = true;
                }
            }
        }

        private void cmbName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (selectLabel != null)
            {
                for (int i = 0; i < this.cmbName.Items.Count; i++)
                {
                    if ((this.cmbName.Items[i] as Label).Content == selectLabel.Content)
                    {
                        this.cmbName.SelectedItem = this.cmbName.Items[i];
                        break;
                    }
                }
            }
            selectLabel = null;
            if (this.cmbName.SelectedItem != null)
            {
                this._password.Password = (this.cmbName.SelectedItem as Label).Tag.ToString();
                _rememberPassword.IsChecked = true;
            }
        }

        private void exit_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.Close();
            }
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Login();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Login();
            }
        }

        private void Login()
        {
            if (this.cmbName.Text.Trim() == "")
            {
                txtMsg.Text = "*用户名不能为空！";
                return;
            }

            if (_password.Password.Trim() == "")
            {
                txtMsg.Text = "*密码不能为空！";
                return;
            }

            try
            {
                IDBOperation dbOperation = new DBOperation_MySQL();
                try
                {
                    MD5 md5 = new MD5CryptoServiceProvider();
                    string password = BitConverter.ToString(md5.ComputeHash(Encoding.Default.GetBytes(this._password.Password))).Replace("-", "");
                    UserInfo userInfo = dbOperation.GetMenu(this.cmbName.Text, password);
                    //if (userInfo.Menus.Count > 0)
                    if (userInfo.ID != "" && userInfo.ID != null)
                    {
                        UserLoginInfo userLoginInfo = new UserLoginInfo() { UserName = this.cmbName.Text, Password = _password.Password, LastLoginTime = DateTime.Now };

                        if (list == null)
                        {
                            list = new List<UserLoginInfo>();
                        }
                        var query = from item in list where item.UserName == userLoginInfo.UserName select item;

                        if (query.Count() > 0)
                        {
                            foreach (UserLoginInfo item in list)
                            {
                                if (item.UserName == userLoginInfo.UserName)
                                {
                                    item.Password = userLoginInfo.Password;
                                    item.LastLoginTime = userLoginInfo.LastLoginTime;
                                }
                            }
                        }
                        else
                        {
                            if (_rememberPassword.IsChecked == true)
                            {
                                list.Add(userLoginInfo);
                            }
                        }

                        try
                        {
                            //将用户列表写入文件
                            Common.SerializerTool.SerializeToFile(list, rememberPasswordPath);
                        }
                        catch (Exception e)
                        {
                            txtMsg.Text = "请以管理员身份运行软件！";
                            return;
                        }

                        Application.Current.Resources.Add("User", userInfo);

                        string current_version = ConfigurationManager.AppSettings["version"];

                        string new_version = dbOperation.GetDbHelper().GetSingle("select version from t_version where id = (select max(id) from t_version)").ToString();

                        if (Convert.ToDouble(new_version == "" ? "0" : new_version) > Convert.ToDouble(current_version))
                        {
                            if (Toolkit.MessageBox.Show("当前有新版本，是否升级？", "系统询问", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                            {
                                //this.Close();
                                //Process openupdatedexe = new Process();
                                //openupdatedexe.StartInfo.FileName = "AutoUpdate.exe";
                                //openupdatedexe.Start();
                                ProcessStartInfo info = new ProcessStartInfo("AutoUpdate.exe", new_version);
                                Process.Start(info);
                                

                            }
                            else
                            {
                                MainWindow mainWindow = new MainWindow(dbOperation);
                                mainWindow.Show();
                                Common.SysLogEntry.WriteLog("", this.cmbName.Text, Common.OperationType.Login, "登录系统");
                                this.Close();
                            }
                        }
                        else
                        {
                            MainWindow mainWindow = new MainWindow(dbOperation);
                            mainWindow.Show();
                            Common.SysLogEntry.WriteLog("", this.cmbName.Text, Common.OperationType.Login, "登录系统");
                            this.Close();
                        }
                        
                    }
                    else
                    {
                        txtMsg.Text = "用户名或密码错误，请确认！";
                        return;
                    }
                }
                catch (Exception e)
                {
                    txtMsg.Text = "数据库处理失败！";
                    writeLog(e.Message);
                    return;
                }
            }
            catch (Exception ex)
            {
                txtMsg.Text = "数据库连接失败！";
                writeLog(ex.Message);
                return;
            }
        }

        private void _name_GotFocus(object sender, RoutedEventArgs e)
        {
            txtMsg.Text = "";
        }


        private void _password_GotFocus(object sender, RoutedEventArgs e)
        {
            txtMsg.Text = "";
        }




        private void cmbName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (cmbName.SelectedItem != null)
            {
                return;
            }
            else
            {
                this._password.Password = "";
                this._rememberPassword.IsChecked = false;
            }
        }

        private void min_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.WindowState = System.Windows.WindowState.Minimized;
            }
        }

        private void _style_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void writeLog(string str)
        {

            string strLog = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  " + str + "/r/n";

            StreamWriter errorlog = new StreamWriter(System.IO.Path.Combine(Environment.CurrentDirectory, @"error.txt"), true);
            errorlog.Write(strLog);
            errorlog.Flush();
            errorlog.Close();
        }
    }

    [Serializable]
    public class UserLoginInfo
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public DateTime LastLoginTime { get; set; }


    }
}
