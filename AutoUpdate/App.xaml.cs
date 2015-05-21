using System;  
using System.Collections.Generic;  
using System.Configuration;  
using System.Data;  
using System.Linq;  
using System.Windows;  
 
namespace AutoUpdate  
{  
  
    /// <summary>   
    /// App.xaml 的交互逻辑   
    /// </summary>   
    public partial class App : Application  
    {  
        //重写OnStartup，获得启动程序   
        protected override void OnStartup(StartupEventArgs e)  
        {  
           if (e.Args != null && e.Args.Count() > 0)  
            {
                //this.Properties["startexe"] = e.Args[0];
                this.Properties["version"] = e.Args[0];      
            }

           //this.Properties["startexe"] = "检测监管系统.exe";
           //this.Properties["version"] = "1.1";
           base.OnStartup(e);  
        }    
    }  
}  

