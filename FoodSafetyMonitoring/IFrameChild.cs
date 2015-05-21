using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoodSafetyMonitoring
{
    interface IFrameChild
    {
       void FrameClose();
       event FoodSafetyMonitoring.Frame.FrameCloseEventHandler FrameCloseEvent;
    }
}
