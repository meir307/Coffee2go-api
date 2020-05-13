using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Common.Tools
{
    public static class CommonFunctions
    {
        public static DateTime? ConvertToDateTime(object o)
        {
            if (o.ToString() == string.Empty) 
                return null;
            return DateTime.Parse(o.ToString());
        }
        public static string ConvertToGuid(object o)
        {
            if (o.ToString() == string.Empty) return null;
            return o.ToString();
        }

        public static string IsNullOrEmpty(object o)
        {
            if (o == null || o.ToString() == "")
                return null;

            return o.ToString();
        }

        public static bool sessionExpired(DataTable dt)
        {
            return false;
        }

     
    }
}
