using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HosSystem.Models
{
    public class Patient
    {
        public string HN { get; set; }
        public string RegNo { get; set; }
        public string AN { get; set; }
        public string Name { get; set; }
       // public DateTime Birthday { get; set; }
        public string Age { get; set; }
        public string Ward { get; set; }
        public string Room { get; set; }
        public string Bed { get; set; }
    }
}