using System;
using System.Collections.Generic;
using System.Text;

namespace Util
{
    public static class DateTimeString
    {
        public static string Get()
        {
            return DateTime.Now.GetDateTimeFormats('s')[0].ToString();
        }
    }
}
