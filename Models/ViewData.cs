using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HosSystem.Models
{
    public class ViewData
    {
        public string UserID { get; set; }
        public Patient patient { get; set; }
        public List<Patient> patients = new List<Patient>();
        public List<ptl_consult> ptl_Consults = new List<ptl_consult>();
    }
}