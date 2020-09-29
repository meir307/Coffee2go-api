using Dal;
using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public class Log
    {
        

        public Log(GlobalData gd, string requestURL, int entityId, string requestData,string logType)
        {

            if (gd.GetParameterValueByKey("Write2Log") != "yes")
                return;


            StringBuilder sb = new StringBuilder();
            sb.Append("insert into Log (RequestURL,EntityId,RequestData,CreatedAt,LogType) values (");
            sb.Append("'" + requestURL + "',");
            sb.Append(entityId + ",");
            sb.Append("'" + requestData +"',");
            sb.Append("'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "',");
            sb.Append("'" + logType + "')");

            CRUD dal = new CRUD(gd.ConnectionString);
            dal.ExecuteNonQuery(sb.ToString());
        }

        private void writreLog()
        {
            
        }
    }
}
