using DatabaseHelper;
using HosSystem.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace HosSystem.Repository
{
    public class ConsultRepo
    {
        PTLEntities context = new PTLEntities();

        public bool IsConsultStatusN(string AN)
        {

            string SQLCommand = @"select AN from ptl_consult where AN=@AN and cs_status ='N'";

            Profile profile = new Profile();
            using (var db = new DBHelper(GlobalVar.connectionString))
            {
                try
                {
                    db.AddParameter("@AN", AN);
                    
                    var reader = db.ExecuteReader(SQLCommand);


                    if (reader.Read())
                    {
                        // มี consutl
                        return true;
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }

            return false;
        }
        //public bool IsConsultStatusN(string AN)
        //{
            
        //    using (var context = new PTLEntities())
        //    {
        //        if (context.ptl_consult.Where(c => c.AN == AN && c.cs_status=="N").Count()>0)
        //        {
        //            // มี consutl
        //            return true;
        //        }
                
        //    }
        //    // ไม่มี consult
        //    return false;
        //}

        public List<ptl_consult> GetConsults(string AN)
        {
            //return consult ที่ค้างอยู่ status =N
            using (var context = new PTLEntities())
            {
                return context.ptl_consult.Where(c => c.AN == AN && c.cs_status == "N").ToList();
            }

             
        }

        public Result CreateConsult(string AN, string UserID,string CS_Header,string CS_Detail)
        {
            Result result = new Result();

            using (var db = new DBHelper(GlobalVar.connectionString))
            {
                StringBuilder sql = new StringBuilder();
                try
                {
                    db.BeginTransaction();
                    sql.Append(@"
                                    DECLARE @MaxOrderID VARCHAR(30); 
                                    DECLARE @Year VARCHAR(4);
                                    DECLARE @Month VARCHAR(2);
                                    DECLARE @Day VARCHAR(2);
                                    DECLARE @Date VARCHAR(10);
                                    DECLARE @RunNo int; 
                                    DECLARE @cs_id VARCHAR(30);


                                    set @Year = CAST(YEAR(getdate()) AS int)  +543
                                    set @Month = RIGHT( '0' + convert( varchar(2) , month(getdate()) ),  2 )
                                    set @Day = RIGHT( '0' + convert( varchar(2) , day(getdate()) ),  2 )
                                    set @Date=@Year  + @Month + @Day
                                    set @cs_id=substring(@Year,3,2) + @Month + @Day
                                    set @MaxOrderID =(select top 1 cs_id from ptl_consult where cs_date = @Date order by  cs_id desc )
                                    set @RunNo= substring(isnull(@MaxOrderID,0),7,4) + 1
 
                                    set @cs_id = substring(@Year,3,2) +@Month + @Day+ Right('0000'+ CONVERT(VARCHAR,@RunNo),4)


                                    INSERT INTO  ptl_consult (cs_id, AN, cs_date, cs_time, cs_cname, cs_header,
                                    cs_detail, cs_status)
                                    VALUES(@cs_id, @AN, @Date, GETDATE(), @user_id, @cs_header, @cs_detail, 'N')

                                   ");


                    db.AddParameter("@AN", AN);
                    db.AddParameter("@user_id", UserID);
                    db.AddParameter("@cs_header", CS_Header);
                    db.AddParameter("@cs_detail", CS_Detail);

                    db.ExecuteNonQuery(sql.ToString(), DbConnectionState.KeepOpen);


                    db.CommitTransaction();
                    result.Flag = true;
                }
                catch (Exception ex)
                {

                    db.RollbackTransaction();
                    result.Flag = false;
                    result.Message = ex.ToString();
                    return result;
                }
            }

            return result;
        }

    }
}