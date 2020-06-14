using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using BL;
using BL.ResponseObjects;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Coffee2GoAPI2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : BaseController
    {
        private IMemoryCache _cache;
        private IWebHostEnvironment _env;

        public UsersController(IMemoryCache memoryCache, IWebHostEnvironment env) : base(memoryCache, env)
        {
            _cache = memoryCache;
            _env = env;
        }

        [Route("api/users/updatedata")]
        [HttpPost]
        //public HttpResponseMessage updatedata(string sessionid, [FromBody] string data)
        public IActionResult updatedata(User newUser)
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

                return Ok("completed successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Route("api/users/Register")]
        [HttpPost]
        public IActionResult Register(User user)
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
                return Created(user.FullName,"completed successfully");


            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Route("api/users/login")]
        [HttpPost]
        public IActionResult login(loginParams lp, string userName, float lat, float lng)
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
                return Ok(loginResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Route("api/users/addorder")]
        [HttpPost]
        public IActionResult AddOrder(Order order)
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
                //Order order = Newtonsoft.Json.JsonConvert.DeserializeObject<Order>(orderData);
                order.AddNew(GD, user);

                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Route("api/users/UserUpdateArrivalTime")]
        [HttpGet]
        public IActionResult UserUpdateArrivalTime(long orderId, string EstimatedArrivalAt)
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

                return Ok("completed successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Route("api/users/GetShops")]
        [HttpGet]
        public IActionResult GetShops(float lat, float lng, [Optional] string range)
        {
            #region example     

            /*
            http://localhost:61596/api/users/GetShops?lat=dsdfsdfsdf&lng=dsdfsdfsdf
            
             */
            #endregion

            try
            {
                User user = new User(GD, SessionId);
                List<Shop> shops = Shop.GetShops(GD, lat, lng, range);
                return Ok(shops);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("api/users/GetOrders")]
        [HttpPost]
        public IActionResult GetOrders(OrdersQueryParams oqp)
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
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}