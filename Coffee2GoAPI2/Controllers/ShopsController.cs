using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BL;
using BL.RequestObjects;
using BL.ResponseObjects;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using static Common.Tools.CommonFunctions;

namespace Coffee2GoAPI2.Controllers
{

    public class ShopsController : BaseController
    {
        private IMemoryCache _cache;
        private IWebHostEnvironment _env;

        public ShopsController(IMemoryCache memoryCache, IWebHostEnvironment env) : base(memoryCache, env)
        {
            _cache = memoryCache;
            _env = env;
        }

        [Route("api/shops/updatedata")]
        [HttpPost]
        public  async Task<IActionResult> updatedata()
        {
            #region example     

            /*
             * http://localhost:61596/api/shops/updatedata
                  ={
                        Email: "b@b.com",
                        BuisnessName: "aa",
                        Lat: "32.094664",
                        Lng: "34.935336",
                        MenuObj:  "[{\"desc\": \"מאוד טעים\", \"name\": \"בורקס\", \"price\": \"10\"}, {\"desc\": \"לא טעים\", \"name\": \"סנדווחץ\", \"price\": \"20\"}]",
                        PhoneNo: "123123"
                    }
             */
            #endregion
            try
            {
                Shop shop = new Shop(GD, SessionId);
                //Shop newShop = Newtonsoft.Json.JsonConvert.DeserializeObject<Shop>(HttpContext.Current.Request.Params.Get("shop"));
                Shop newShop = Newtonsoft.Json.JsonConvert.DeserializeObject<Shop>(HttpContext.Request.Query["shop"]);
                newShop.Id = shop.Id;
                newShop.ValidateData(GD, CheckType.Update);

                if (UploadFileExists())
                    newShop.Logo = await SaveUploadedFile(GD.GetParameterValueByKey("ShopsLogoUploadPath"), shop.PhoneNo);

                shop.UpdateData(newShop);

                return Ok(newShop);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("api/shops/Register")]
        [HttpPost]
        public async Task<IActionResult> Register()
        {
            #region example     

            /*
             * http://localhost:61596/api/shops/register
                  ={
                        Email: "b@b.com",
                        Password: "qwerty",
                        BuisnessName: "aa",
                        Lat: "32.094664",
                        Lng: "34.935336",
                        MenuObj:  "[{\"desc\": \"מאוד טעים\", \"name\": \"בורקס\", \"price\": \"10\"}, {\"desc\": \"לא טעים\", \"name\": \"סנדווחץ\", \"price\": \"20\"}]",
                        PhoneNo: "123123"
                    }
             */
            #endregion
            try
            {

                //Shop shop = Newtonsoft.Json.JsonConvert.DeserializeObject<Shop>(HttpContext.Current.Request.Params.Get("shop"));
                Shop shop = Newtonsoft.Json.JsonConvert.DeserializeObject<Shop>(HttpContext.Request.Query["shop"]);

                shop.ValidateData(GD, CheckType.Register);
                if (!UploadFileExists())
                    return BadRequest("Image file not found");
                else
                     shop.Logo = await SaveUploadedFile(GD.GetParameterValueByKey("ShopsLogoUploadPath"), shop.PhoneNo);

                shop.Register(GD);

                shop.SendRegistrationEmail(HttpContext.Request.Path.ToString(), GD);
                return Created("Created:",shop.BuisnessName);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("api/shops/login")]
        [HttpPost]
        public  IActionResult login(loginParams lp, string userName)
        {
            #region example     

            /*
             http://localhost:61596/api/shops/login?username=meir
             */
            #endregion
            try
            {
                //loginParams lp = Newtonsoft.Json.JsonConvert.DeserializeObject<loginParams>(userData);
                Shop shop = new Shop(GD, userName, lp.password);

                return Ok(shop);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Route("api/shops/approveorder")]
        [HttpGet]
        public IActionResult ApproveOrder(long orderId)
        {
            #region example     

            /*
             http://localhost:61596/api/shops/approveorder?orderid=10

            
             */
            #endregion
            try
            {
                Shop shop = new Shop(GD, SessionId);
                Order order = new Order(GD, orderId);
                order.ShopApprove(shop.Id);

                return Ok("completed successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("api/shops/activateaccount")]
        [HttpGet]
        public IActionResult ActivateAccount(string email)
        {
            #region example     

            /*
             http://localhost:61596/api/shops/activateaccount?email=10

            
             */
            #endregion
            try
            {
                Shop shop = new Shop();
                shop.ActivateAccount(GD, email);

                string body = "";
                var webRoot = _env.WebRootPath;
                string fullPath = System.IO.Path.Combine(webRoot,"/App_Data/ActivationCompleted.html");

                //var fullPath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/ActivationCompleted.html");

                FileStream fileStream = new FileStream(fullPath, FileMode.Open);
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    body = reader.ReadToEnd();
                }

                return Ok(body);
                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [Route("api/shops/orderready")]
        [HttpGet]
        public IActionResult OrderReady(long orderId)
        {
            #region example     

            /*
             http://localhost:61596/api/shops/orderready?orderid=10

            
             */
            #endregion
            try
            {
                Shop shop = new Shop(GD, SessionId);
                Order order = new Order(GD, orderId);
                order.Ready(shop.Id);

                return Ok("completed successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Route("api/shops/opennow")]
        [HttpGet]
        public IActionResult OpenNow(int open)
        {
            #region example     

            /*
             http://localhost:61596/api/shops/opennow?open=0

            
             */
            #endregion
            try
            {
                BL.Shop shop = new BL.Shop(GD, SessionId);
                shop.setOpen(open);
                return Ok("completed successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("api/shops/orderdelivered")]
        [HttpGet]
        public IActionResult orderdelivered(long orderId)
        {
            #region example     

            /*
             http://localhost:61596/api/shops/orderdelivered?orderid=10

            
             */
            #endregion
            try
            {
                Shop shop = new Shop(GD, SessionId);
                Order order = new Order(GD, orderId);
                order.Delivered(shop.Id);

                return Ok("completed successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //[Route("api/shops/getorders")]
        //[HttpGet]
        //public HttpResponseMessage GetOrder()
        //{
        //    #region example     

        //    /*
        //     http://localhost:61596/api/shops/getorders


        //     */
        //    #endregion
        //    try
        //    {
        //        Shop shop = new Shop(GD, SessionId);
        //        Order order = new Order(GD);
        //        List<OrdersResponse> orders = order.GetOrders(shop);

        //        return Request.CreateResponse(HttpStatusCode.OK, orders);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
        //    }
        //}


        [Route("api/shops/rejectorder")]
        [HttpPost]
        public IActionResult RejectOrder(RejectOrder ro)
        {
            #region example     

            /*
             * http://localhost:61596/api/shops/rejectorder
                  ={
                        OrderId: "1",
                        RejectMsg: "dhfghfhfghf ghfgh fgh gh "
                    }
             */
            #endregion
            try
            {
                Shop shop = new Shop(GD, SessionId);
                Order order = new Order(GD, ro.OrderId);
                order.RejectOrder(ro, shop.Id);
                return Ok("Order wes rejected successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("api/shops/getorders")]
        [HttpPost]
        public IActionResult GetOrder(OrdersQueryParams oqp)
        {
            #region example     

            /*
             http://localhost:61596/api/shops/getorders

            
             */
            #endregion
            try
            {
                Shop shop = new Shop(GD, SessionId);
                Order order = new Order(GD);
                //List<OrdersResponse> orders = order.GetOrders(shop);
                List<OrdersResponse> orders = order.GetOrders(oqp, shop);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("api/shops/changepassword")]
        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordParams cpp)
        {
            #region example     

            /*
             http://localhost:61596/api/shops/changepassword

            
             */
            #endregion
            try
            {
                Shop shop = new Shop(GD, SessionId);
                shop.ChangePassword(cpp.OldPassword, cpp.NewPassword);

                return Ok(shop);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}