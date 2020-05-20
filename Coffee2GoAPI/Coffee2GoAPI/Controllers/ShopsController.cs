using BL;
using BL.RequestObjects;
using BL.ResponseObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using static Common.Tools.CommonFunctions;

namespace Coffee2GoAPI.Controllers
{
    public class ShopsController : Base
    {
        public ShopsController() { }

        [Route("api/shops/updatedata")]
        [HttpPost]
         public HttpResponseMessage updatedata()
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
                Shop newShop = Newtonsoft.Json.JsonConvert.DeserializeObject<Shop>(HttpContext.Current.Request.Params.Get("shop"));
                newShop.Id = shop.Id;
                newShop.ValidateData(GD, CheckType.Update);

                if (UploadFileExists())
                    newShop.Logo = SaveUploadedFile(GD.GetParameterValueByKey("ShopsLogoUploadPath"), shop.PhoneNo);
                
                shop.UpdateData(newShop);

                return Request.CreateResponse(HttpStatusCode.OK, newShop);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [Route("api/shops/Register")]
        [HttpPost]
        public HttpResponseMessage Register()
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
                
                Shop shop = Newtonsoft.Json.JsonConvert.DeserializeObject<Shop>(HttpContext.Current.Request.Params.Get("shop"));
                shop.ValidateData(GD, CheckType.Register);
                if (!UploadFileExists())
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Image file not found");
                else
                    shop.Logo = SaveUploadedFile(GD.GetParameterValueByKey("ShopsLogoUploadPath"), shop.PhoneNo);

                shop.Register(GD);
                                
                shop.SendRegistrationEmail(HttpContext.Current.Request.Url.ToString(), GD);
                return Request.CreateResponse(HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [Route("api/shops/login")]
        [HttpPost]
        public HttpResponseMessage login(loginParams lp, string userName)
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
                
                return Request.CreateResponse(HttpStatusCode.OK, shop);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }


        [Route("api/shops/approveorder")]
        [HttpGet]
        public HttpResponseMessage ApproveOrder(long orderId)
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

                return Request.CreateResponse(HttpStatusCode.OK, "completed successfully");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [Route("api/shops/activateaccount")]
        [HttpGet]
        public HttpResponseMessage ActivateAccount(string email)
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
                var fullPath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/ActivationCompleted.html");

                using (StreamReader reader = File.OpenText(fullPath)) 
                {
                    body = reader.ReadToEnd();
                }
                return Request.CreateResponse(HttpStatusCode.OK, body);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
        


        [Route("api/shops/orderready")]
        [HttpGet]
        public HttpResponseMessage OrderReady(long orderId)
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

                return Request.CreateResponse(HttpStatusCode.OK, "completed successfully");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }


        [Route("api/shops/opennow")]
        [HttpGet]
        public HttpResponseMessage OpenNow(int open)
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
                return Request.CreateResponse(HttpStatusCode.OK, "completed successfully");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [Route("api/shops/orderdelivered")]
        [HttpGet]
        public HttpResponseMessage orderdelivered(long orderId)
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

                return Request.CreateResponse(HttpStatusCode.OK, "completed successfully");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
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
        public HttpResponseMessage RejectOrder(RejectOrder ro)
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
                return Request.CreateResponse(HttpStatusCode.OK, "Order wes rejected successfully.");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [Route("api/shops/getorders")]
        [HttpPost]
        public HttpResponseMessage GetOrder(OrdersQueryParams oqp)
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
                return Request.CreateResponse(HttpStatusCode.OK, orders);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [Route("api/shops/changepassword")]
        [HttpPost]
        public HttpResponseMessage ChangePassword(ChangePasswordParams cpp)
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

                return Request.CreateResponse(HttpStatusCode.OK, shop);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
    }
}
