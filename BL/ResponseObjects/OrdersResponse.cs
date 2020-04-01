using System;
using System.Collections.Generic;
using System.Text;

namespace BL.ResponseObjects
{


    
    public class OrdersResponse
    {
        public long OrderId { get; set; }
        public string UserName { get; set; }

        public string BuisnessName { get; set; }

        public string MobilePhoneNo { get; set; }
        
        public string Email { get; set; }

        public string OrderObj { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? EstimatedArrivalAt { get; set; }
        public DateTime? ShopApproveAt { get; set; }
        public DateTime? OrderReadyAt { get; set; }
        public DateTime? ShopRejectedAt { get; set; }
        public string RejectedMsg { get; set; }
       
        public DateTime? OrderdeliveredAt { get; set; } 


    }
}
