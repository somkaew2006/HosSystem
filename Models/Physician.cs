using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HosSystem.Models
{
    public class Physician
    {
        public string DocCode { get; set; }
        public string DocName { get; set; }
        public string DocLName { get; set; }
        public string DocDept { get; set; }
        public string DocTitle { get; set; }
        public string FullName { get { return DocTitle + " " + DocName + " " + DocLName; } }
    }
}