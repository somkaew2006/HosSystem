using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HosSystem.Models
{
    public class ViewLab
    {
        public List<LabMenuHead> labMenuHeads = new List<LabMenuHead>();
        public Patient patient { get; set; }

    }
}