using BL;
using BL.ResponseObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Web.Http;

namespace Coffee2GoAPI.Controllers
{
    public class UsersController : Base
    {
        public UsersController() { }

        [Route("api/users/updatedata")]
        [HttpPost]
        //public HttpResponseMessage updatedata(string sessionid, [FromBody] string data)
        public HttpResponseMessage updatedata(User newUser)
        {
            #region example     

            /*
             * http://localhost:61596/api/users/updatedata
                  ={
	                    Email: "meir",
	                    fullName: "mandeles",
	                    mobilePhoneNo: "123123123"
                    }
             */
            #endregion
            try
            {
                User user = new User(GD, SessionId);
                user.UpdateData(newUser);

                return Request.CreateResponse(HttpStatusCode.OK, "completed successfully");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }


        [Route("api/users/Register")]
        [HttpPost]
        public HttpResponseMessage Register(User user)
        {
            #region example     

            /*
             * http://localhost:61596/api/users/register
              ={
	                Email: "meir",
	                password: "qwerty", 
	                fullName: "mandeles",
	                mobilePhoneNo: "123123123"
                }
             */
            #endregion
            try
            {
                user.Register(GD);
                return Request.CreateResponse(HttpStatusCode.Created, "Step 1 completed successfully");

                
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [Route("api/users/activateaccount")]
        [HttpGet]
        public HttpResponseMessage ActivateAccount(string phoneNum, string activationCode)
        {
            #region example     

            /*
            http://localhost:61596/api/users/activateaccount?phoneNum=123123123&activationCode=246876
            
             */
            #endregion

            try
            {
                User user = new User(GD);
                user.ActivateAccount(phoneNum, activationCode);

                return Request.CreateResponse(HttpStatusCode.OK, "completed successfully");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }



        [Route("api/users/login")]
        [HttpPost]
        public HttpResponseMessage login(loginParams lp, string userName, float lat, float lng)
        {
            #region example     

            /*
             http://localhost:61596/api/users/login?username=meir&password=qwerty&lng=34.93533600&lat=32.09466400
             */
            #endregion

            try
            {
                User user = new User(GD, userName, lp.password);

                LoginResponse loginResponse = new LoginResponse(GD, user);
                loginResponse.getShops(lat, lng);
                loginResponse.getOrders(user);
                return Request.CreateResponse(HttpStatusCode.OK, loginResponse);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }


        [Route("api/users/addorder")]
        [HttpPost]
        public HttpResponseMessage AddOrder(Order order)
        {
            #region example     

            /*
             http://localhost:61596/api/users/addorder

            ={
                "ShopId": 1,
                "orderObj": "[{\"desc\": \"מאוד טעים\", \"name\": \"בורקס\", \"price\": \"10\"}, {\"desc\": \"לא טעים\", \"name\": \"סנדווחץ\", \"price\": \"20\"}]",
                "EstimatedArrivalAt": "02/02/2020 16:23"
             }
             */
            #endregion
            try
            {
                User user = new User(GD, SessionId);

               // Log l = new Log(GD, "api/users/addorder", user.Id, JsonConvert.SerializeObject(order), "info");
                //Order order = Newtonsoft.Json.JsonConvert.DeserializeObject<Order>(orderData);
                order.AddNew(GD, user);

                return Request.CreateResponse(HttpStatusCode.OK, order);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }


        [Route("api/users/UserUpdateArrivalTime")]
        [HttpGet]
        public HttpResponseMessage UserUpdateArrivalTime(long orderId, string EstimatedArrivalAt)
        {
            #region example     

            /*
            http://localhost:61596/api/users/UserUpdateArrivalTime?orderid=10&EstimatedArrivalAt=2020-02-02T19:03:00
            
             */
            #endregion

            try
            {
                User user = new User(GD, SessionId);
                Order order = new Order(GD, orderId);
                order.UserUpdateArrivalTime(EstimatedArrivalAt, user.Id);
                List<OrdersResponse> orders = order.GetOrder(user.Id, orderId);
                return Request.CreateResponse(HttpStatusCode.OK, orders);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }


        [Route("api/users/GetShops")]
        [HttpGet]
        public HttpResponseMessage GetShops(float lat, float lng, [Optional] string range)
        {
            #region example     

            /*
            http://localhost:61596/api/users/GetShops?lat=dsdfsdfsdf&lng=dsdfsdfsdf
            
             */
            #endregion

            try
            {
               // User user = new User(GD, SessionId);
                List<Shop> shops = Shop.GetShops(GD, lat, lng, range);
                return Request.CreateResponse(HttpStatusCode.OK, shops);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [Route("api/users/GetOrders")]
        [HttpPost]
        public HttpResponseMessage GetOrders(OrdersQueryParams oqp)
        {
            #region example     

            /*
            http://localhost:61596/api/users/GetOrders
            
             */
            #endregion

            try
            {
                User user = new User(GD, SessionId);
                Order order = new Order(GD);
                List<OrdersResponse> orders = order.GetOrders(oqp, user);
                return Request.CreateResponse(HttpStatusCode.OK, orders);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
    }

}
