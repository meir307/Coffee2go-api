using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BL
{
    public static class UserSqlProc
    {
        public static string UserRegistration(User user)
        {
            string RegistrationDate = user.RegistrationDate.HasValue ? user.RegistrationDate.Value.ToString("yyyy-MM-dd HH:mm") : string.Empty;

            //Guid id = Guid.NewGuid();
            StringBuilder sSql = new StringBuilder();
            sSql.Append("insert into users (FullName,Email,Password,MobilePhoneNo,Active, ActivationCode,RegistrationDate) values (");

            //sSql.Append(("UNHEX(REPLACE(\"" + id.ToString() + "\", \"-\",\"\"))"));
            sSql.Append("'" + user.FullName + "',");
            sSql.Append("'" + user.Email + "',");
            sSql.Append("'" + user.Password + "',");
            sSql.Append("'" + user.MobilePhoneNo + "',");
            sSql.Append(" 0,");
            sSql.Append("'" + user.ActivationCode + "',");
            sSql.Append("'" + RegistrationDate + "')");

            return sSql.ToString();
        }

        internal static string UpdateSessionTime(User user)
        {
            StringBuilder sSql = new StringBuilder();
            sSql.Append("update users set SessionTime = '");
            sSql.Append(user.SessionTime.ToString("yyyy-MM-dd hh:mm") + "' ");
            sSql.Append("where SessionId = ");
            sSql.Append("unhex(\"" + user.SessionId + "\")");

            return sSql.ToString();
        }

        internal static string SetSessionAndSessionTime(User user, DataTable dt)
        {
            string LastLoginAt = user.LastLoginAt.HasValue ? user.LastLoginAt.Value.ToString("yyyy-MM-dd HH:mm") : string.Empty;

            StringBuilder sSql = new StringBuilder();

            sSql.Append("update users set SessionTime = '");
            sSql.Append(user.SessionTime.ToString("yyyy-MM-dd hh:mm") + "' ");
            sSql.Append(", SessionId =");
            sSql.Append(("UNHEX(REPLACE(\"" + user.SessionId + "\", \"-\",\"\"))"));
            sSql.Append(", lastLoginAt ='");
            sSql.Append(LastLoginAt + "' ");
            sSql.Append("where Id = ");
            sSql.Append(dt.Rows[0]["Id"].ToString());
            //sSql.Append("unhex(\"" + dt.Rows[0]["Id"].ToString() + "\")");

            return sSql.ToString();
        }

        internal static string GetUser(string userName, string password)
        {
            StringBuilder sSql = new StringBuilder();
            sSql.Append("select Id,Fullname,Email,Password,MobilePhoneNo,LastLoginAt, RegistrationDate ,HEX(sessionid) sessionId");
            sSql.Append(" from users where Email='" + userName + "' and Password='" + password + "'");
            sSql.Append(" and Active=1 ");
            return sSql.ToString();
        }

        internal static string GetUser(string sessionId)
        {
            StringBuilder sSql = new StringBuilder();
            sSql.Append("select Id,Fullname,Email,Password,MobilePhoneNo,LastLoginAt, RegistrationDate ,HEX(SessionId) SessionId  from users where SessionId=");
            sSql.Append("UNHEX(REPLACE(\"" + sessionId + "\", \"-\",\"\"))");

            return sSql.ToString();
        }
    }
}
