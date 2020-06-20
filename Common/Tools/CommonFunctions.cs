using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Common.Tools
{
    public static class CommonFunctions
    {
        public enum CheckType
        {
            Register,
            Update
        }
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

        public static string getRandomString(int len)
        {
            string _numbers = "0123456789";
            Random random = new Random();
            string ret = "";
                       
            for (int i = 0; i< len; i++)
            {
                ret += (_numbers[random.Next(0, _numbers.Length)]);
            }

            return ret;
        }

        

    }
}
