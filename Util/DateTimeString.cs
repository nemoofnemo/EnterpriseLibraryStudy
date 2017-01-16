using System;
using System.Collections.Generic;
using System.Text;

namespace Util
{
    /// <summary>
    /// get time stamp as YYYY-MM-DD HH:mm:ss
    /// </summary>
    public static class DateTimeString
    {
        public static string Get()
        {
            return DateTime.Now.GetDateTimeFormats('s')[0].ToString();
        }
    }
}
