using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HosSystem.Models
{
    public class ViewSummaryOrder
    {
        public List<PTLOrder> viewOrders = new List<PTLOrder>();
        public List<Patient> patients = new List<Patient>();
        public List<Ipd_NnoteD> ipd_NnoteDs = new List<Ipd_NnoteD>();

    }
}