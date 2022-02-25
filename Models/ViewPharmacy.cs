using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HosSystem.Models
{
    public class ViewPharmacy
    {
        //public List<Patient> patients = new List<Patient>();
        public List<PTLOrder> pTLOrderTopTen = new List<PTLOrder>();
        public List<PTLOrder> pTLOrderInfo = new List<PTLOrder>();
        public string Search { get; set; }
        public string WardID { get; set; }
    }
}