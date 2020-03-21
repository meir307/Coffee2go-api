using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace Coffee2GoAPI.Controllers
{
    public class Base : ApiController
    {
        public Int32 RequestCount;
               
        public BL.GlobalData GD
        {
            get
            {
                return (BL.GlobalData)MemoryCacher.GetValue("GlobalData");
            }
        }
        public string SessionId
        {
            get
            {
                if (this.Request.Headers.Contains("SessionId"))
                    return this.Request.Headers.GetValues("SessionId").First();

                throw new Exception("Request header should contain SessionId.");
            }
        }

        public Base()
        {
            RequestCount = (Int32)MemoryCacher.GetValue("RequestCount");
            RequestCount++;
            MemoryCacher.Set("RequestCount", RequestCount, DateTimeOffset.MaxValue);
        }
    }
    
    public class loginParams
    {
        public string password;
    }
}