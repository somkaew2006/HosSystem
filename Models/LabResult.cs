using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HosSystem.Models
{
    public class LabResult
    {
        public string req_no { get; set; }
        public string res_date { get; set; }
        public string res_time { get; set; }
        public string result_name { get; set; }
        public string resNormal { get; set; }
        public string real_res { get; set; }
        public string low_normal { get; set; }
        public string high_normal { get; set; }
        public string lab_type { get; set; }
        public string lab_code { get; set; }
        public string remark { get; set; }


        public bool IsTextRed
        {
            get
            {
                if (low_normal == "" && high_normal == "")
                {
                    return false;
                }
                else
                {
                    if  ( Convert.ToDecimal( real_res)>= Convert.ToDecimal( low_normal) &&Convert.ToDecimal( real_res)<=Convert.ToDecimal( high_normal))
                    {
                        return false;
                    }
                    else
                    {
                        return true;   
                    }
                }
            }
        }

        public string ReferenceRate
        {
            get
            {
                if (low_normal=="" && high_normal=="")
                {
                    return "-";
                }
                return low_normal + " - " + high_normal;
            }
        }
    }
}