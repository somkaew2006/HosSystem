using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HosSystem.Models
{
    public class PTLOrder
    {
       
        public string AN { get; set; }
        public string HN { get; set; }
        public string OrderID { get; set; }
        public string RegNo { get; set; }
        public string ProgessNote { get; set; }
        public string OrderOneDay { get; set; }
        public string OrderContinue { get; set; }
        public string OrderDetail { get; set; }
        public string PatientName { get; set; }
        public string OrderDate { get; set; }
        public string OrderTime { get; set; }
        public string OrderStatus { get; set; }

    }
}