using Dal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using System.Text;

namespace BL.ResponseObjects
{
    public class LoginResponse
    {
        
        GlobalData GD;

        public string SessionId;
        public List<Shop> Shops = null;
        public List<OrdersResponse> OpenOrders = null;
               
        public LoginResponse(GlobalData gd, User user)
        {
            GD = gd;
            SessionId = user.SessionId;
        }

        public void getOrders(User user)
        {
            Order order = new Order(GD);
            OrdersQueryParams oqp = new OrdersQueryParams();
            oqp.FromDate = DateTime.Now.AddDays(-1);
            oqp.ToDate = DateTime.Now;
            OpenOrders = order.GetOrders(oqp, user);
        }

        public void getShops(float lat, float lng, [Optional] string distance)
        {
            Shops = Shop.GetShops(GD, lat, lng, distance);
        }
    }
}
