using DatabaseHelper;
using HosSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HosSystem.Repository
{
    public class Ipd_NnoteDRepo
    {
        public List<Ipd_NnoteD> GetNnoteD(string HN,string RegNo,int EndRow)
        {
            int GetTo = EndRow + 3;
            string SQLCommand = @"select * from 
	                                (
	                                 select  ROW_NUMBER() OVER(ORDER BY CONVERT(INT, runno) desc) AS Row,hn, regNo,ndate,ntime,runno,pb_list,assessment,intervention,evaluation   From Ipd_NnoteD where hn =@HN and regNo=@RegNo  
	                                )ta  where Row between 1 and @GetTo ";

            List<Ipd_NnoteD> ipd_NnoteDs = new List<Ipd_NnoteD>();
            using (var db = new DBHelper(GlobalVar.connectionString))
            {
                try
                {
                    db.AddParameter("@HN",HN );
                    db.AddParameter("@RegNo", RegNo);
                    db.AddParameter("@GetTo", GetTo);
                    var reader = db.ExecuteReader(SQLCommand);

                    while (reader.Read())
                    {
                        Ipd_NnoteD ipd_NnoteD = new Ipd_NnoteD();
                        ipd_NnoteD.hn = (string)reader["hn"];
                        ipd_NnoteD.regNo = (string)reader["regNo"];
                        ipd_NnoteD.ndate = (string)reader["ndate"];
                        ipd_NnoteD.ntime = (string)reader["ntime"];
                        ipd_NnoteD.runno = (string)reader["runno"];
                        ipd_NnoteD.pb_list = (string)reader["pb_list"];
                        ipd_NnoteD.assessment = (string)reader["assessment"];
                        ipd_NnoteD.intervention = (string)reader["intervention"];
                        ipd_NnoteD.evaluation = (string)reader["evaluation"];

                        ipd_NnoteDs.Add(ipd_NnoteD);
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }

            return ipd_NnoteDs;
        }
    }
}