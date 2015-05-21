using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoodSafetyMonitoring.Common
{
    /// <summary>
    /// 操作类型
    /// </summary>
    enum OperationType
    {
        Login,//登录
        Add,//添加
        Modify,//修改
        Delete,//删除
        AddAndModify,//添加修改
        AddAndDelete,//添加删除
        ModifyAndDelete,//修改删除
        All//全部
    }
}
