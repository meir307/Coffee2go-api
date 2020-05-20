using BL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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

        public bool UploadFileExists()
        {
            HttpPostedFile file = null;
            if (HttpContext.Current.Request.Files.Count == 0)
                return false;

            file = HttpContext.Current.Request.Files[0];

            // Check if we have a file
            if (file == null)
                return false;


            // Make sure the file has content
            if (!(file.ContentLength > 0))
                return false;

            return true;
        }

        public string SaveUploadedFile(string uploadPath, string fileName)
        {
            HttpPostedFile file = HttpContext.Current.Request.Files[0]; 
            
            if (!Directory.Exists(HttpContext.Current.Server.MapPath(uploadPath)))
            {
                // If it doesn't exist, create the directory
                Directory.CreateDirectory(HttpContext.Current.Server.MapPath(uploadPath));
            }

            //Upload File
            string fName = fileName + getExtention(file.FileName);
            file.SaveAs(HttpContext.Current.Server.MapPath($"{uploadPath}/{fName}"));
            return fName;

        }
        private string getExtention(string fileName)
        {
            int i = fileName.IndexOf(".");
            return fileName.Substring(i);
        }
    }
    
    public class loginParams
    {
        public string password;
    }
}