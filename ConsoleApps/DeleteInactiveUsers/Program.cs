using Dal;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeleteInactiveUsers
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {                
                BL.GlobalData gd = new BL.GlobalData(ConfigurationManager.AppSettings["ConnectionString"]);

                CRUD dal = new CRUD(gd.ConnectionString);

                StringBuilder sSql = new StringBuilder();
                sSql.Append("SET SQL_SAFE_UPDATES = 0;");

                sSql.Append("delete from users where");
                sSql.Append(" TIMESTAMPDIFF(minute, RegistrationDate, NOW()) >" + gd.GetParameterValueByKey("DeleteInactiveUserAfter_Minutes"));
                sSql.Append(" and active = 0;");

                sSql.Append("SET SQL_SAFE_UPDATES = 1;");

                dal.ExecuteNonQuery(sSql.ToString());

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
