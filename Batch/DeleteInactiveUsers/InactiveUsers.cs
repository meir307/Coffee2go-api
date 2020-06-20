using BL;
using Common.Tools;
using Dal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DeleteInactiveUsers
{
    public class InactiveUsers
    {
        //BL.GlobalData gd= new BL.GlobalData()
        private GlobalData gd;
        DataTable dt;

        public InactiveUsers(GlobalData gd)
        {
            this.gd = gd;
            CRUD dal = new CRUD(gd.ConnectionString);
            string sSql = "select * from users where Active = 0";
            dal.ExecuteQuery(sSql, ref dt);
        }

        internal void DeleteInactiveUsers()
        {
            DateTime RegistrationDate;
            foreach (DataRow dr in dt.Rows)
            {
                RegistrationDate = DateTime.Parse(dr["RegistrationDate"].ToString());
                TimeSpan span = DateTime.Now.Subtract(RegistrationDate);

                if (span.Minutes > int.Parse(gd.GetParameterValueByKey("DeleteInactiveUserAfter_Minutes")))
                {
                    daleteUser((int)dr["Id"]);
                }

            }
        }

        private void daleteUser(int id)
        {
            string sSql = "delete from users where Id = " + id.ToString();
            CRUD dal = new CRUD(gd.ConnectionString);
            dal.ExecuteNonQuery(sSql);
        }
    }
}
