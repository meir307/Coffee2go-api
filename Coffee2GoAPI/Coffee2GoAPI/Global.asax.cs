using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Coffee2GoAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            
            setApplicationCache();

        }

        private void setApplicationCache()
        {
            string connStr = ConfigurationManager.AppSettings.Get("ConnectionString");
            BL.GlobalData gd = new BL.GlobalData(connStr);
            MemoryCacher.Add("GlobalData", gd, DateTimeOffset.MaxValue);


            MemoryCacher.Add("RequestCount", 0, DateTimeOffset.MaxValue);
        }
    }
}
