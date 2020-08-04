using BL.ResponseObjects;
using Common.Tools;
using Dal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BL
{
    public class Order
    {
        public long Id { get; set; }
        public long ShopId { get; set; }
        public long? UserId { get; set; }
        public string OrderObj { get; set; }
        public DateTime CreatedAt{ get; set; }
        public DateTime? EstimatedArrivalAt { get; set; }
        public DateTime? ShopApproveAt { get; set; }
        public DateTime? OrderReadyAt { get; set; }
        public DateTime? ShopRejectedAt { get; set; }
        public string RejectedMsg { get; set; }

        CRUD dal;
        GlobalData gd;
        public  Order()
        {
            
        }

        public Order(GlobalData g)
        {
            gd = g;
        }

        public Order(GlobalData g, long id)
        {
            gd = g;
            this.Id = id;
        }

        public void AddNew(GlobalData g, User user)
        {
            string EstimatedArrivalAt = this.EstimatedArrivalAt.HasValue ? this.EstimatedArrivalAt.Value.ToString("yyyy-MM-dd HH:mm") : string.Empty;
            long lastInsertId = 0;
            dal = new CRUD(g.ConnectionString);
            StringBuilder sSql = new StringBuilder();
            this.CreatedAt = DateTime.Now;
            
            sSql.Append("insert into orders (ShopId, UserId, OrderObj, CreatedAt, EstimatedArrivalAt) values(");
            sSql.Append(this.ShopId + ",");
            sSql.Append(user.Id + ",");
            sSql.Append("'" + this.OrderObj + "',");
            sSql.Append("'" + this.CreatedAt.ToString("yyyy-MM-dd HH:mm") + "',");
            sSql.Append("'" + EstimatedArrivalAt + "')");
            

            dal.ExecuteNonQuery(sSql.ToString(), ref lastInsertId);
            this.Id = lastInsertId;
        }

        public void UserUpdateArrivalTime(string arrivalTime, int userId)
        {
            this.EstimatedArrivalAt = Common.Tools.CommonFunctions.ConvertToDateTime(arrivalTime);
            string EstimatedArrivalAt = this.EstimatedArrivalAt.HasValue ? this.EstimatedArrivalAt.Value.ToString("yyyy-MM-dd HH:mm") : string.Empty;

            dal = new CRUD(gd.ConnectionString);
            StringBuilder sSql = new StringBuilder();

            sSql.Append("update orders set EstimatedArrivalAt = ");
            
            sSql.Append("'" + EstimatedArrivalAt + "' ");
            sSql.Append("where Id=" + this.Id);
            sSql.Append(" and UserId = " + userId);
            int numberOfRecords = dal.ExecuteNonQuery(sSql.ToString());

            if (numberOfRecords == 0)
                throw new Exception("Order No " + this.Id + " dose not exists or not belong to the user.");
        }

        public void ShopApprove(int shopId)
        {
            
            dal = new CRUD(gd.ConnectionString);
            StringBuilder sSql = new StringBuilder();

            sSql.Append("update orders set ShopapproveAt = ");
            sSql.Append("'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "' ");
            sSql.Append("where Id=" + this.Id);
            sSql.Append(" and ShopId=" + shopId);

            int numberOfRecords = dal.ExecuteNonQuery(sSql.ToString());

            if (numberOfRecords == 0)
                throw new Exception("Order " + this.Id + " dose not exists or not belong to the shop.");
        }

        public void Ready(int shopId)
        {
            dal = new CRUD(gd.ConnectionString);
            StringBuilder sSql = new StringBuilder();

            sSql.Append("update orders set OrderReadyAt = ");
            sSql.Append("'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "' ");
            sSql.Append("where Id=" + this.Id);
            sSql.Append(" and ShopId=" + shopId);

            int numberOfRecords = dal.ExecuteNonQuery(sSql.ToString());

            if (numberOfRecords == 0)
                throw new Exception("Order " + this.Id + " dose not belog to shop");
        }


        public void Delivered(int shopId)
        {
            dal = new CRUD(gd.ConnectionString);
            StringBuilder sSql = new StringBuilder();

            sSql.Append("update orders set OrderdeliveredAt = ");
            sSql.Append("'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "' ");
            sSql.Append("where Id=" + this.Id);
            sSql.Append(" and ShopId=" + shopId);
            int numberOfRecords = dal.ExecuteNonQuery(sSql.ToString());

            if (numberOfRecords == 0)
                throw new Exception("Order " + this.Id + " dose not exists or not belong to the shop.");
        }

        public void RejectOrder(RejectOrder ro, int shopId)
        {
            dal = new CRUD(gd.ConnectionString);
            StringBuilder sSql = new StringBuilder();

            sSql.Append("update orders set ShopRejectedAt = ");
            sSql.Append("'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "', ");
            sSql.Append(" RejectedMsg = '" + ro.RejectMsg + "'");
            sSql.Append(" where Id=" + this.Id);
            sSql.Append(" and ShopId=" + shopId);
            int numberOfRecords = dal.ExecuteNonQuery(sSql.ToString());

            if (numberOfRecords == 0)
                throw new Exception("Order " + this.Id + " dose not exists or not belong to the shop.");
        }


        public List<OrdersResponse> GetOrders(OrdersQueryParams oqp, Shop shop)
        {
            oqp.ShopId = shop.Id;
            oqp.UserId = null;
            return getOrders(oqp);
        }

        public List<OrdersResponse> GetOrder(int userId, long orderId)
        {
            OrdersQueryParams oqp = new OrdersQueryParams();
            oqp.OrderId = orderId;
            oqp.UserId = userId;
            return getOrders(oqp);
        }

        public List<OrdersResponse> GetOrders(OrdersQueryParams oqp, User user)
        {
            oqp.UserId = user.Id;
            oqp.ShopId = null;
            return getOrders(oqp);
        }

        

        private List<OrdersResponse> getOrders(OrdersQueryParams oqp)
        {
            oqp.Validate();
            string name = "";
            string whereClause = "";
            string fromClause = "";
            if (oqp.UserId.HasValue)
            {
                name = "BuisnessName, null fullname, null MobilePhoneNo, null Email";
                fromClause = " from orders,Shops ";
                whereClause = " where  Userid = " + oqp.UserId + " and orders.shopid=shops.id";
            }
            else
            {
                name = "fullname, null BuisnessName, MobilePhoneNo, Email";
                fromClause = "from orders,users ";
                whereClause = "where shopid = " + oqp.ShopId + " and orders.userid=users.id";
            }

            if (oqp.OrderId.HasValue)
                whereClause = whereClause + " and orders.Id = " + oqp.OrderId;
            
            StringBuilder sSql = new StringBuilder();

            sSql.Append("select orders.Id OrderId," + name + ", CreatedAt, EstimatedArrivalAt, ShopApproveAt, OrderReadyAt, ShopRejectedAt, OrderdeliveredAt, RejectedMsg, OrderObj ");
            sSql.Append(fromClause);
            sSql.Append(whereClause);

            if (!oqp.OrderId.HasValue)
            {
                if (oqp.IsDelivered.HasValue && oqp.IsDelivered == false)
                    sSql.Append(" and OrderDeliveredAt is null");

                if (oqp.IsDelivered.HasValue && oqp.IsDelivered == true)
                    sSql.Append(" and OrderDeliveredAt is not null");

                if (oqp.IsReady.HasValue && oqp.IsReady == false)
                    sSql.Append(" and OrderReadyAt is null");

                if (oqp.IsReady.HasValue && oqp.IsReady == true)
                    sSql.Append(" and OrderReadyAt is not null");

                if (oqp.IsRejected.HasValue && oqp.IsRejected == false)
                    sSql.Append(" and ShopRejectedAt is null");

                if (oqp.IsRejected.HasValue && oqp.IsRejected == true)
                    sSql.Append(" and ShopRejectedAt is not null");


                sSql.Append(" and CreatedAt >= '" + oqp.FromDate.ToString("yyyy-MM-dd HH:mm") + "'");
                sSql.Append(" and CreatedAt <= '" + oqp.ToDate.ToString("yyyy-MM-dd HH:mm") + "'");

                //sSql.Append(" and CreatedAt > '" + DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd hh:mm") + "'");
                sSql.Append(" order by EstimatedArrivalAt");
            }

            dal = new CRUD(gd.ConnectionString);
            DataTable dt = new DataTable();
            dal.ExecuteQuery(sSql.ToString(), ref dt);

            return toList(dt);
        }


        private List<OrdersResponse> toList(DataTable dt)
        {
            List<OrdersResponse> ol = new List<OrdersResponse>();

            OrdersResponse sro;
            foreach (DataRow dr in dt.Rows)
            {
                sro = new OrdersResponse();

                sro.OrderId = long.Parse(dr["OrderId"].ToString());
                sro.UserName = CommonFunctions.IsNullOrEmpty(dr["fullname"]);
                sro.BuisnessName = CommonFunctions.IsNullOrEmpty(dr["BuisnessName"]);
                sro.MobilePhoneNo = CommonFunctions.IsNullOrEmpty(dr["MobilePhoneNo"]);
                sro.Email = CommonFunctions.IsNullOrEmpty(dr["Email"]);
                sro.CreatedAt = CommonFunctions.ConvertToDateTime(dr["CreatedAt"]);
                sro.EstimatedArrivalAt = CommonFunctions.ConvertToDateTime(dr["EstimatedArrivalAt"]);
                sro.ShopApproveAt = CommonFunctions.ConvertToDateTime(dr["ShopApproveAt"]);
                sro.OrderReadyAt = CommonFunctions.ConvertToDateTime(dr["OrderReadyAt"]);
                sro.ShopRejectedAt = CommonFunctions.ConvertToDateTime(dr["ShopRejectedAt"]);
                sro.OrderdeliveredAt = CommonFunctions.ConvertToDateTime(dr["OrderdeliveredAt"]);
                
                sro.RejectedMsg = dr["RejectedMsg"].ToString();
                sro.OrderObj = dr["OrderObj"].ToString();

                ol.Add(sro);
            }

            return ol;
        }


    }
}
