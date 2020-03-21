using Dal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BL
{
    public class GlobalData
    {
        DataTable SystemParameters;
        public string ConnectionString { get;}
        public GlobalData(string connString)
        {
            ConnectionString = connString;

            SystemParameters = new DataTable();
            CRUD dal = new CRUD(ConnectionString);

            dal.ExecuteQuery("select * from su_params", ref SystemParameters);

        }

        public string GetParameterValueByKey(string key)
        {
            DataRow[] dr = SystemParameters.Select("Key='" + key +"'");
            return dr[0]["Value"].ToString();
        }
    }
}
