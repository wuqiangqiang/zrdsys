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
using FoodSafetyMonitoring.dao;
using DBUtility;
using System.Data;
using Toolkit = Microsoft.Windows.Controls;

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// SetTaskGrade.xaml 的交互逻辑
    /// </summary>
    public partial class SetTaskGrade : Window
    {
        private IDBOperation dbOperation;
        private string gradeId;
        private string deptId;
        private DataTable currentTable;
        private SysTaskCheck systaskcheck;

        public SetTaskGrade(IDBOperation dbOperation, string grade_id, string dept_id, DataTable current_table,SysTaskCheck systaskcheck)
        {
            InitializeComponent();

            this.dbOperation = dbOperation;

            this.systaskcheck = systaskcheck; 

            gradeId = grade_id;

            deptId = dept_id;

            currentTable = current_table;

            string grade_up;
            string grade_down;

            switch (gradeId)
            {
                case "1": _grade_up.Text = "100";
                    _grade_up.IsEnabled = false;
                    break;
                case "2": grade_up = dbOperation.GetDbHelper().GetSingle(string.Format("select parameterDown from t_city_grade where cityId = '{0}' and gradeId = '{1}'", (Application.Current.Resources["User"] as UserInfo).DepartmentID,"1")).ToString();
                    grade_down = dbOperation.GetDbHelper().GetSingle(string.Format("select parameterUp from t_city_grade where cityId = '{0}' and gradeId = '{1}'", (Application.Current.Resources["User"] as UserInfo).DepartmentID, "3")).ToString();
                    _grade_up.Text = grade_up;
                    _grade_down.Text = grade_down;
                    break;
                case "3": grade_up = dbOperation.GetDbHelper().GetSingle(string.Format("select parameterDown from t_city_grade where cityId = '{0}' and gradeId = '{1}'", (Application.Current.Resources["User"] as UserInfo).DepartmentID, "2")).ToString();
                    grade_down = dbOperation.GetDbHelper().GetSingle(string.Format("select parameterUp from t_city_grade where cityId = '{0}' and gradeId = '{1}'", (Application.Current.Resources["User"] as UserInfo).DepartmentID, "4")).ToString();
                    _grade_up.Text = grade_up;
                    _grade_down.Text = grade_down;
                    break;
                case "4": grade_up = dbOperation.GetDbHelper().GetSingle(string.Format("select parameterDown from t_city_grade where cityId = '{0}' and gradeId = '{1}'", (Application.Current.Resources["User"] as UserInfo).DepartmentID, "3")).ToString();
                    grade_down = dbOperation.GetDbHelper().GetSingle(string.Format("select parameterUp from t_city_grade where cityId = '{0}' and gradeId = '{1}'", (Application.Current.Resources["User"] as UserInfo).DepartmentID, "5")).ToString();
                    _grade_up.Text = grade_up;
                    _grade_down.Text = grade_down;
                    break;
                case "5": _grade_down.Text = "0";
                    _grade_down.IsEnabled = false;
                    break;
                default: break;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (_grade_down.Text == "" || _grade_up.Text == "") 
            {
                _txtmsg.Text = "*请输入参数！*";
                return;
            }

            if (int.Parse(_grade_up.Text) <= int.Parse(_grade_down.Text))
            {
                _txtmsg.Text = "*参数下限值必须小于上限值！";
                return;
            }

            if (int.Parse(_grade_up.Text) >100 || int.Parse(_grade_down.Text) > 100)
            {
                _txtmsg.Text = "*参数值必须小于100！";
                return;
            }

            bool exit_flag = dbOperation.GetDbHelper().Exists(string.Format("select count(gradeId) from t_city_grade where cityId = '{0}' and gradeId = '{1}'", deptId, gradeId));

            if (exit_flag)
            {
                int n = dbOperation.GetDbHelper().ExecuteSql(string.Format("update t_city_grade set parameterDown='{0}',parameterUp = '{1}',createUserid='{2}',createDate='{3}' where cityId = '{4}' and gradeId = '{5}' ",
                                                                  _grade_down.Text, _grade_up.Text,
                                                                  (Application.Current.Resources["User"] as UserInfo).ID,
                                                                  DateTime.Now, deptId, gradeId));
                if (n == 1)
                {
                    Toolkit.MessageBox.Show("检测任务指标更新成功！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    systaskcheck.loadTaskGrade();
                    this.Close();
                }
                else
                {
                    Toolkit.MessageBox.Show("检测任务指标更新失败！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
            }
            else
            {
                int n = dbOperation.GetDbHelper().ExecuteSql(string.Format("insert into t_city_grade(cityId,gradeId,parameterDown,parameterUp,createUserid,createDate) values('{0}','{1}','{2}','{3}','{4}', '{5}')",
                                                                 deptId, gradeId, _grade_down.Text, _grade_up.Text,
                                                                 (Application.Current.Resources["User"] as UserInfo).ID,
                                                                 DateTime.Now));
                if (n == 1)
                {
                    Toolkit.MessageBox.Show("检测任务指标插入成功！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    systaskcheck.loadTaskGrade();
                    this.Close();
                }
                else
                {
                    Toolkit.MessageBox.Show("检测任务指标插入失败！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
            }
        }

        private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            this.Left += e.HorizontalChange;
            this.Top += e.VerticalChange;
        }

        private void exit_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.Close();
            }
        }

        private void exit_MouseEnter(object sender, MouseEventArgs e)
        {
            exit.Source = new BitmapImage(new Uri("pack://application:,," + "/res/close_on.png"));
        }

        private void exit_MouseLeave(object sender, MouseEventArgs e)
        {
            exit.Source = new BitmapImage(new Uri("pack://application:,," + "/res/close.png"));
        }

        private void Grade_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!isNumberic(text))
                { e.CancelCommand(); }
            }
            else { e.CancelCommand(); }
        }

        private void Grade_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void Grade_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!isNumberic(e.Text))
            {
                e.Handled = true;
            }
            else
                e.Handled = false;
        }

        //isDigit是否是数字
        public static bool isNumberic(string _string)
        {
            if (string.IsNullOrEmpty(_string))

                return false;
            foreach (char c in _string)
            {
                if (!char.IsDigit(c))
                    //if(c<'0' c="">'9')//最好的方法,在下面测试数据中再加一个0，然后这种方法效率会搞10毫秒左右
                    return false;
            }
            return true;
        }
    }
}
