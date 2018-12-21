using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ElectricPowerDebuger.Common
{
    #region  双缓冲扩展类，解决闪烁问题
    public static class DoubleBufferExpand
    {
        public static void DoubleBuffered(this object obj, bool flag)
        {
            Type tp = obj.GetType();
            PropertyInfo pi = tp.GetProperty("DoubleBuffered",
                BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(obj, flag, null);
        }
    }
    #endregion
}
