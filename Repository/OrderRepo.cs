using DatabaseHelper;
using HosSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using WebGrease;

namespace HosSystem.Repository
{
    public class OrderRepo
    {
        public List<PTLOrder> GetOrder(string AN, int EndRow)
        {
            int GetTo = EndRow + 3;
            //string SQLCommand = @"select * from 
            //                        (
            //                            select ROW_NUMBER() OVER(ORDER BY order_id desc) AS Row, AN,order_id, progessNote,order_oneday,order_continue from ptl_order   where AN =@AN  
            //                        )ta  where Row between 1 and @GetTo  ";


            //test ดึงข้อมูลเทส ptl_order
            string SQLCommand = @"select * from 
                                    (
                                        
                                        select ROW_NUMBER() OVER(ORDER BY order_id desc) AS Row, ord.AN,order_id, progessNote,order_oneday,order_continue ,isnull(order_detail,'') as order_detail 
                                         from ptl_order ord left join ptl_order_t ordt
                                         on ord.order_t = ordt.order_t
                                         where ord.AN =@AN  and showFlag <>'N'
                                    )ta  where Row between 1 and @GetTo  ";


            List<PTLOrder> orders = new List<PTLOrder>();
            using (var db = new DBHelper(GlobalVar.connectionString))
            {
                try
                {
                    db.AddParameter("@AN", AN);
                    //db.AddParameter("@GetStart", GetStart);
                    db.AddParameter("@GetTo", GetTo);
                    var reader = db.ExecuteReader(SQLCommand);


                    while (reader.Read())
                    {
                        PTLOrder order = new PTLOrder();
                        order.AN = (string)reader["AN"];
                        order.OrderID = (string)reader["order_id"];
                        order.ProgessNote = (string)reader["progessNote"];
                        order.OrderOneDay = (string)reader["order_oneday"];
                        order.OrderContinue = (string)reader["order_continue"];
                        order.OrderDetail = (string)reader["order_detail"];

                        orders.Add(order);
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }

            return orders;
        }

       

        // create order มาจากหน้า consult
        public Result CreateOrderAndUpdateConsult(string AN, string RegNo, string HN, string DocID, string Comment)
        {
            Result result = new Result();
            List<ptl_consult> ptl_Consults = new ConsultRepo().GetConsults(AN);
            string progessNote = "";
            List<string> cs_ids = new List<string>();
            foreach (ptl_consult item in ptl_Consults)
            {
                progessNote += " เรียนปรึกษาแพทย์เรื่อง " + item.cs_header + " รายละเอียด " + item.cs_detail + " ผู้ปรึกษา " + item.cs_cname + " " + item.cs_date + ", ";
                cs_ids.Add("'" + item.cs_id + "'");
            }
            string cs_id = string.Join(",", cs_ids);


            //insert ptl_order and update ptl_consult
            using (var db = new DBHelper(GlobalVar.connectionString))
            {
                StringBuilder sql = new StringBuilder();
                try
                {
                    db.BeginTransaction();
                    sql.Append(@"
                                    DECLARE @MaxOrderID VARCHAR(30); 
                                    DECLARE @Prefix VARCHAR(30);
                                    DECLARE @Year VARCHAR(4);
                                    DECLARE @Month VARCHAR(2);
                                    DECLARE @Day VARCHAR(2);
                                    DECLARE @Date VARCHAR(10);
                                    DECLARE @RunNo int; 
                                    DECLARE @OrderID VARCHAR(30);
                                    DECLARE @OrderTime VARCHAR(10);

                                    set @Year = CAST(YEAR(getdate()) AS int)  +543
                                    set @Month = RIGHT( '0' + convert( varchar(2) , month(getdate()) ),  2 )
                                    set @Day = RIGHT( '0' + convert( varchar(2) , day(getdate()) ),  2 )
                                    set @Date=@Year + @Month + @Day

                                    set @MaxOrderID =(select top 1 order_id from ptl_order where order_date = @Date order by order_time desc , order_id desc )
 
                                    set @RunNo= substring(isnull(@MaxOrderID,0),9,10) + 1

                                    set @OrderID = @Year +@Month + @Day+ Cast(@RunNo as VARCHAR)
                                    set @OrderTime =(SELECT cast( DATEPART(HOUR, GETDATE()) as varchar ) +':' +cast( DATEPART(MINUTE, GETDATE()) as varchar))
                    
                                    insert into ptl_order(AN,order_id,order_date,order_time,order_doc,
                                        order_status,progessNote,hn,regNo,showFlag)

                                        values(@AN,@OrderID,@Date,@OrderTime,@doc_id,'N',
                                        @ProgessNote,@HN,@RegNo,'Y')


                                    update ptl_consult set cs_status='A',acc_doc=@doc_id,
                                    doc_comment=@doc_comment,acc_date=@Date,acc_time=getdate(),
                                    order_id=@OrderID where cs_id in " + "(" + cs_id + ")");


                    db.AddParameter("@AN", AN);
                    db.AddParameter("@doc_id", DocID);
                    db.AddParameter("@ProgessNote", progessNote);
                    db.AddParameter("@HN", HN);
                    db.AddParameter("@RegNo", RegNo);

                    db.AddParameter("@doc_comment", Comment);



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

        //create order มากจาก หน้า แพทย์
        public Result CreateOrder(string AN, string RegNo, string HN, string DocID, string order_continue, string order_oneday, string progressNote, string order_detail)
        {
            Result result = new Result();



            //List<ptl_consult> ptl_Consults = new ConsultRepo().GetConsults(AN);
            //string progessNote = "";
            //List<string> cs_ids = new List<string>();
            //foreach (ptl_consult item in ptl_Consults)
            //{
            //    progessNote += " เรียนปรึกษาแพทย์เรื่อง " + item.cs_header + " รายละเอียด " + item.cs_detail + " ผู้ปรึกษา " + item.cs_cname + " " + item.cs_date + ", ";
            //    cs_ids.Add("'" + item.cs_id + "'");
            //}
            //string cs_id = string.Join(",", cs_ids);


            using (var db = new DBHelper(GlobalVar.connectionString))
            {
                StringBuilder sql = new StringBuilder();
                try
                {
                    db.BeginTransaction();
                    sql.Append(@"
                                    DECLARE @MaxOrderID VARCHAR(30); 
                                    DECLARE @Prefix VARCHAR(30);
                                    DECLARE @Year VARCHAR(4);
                                    DECLARE @Month VARCHAR(2);
                                    DECLARE @Day VARCHAR(2);
                                    DECLARE @Date VARCHAR(10);
                                    DECLARE @RunNo int; 
                                    DECLARE @OrderID VARCHAR(30);
                                    DECLARE @OrderTime VARCHAR(10);
                                    DECLARE @order_t varchar(10)

                                    set @Year = CAST(YEAR(getdate()) AS int)  +543
                                    set @Month = RIGHT( '0' + convert( varchar(2) , month(getdate()) ),  2 )
                                    set @Day = RIGHT( '0' + convert( varchar(2) , day(getdate()) ),  2 )
                                    set @Date=@Year + @Month + @Day

                                    set @MaxOrderID =(select top 1 order_id from ptl_order where order_date = @Date order by order_time desc , order_id desc )
 
                                    set @RunNo= substring(isnull(@MaxOrderID,0),9,10) + 1

                                    set @OrderID = @Year +@Month + @Day+ Cast(@RunNo as VARCHAR)
                                    set @OrderTime =(SELECT cast( DATEPART(HOUR, GETDATE()) as varchar ) +':' +cast( DATEPART(MINUTE, GETDATE()) as varchar))
                    
                                    insert into ptl_order(AN,order_id,order_date,order_time,order_doc,
                                        order_status,order_continue,order_oneday,progessNote,hn,regNo,showFlag)

                                        values(@AN,@OrderID,@Date,@OrderTime,@doc_id,'E',
                                        @order_continue,@order_oneday,@ProgessNote,@HN,@RegNo,'Y')

                                  

                                   ");

                    // มี แนบ file ต้อง insert ptl_order_t and update ptl_order
                    StringBuilder sqlOrderT = new StringBuilder();
                    if (order_detail != "")
                    {

                        sqlOrderT.Append(@"
                                    set @order_t= (select max(CAST(order_t AS int))+1 as MaxOrder_t  from   ptl_order)

                                    insert into ptl_order_t( order_t, AN , hn, regNo, order_detail)
                                    values( @order_t,@AN, @HN, @RegNo, @order_detail)

                                    update ptl_order set order_t =@order_t where order_id =@OrderID 
                                    ");

                        db.AddParameter("@order_detail", order_detail);
                    }


                    db.AddParameter("@AN", AN);
                    db.AddParameter("@doc_id", DocID);
                    db.AddParameter("@HN", HN);
                    db.AddParameter("@RegNo", RegNo);

                    db.AddParameter("@order_continue", order_continue);
                    db.AddParameter("@order_oneday", order_oneday);
                    db.AddParameter("@ProgessNote", progressNote);




                    string sqlExecute = sql.ToString() + " " + sqlOrderT.ToString();

                    db.ExecuteNonQuery(sqlExecute, DbConnectionState.KeepOpen);

                    // เก็บ file image 



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

        public Result CheckOrderStatus(string DocID, string HN, string AN, string RegNo)
        {
            Result result = new Result();
            result.Flag = false;
            string SQLCommand = @"select order_id from ptl_order where order_doc=@DocID and hn=@HN and AN=@AN and regNo=@RegNo and order_status='N' and showFlag='Y' ";


            using (var db = new DBHelper(GlobalVar.connectionString))
            {
                try
                {
                    db.AddParameter("@DocID", DocID);
                    db.AddParameter("@HN", HN);
                    db.AddParameter("@AN", AN);
                    db.AddParameter("@RegNo", RegNo);

                    var reader = db.ExecuteReader(SQLCommand);


                    if (reader.Read())
                    {
                        // มี order ค้าง
                        result.Flag = true;

                    }
                }
                catch (Exception ex)
                {
                    // มี order ค้าง
                    result.Flag = true;
                    throw ex;
                }
            }

            return result;
        }


        //check ว่าเป็น order ของตัวเองหรือเปล่า
        public Result CheckOrderOwner(string DocID, string OrderID)
        {
            Result result = new Result();
            result.Flag = false;
            string SQLCommand = @"select order_id from ptl_order where order_id=@OrderID  and order_doc=@DocID";



            using (var db = new DBHelper(GlobalVar.connectionString))
            {
                try
                {
                    db.AddParameter("@DocID", DocID);
                    db.AddParameter("@OrderID", OrderID);

                    var reader = db.ExecuteReader(SQLCommand);


                    if (reader.Read())
                    {
                        // เป็น order ของตัวเอง สามารถแก้ไขได้/ลบได้
                        result.Flag = true;

                    }
                }
                catch (Exception ex)
                {
                    // มี order ค้าง
                    result.Flag = true;
                    throw ex;
                }
            }

            return result;
        }

        //showFlag='N' เป็น การ Delete
        public Result UpdateOrderStatus(string OrderID)
        {
            Result result = new Result();
            string SQLCommand = @"Update  ptl_order
                                    set showFlag='N'
                                  where order_id=@OrderID  ";


            using (var db = new DBHelper(GlobalVar.connectionString))
            {
                try
                {
                    db.BeginTransaction();

                    db.AddParameter("@OrderID", OrderID);

                    db.ExecuteNonQuery(SQLCommand.ToString(), DbConnectionState.KeepOpen);


                    db.CommitTransaction();
                    result.Flag = true;


                }
                catch (Exception ex)
                {
                    db.RollbackTransaction();
                    result.Flag = false;
                    throw ex;
                }
            }

            return result;
        }

        public List<PTLOrder> GetTopTenOrder()
        {
            //test ดึงข้อมูลเทส ptl_order
            string SQLCommand = @"select top 10 a.order_id,a.hn,pat.firstName + ' '+ pat.lastName as FullName,a.order_date
                                    from ptl_order a 
                                    inner join Ipd_h b on a.hn=b.hn and a.regNo=b.regist_flag
                                    left join ptl_order_t c on a.order_t=c.order_t
                                    left join [dbo].[PATIENT] pat on a.hn = pat.hn
                                    where order_status='E' and showFlag='Y' and isnull(a.order_acc_ph,'')=''
                                    and isnull(b.discharge_date,'')='' 
                                    Order by a.order_id     
                                       ";

            
            List<PTLOrder> orders = new List<PTLOrder>();
            using (var db = new DBHelper(GlobalVar.connectionString))
            {
                try
                {
                    
                    var reader = db.ExecuteReader(SQLCommand);


                    while (reader.Read())
                    {
                        PTLOrder order = new PTLOrder();
                        order.HN = (string)reader["HN"];
                        order.OrderID = (string)reader["order_id"];
                        order.PatientName = (string)reader["FullName"];
                        order.OrderDate = (string)reader["order_date"];
                        

                        orders.Add(order);
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }

            return orders;
        }
        public List<PTLOrder> GetTopTenOrder(string WardID)
        {
            //test ดึงข้อมูลเทส ptl_order
            string SQLCommand = @"select top 10 a.order_id,a.hn,pat.firstName + ' '+ pat.lastName as FullName,a.order_date
                                    from ptl_order a 
                                    inner join Ipd_h b on a.hn=b.hn and a.regNo=b.regist_flag
                                    left join ptl_order_t c on a.order_t=c.order_t
                                    left join [dbo].[PATIENT] pat on a.hn = pat.hn
                                    where order_status='E' and showFlag='Y' and isnull(a.order_acc_ph,'')=''
                                    and isnull(b.discharge_date,'')='' 
                                    and b.ward_id=@WardID
                                    and (a.nures_acc_id is null or a.nures_acc_id ='')
                                    Order by a.order_id     
                                       ";

            
            List<PTLOrder> orders = new List<PTLOrder>();
            using (var db = new DBHelper(GlobalVar.connectionString))
            {
                try
                {
                    db.AddParameter("@WardID", WardID);
                    var reader = db.ExecuteReader(SQLCommand);


                    while (reader.Read())
                    {
                        PTLOrder order = new PTLOrder();
                        order.HN = (string)reader["HN"];
                        order.OrderID = (string)reader["order_id"];
                        order.PatientName = (string)reader["FullName"];
                        order.OrderDate = (string)reader["order_date"];


                        orders.Add(order);
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }

            return orders;
        }

        public List<PTLOrder> GetOrderSearch(string Search)
        {

            string SQLCommand = @"select a.*,isnull(c.order_detail,'') as order_detail,pat.firstName + ' '+ pat.lastName as FullName 
                                    from ptl_order a 
                                    inner join Ipd_h b on a.hn=b.hn and a.regNo=b.regist_flag
                                    left join ptl_order_t c on a.order_t=c.order_t
                                    left join [dbo].[PATIENT] pat on a.hn = pat.hn
                                    where order_status='E' and showFlag='Y' and isnull(a.order_acc_ph,'')=''
                                    and isnull(b.discharge_date,'')='' and (a.order_id =@Search or ltrim(a.hn)=@Search)
                                    Order by a.order_id    
                                       ";


            List<PTLOrder> orders = new List<PTLOrder>();
             
            using (var db = new DBHelper(GlobalVar.connectionString))
            {
                try
                {
                    db.AddParameter("@Search", Search);
                    var reader = db.ExecuteReader(SQLCommand);


                    while (reader.Read())
                    {
                        PTLOrder order = new PTLOrder();
                        order.HN = (string)reader["HN"];
                        order.AN = (string)reader["AN"];
                        order.RegNo = (string)reader["RegNo"];
                        order.OrderID = (string)reader["order_id"];
                        order.PatientName = (string)reader["FullName"];
                        order.OrderDate = (string)reader["order_date"];
                        order.OrderTime = (string)reader["order_time"];
                      
                        order.OrderStatus = (string)reader["order_status"];
                        order.OrderDetail = (string)reader["order_detail"];
                        order.ProgessNote = (string)reader["progessNote"];
                        order.OrderOneDay = (string)reader["order_oneday"];
                        order.OrderContinue = (string)reader["order_continue"];


                        orders.Add(order);
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
            return orders;

        }

        public List<PTLOrder> GetOrderSearch(string Search,string WardID)
        {

            string SQLCommand = @"select a.*,isnull(c.order_detail,'') as order_detail,pat.firstName + ' '+ pat.lastName as FullName 
                                    from ptl_order a 
                                    inner join Ipd_h b on a.hn=b.hn and a.regNo=b.regist_flag
                                    left join ptl_order_t c on a.order_t=c.order_t
                                    left join [dbo].[PATIENT] pat on a.hn = pat.hn
                                    where order_status='E' and showFlag='Y' and isnull(a.order_acc_ph,'')=''
                                    and isnull(b.discharge_date,'')='' and (a.order_id =@Search or ltrim(a.hn)=@Search)
                                    and b.ward_id=@WardID
                                    and (a.nures_acc_id is null or a.nures_acc_id ='')
                                    Order by a.order_id    
                                       ";


            List<PTLOrder> orders = new List<PTLOrder>();

            using (var db = new DBHelper(GlobalVar.connectionString))
            {
                try
                {
                    db.AddParameter("@Search", Search);
                    db.AddParameter("@WardID", WardID);
                    var reader = db.ExecuteReader(SQLCommand);


                    while (reader.Read())
                    {
                        PTLOrder order = new PTLOrder();
                        order.HN = (string)reader["HN"];
                        order.AN = (string)reader["AN"];
                        order.RegNo = (string)reader["RegNo"];
                        order.OrderID = (string)reader["order_id"];
                        order.PatientName = (string)reader["FullName"];
                        order.OrderDate = (string)reader["order_date"];
                        order.OrderTime = (string)reader["order_time"];
                        order.OrderStatus = (string)reader["order_status"];
                        order.OrderDetail = (string)reader["order_detail"];
                        order.ProgessNote = (string)reader["progessNote"];
                        order.OrderOneDay = (string)reader["order_oneday"];
                        order.OrderContinue = (string)reader["order_continue"];


                        orders.Add(order);
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
            return orders;

        }
        //หนา เภสัช
        public Result AcceptOrder(string OrderID,string UserID)
        {
            Result result = new Result();
            string SQLCommand = @"update ptl_order set order_acc_ph=@UserID, acc_ph_time=GETDATE() where order_id=@OrderID ";


            using (var db = new DBHelper(GlobalVar.connectionString))
            {
                try
                {
                    db.BeginTransaction();

                    db.AddParameter("@OrderID", OrderID);
                    db.AddParameter("@UserID", UserID);

                    db.ExecuteNonQuery(SQLCommand.ToString(), DbConnectionState.KeepOpen);


                    db.CommitTransaction();
                    result.Flag = true;


                }
                catch (Exception ex)
                {
                    db.RollbackTransaction();
                    result.Flag = false;
                    throw ex;
                }
            }

            return result;
        }

        // หน้าพยาบาล
        public Result AcceptOrderByNures(string OrderID, string UserID)
        {
            Result result = new Result();
            string SQLCommand = @"DECLARE @Year VARCHAR(4);
                                    DECLARE @Month VARCHAR(2);
                                    DECLARE @Day VARCHAR(2);
                                    DECLARE @Date VARCHAR(10);
                                    DECLARE @Time VARCHAR(10);

                                    set @Year = CAST(YEAR(getdate()) AS int)  +543
                                    set @Month = RIGHT( '0' + convert( varchar(2) , month(getdate()) ),  2 )
                                    set @Day = RIGHT( '0' + convert( varchar(2) , day(getdate()) ),  2 )
                                    set @Date=@Year + @Month + @Day
                                    set @Time = CONVERT(VARCHAR(5), GETDATE(),8) 

                                    update ptl_order 
                                    set nures_acc_id=@UserID,
                                     nures_accDate=@Date,
                                     nures_accTime =@Time
                                     where order_id=@OrderID  ";


            using (var db = new DBHelper(GlobalVar.connectionString))
            {
                try
                {
                    db.BeginTransaction();

                    db.AddParameter("@OrderID", OrderID);
                    db.AddParameter("@UserID", UserID);

                    db.ExecuteNonQuery(SQLCommand.ToString(), DbConnectionState.KeepOpen);


                    db.CommitTransaction();
                    result.Flag = true;


                }
                catch (Exception ex)
                {
                    db.RollbackTransaction();
                    result.Flag = false;
                    throw ex;
                }
            }

            return result;
        }
    }

  
}