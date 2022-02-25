using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HosSystem.Models
{
    public class LabMenuHead
    {
        public string hn { get; set; }
         public string req_no { get; set; }
        public string req_date { get; set; }
        public string req_time { get; set; }
        public string lab_type { get; set; }
        public string reg_flag { get; set; }

        public string lab_menu_name
        {
            get
            {
                //25640127
                string year = req_date.Substring(0, 4);
                string month = req_date.Substring(4, 2);
                string day = req_date.Substring(6, 2);
                
                string hh = req_time.Substring(0, 2);
                string mm = req_time.Substring(2, 2);

                return lab_type + " " + day + "/" + month +"/"+year + " " + hh +":" + mm ;
            }
        }

        public List<LabMenuDetail> labMenuDetails = new List<LabMenuDetail>();
    }
}