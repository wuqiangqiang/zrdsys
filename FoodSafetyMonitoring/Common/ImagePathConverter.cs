using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace FoodSafetyMonitoring.Common
{
    public class ImagePathConverter : IValueConverter
    {
        #region IValueConverter 成员

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string source = string.Empty;
            switch ((string)value)
            {
                case "枪机":
                    source = "/Manager/Images/枪机.png";
                    break;
                case "球机":
                    source = "/Manager/Images/球机.png";
                    break;
                case "半球机":
                    source = "/Manager/Images/半球机.png";
                    break;
            }
            return source;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }


    public class DeviceImagePathConverter : IValueConverter
    {
        #region IValueConverter 成员

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string source = string.Empty;

            if (value.ToString().Contains("网关"))
            {
                source = "/Manager/Images/网关.png";
            }
            else if (value.ToString().Contains("定位器"))
            {
                source = "/Manager/Images/定位读卡器.png";
            }
            else if (value.ToString().Contains("卡片"))
            {
                source = "/Manager/Images/电子标签.png";
            }
            else if (value.ToString().Contains("腕带"))
            {
                source = "/Manager/Images/腕带.png";
            }
            else if (value.ToString().Contains("感"))
            {
                source = "/Manager/Images/传感器.png";
            }
            return source;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class PersonImagePathConverter : IValueConverter
    {
        #region IValueConverter 成员

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string source = string.Empty;

            if (value.ToString().Contains("男"))
            {
                source = "/Manager/Images/person.png";
            }
            else if (value.ToString().Contains("女"))
            {
                source = "/Manager/Images/小人3.png";
            }
            return source;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class AgeConverter : IValueConverter
    {
        #region IValueConverter 成员

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int age = 0;
            try
            {
                age = DateTime.Now.Year - ((DateTime)value).Year;
            }
            catch (Exception)
            {
                age = 0;
            }
            return age;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

}
