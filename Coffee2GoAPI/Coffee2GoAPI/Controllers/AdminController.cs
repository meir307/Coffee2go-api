using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Coffee2GoAPI.Controllers
{
    public class AdminController : Base
    {
        public AdminController(){}

        [Route("api/admin/GetRequestCount")]
        [HttpGet]
        public HttpResponseMessage GetRequestCount()
        {
            #region example     

            /*
             http://localhost:61596/api/admin/GetRequestCount
            
             */
            #endregion
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, RequestCount.ToString());
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
    }
}
