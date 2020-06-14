using BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Coffee2GoAPI2.Controllers
{

    public class BaseController : ControllerBase
    {

        public Int32 RequestCount;
        private IMemoryCache _cache;
        private IWebHostEnvironment _env;

        public BL.GlobalData GD
        {
            get
            {
                return (BL.GlobalData)_cache.Get("GlobalData");
            }
        }
        public string SessionId
        {
            get
            {
                if (this.Request.Headers.ContainsKey("SessionId"))
                {
                    
                    return Request.Headers["SessionId"];

                }
             
                throw new Exception("Request header should contain SessionId.");
            }
        }

        public BaseController(IMemoryCache memoryCache, IWebHostEnvironment env)
        {
            _cache = memoryCache;
            _env = env;

            RequestCount = _cache.Get<Int32>("RequestCount");
            RequestCount++;
            _cache.Set("RequestCount", RequestCount, DateTimeOffset.MaxValue);
 
        }

        public bool UploadFileExists()
        {
            IFormFile file = null;

            if (HttpContext.Request.Form.Files.Count == 0)
                return false;

            file = HttpContext.Request.Form.Files[0];

            // Check if we have a file
            if (file == null)
                return false;


            // Make sure the file has content
            if (!(file.Length > 0))
                return false;

            return true;
        }

        public async Task<string> SaveUploadedFile(string uploadPath, string fileName)
        {
            IFormFile file = HttpContext.Request.Form.Files[0];

            var webRoot = _env.WebRootPath;
            var folderpath = System.IO.Path.Combine(webRoot, uploadPath);

            string fullpath = Path.Combine(folderpath, fileName);

            using (var fileStream = new FileStream(fullpath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return fileName;

        }
        private string getExtention(string fileName)
        {
            int i = fileName.IndexOf(".");
            return fileName.Substring(i);
        }
    }


}

    public class loginParams
    {
        public string password;
    }


