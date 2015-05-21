using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoodSafetyMonitoring
{
    public interface IUserControlClose
    {
        event FoodSafetyMonitoring.MainWindow.UserControlCloseEventHandler UserControlCloseEvent;
        void Close();
    }
}
