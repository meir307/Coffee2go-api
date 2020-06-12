using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Coffee2GoAPI2.Controllers
{

    public class AdminController : BaseController
    {
        private IMemoryCache _cache;
        private IWebHostEnvironment _env;
        public AdminController(IMemoryCache memoryCache, IWebHostEnvironment env) : base(memoryCache, env)
        {
            _cache = memoryCache;
            _env = env;
        }

        [Route("api/admin/GetRequestCount")]
        [HttpGet]
        public IActionResult GetRequestCount()
        {
            #region example     

            /*
             http://localhost:61596/api/admin/GetRequestCount
            
             */
            #endregion
            try
            {
                return Ok(RequestCount.ToString());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}