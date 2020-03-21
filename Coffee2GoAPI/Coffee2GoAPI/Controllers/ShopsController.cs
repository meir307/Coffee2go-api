using BL;
using BL.ResponseObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Coffee2GoAPI.Controllers
{
    public class ShopsController : Base
    {
        public ShopsController() { }

        [Route("api/shops/updatedata")]
        [HttpPost]
         public HttpResponseMessage updatedata(Shop newShop)
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
                //Shop newShop = Newtonsoft.Json.JsonConvert.DeserializeObject<Shop>(data);
                shop.UpdateData(newShop);

                return Request.CreateResponse(HttpStatusCode.OK, "completed successfully");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [Route("api/shops/Register")]
        [HttpPost]
        public HttpResponseMessage Register(Shop shop)
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
                //BL.Shop shop = Newtonsoft.Json.JsonConvert.DeserializeObject<Shop>(userData);
                shop.Register(GD);
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

    }
}
