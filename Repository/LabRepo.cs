using DatabaseHelper;
using HosSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HosSystem.Repository
{
    public class LabRepo
    {
        public List<LabMenuHead> GetLabMenu(string hn ,string regNo)
        {

            string SQLCommand = @"select  hn,  isnull(req_no,'') as req_no,isnull(req_date,'') as req_date
                                ,isnull(req_time,'') as req_time,isnull(lab_type,'')
                                 as lab_type,isnull(reg_flag,'')  as reg_flag   from dbo.Labreq_h(nolock) 
                                where hn =@hn 
                                and reg_flag=@regNo
                                and lconfirm='Y'
                                AND res_ok='Y' 
                                and tot_res !='00'
                                ORDER BY lab_type ";
             

            List<LabMenuHead> labMenuHeads = new List<LabMenuHead>();
            using (var db = new DBHelper(GlobalVar.connectionString))
            {
                try
                {
                    db.AddParameter("@hn", hn);
                    db.AddParameter("@regNo", regNo);

                    var reader = db.ExecuteReader(SQLCommand);


                    while (reader.Read())
                    {
                        LabMenuHead labMenuHead = new LabMenuHead();
                        labMenuHead.hn = (string)reader["hn"];
                        labMenuHead.req_no = (string)reader["req_no"];
                        labMenuHead.req_date = (string)reader["req_date"];
                        labMenuHead.req_time = (string)reader["req_time"];
                        labMenuHead.lab_type = (string)reader["lab_type"];
                        labMenuHead.reg_flag = (string)reader["reg_flag"];
                        labMenuHead.labMenuDetails = GetLabDetail(labMenuHead.req_no);

                        labMenuHeads.Add(labMenuHead);
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }

            return labMenuHeads;
        }

        public List<LabMenuDetail> GetLabDetail(string req_no)
        {

            string SQLCommand = @"select req_no, lab_name from  dbo.Labreq_d where req_no=@req_no ";


            List<LabMenuDetail> labMenuDetails = new List<LabMenuDetail>();
            using (var db = new DBHelper(GlobalVar.connectionString))
            {
                try
                {
                    db.AddParameter("@req_no", req_no);

                    var reader = db.ExecuteReader(SQLCommand);


                    while (reader.Read())
                    {
                        LabMenuDetail labMenuDetail = new LabMenuDetail();
                        labMenuDetail.req_no = (string)reader["req_no"];
                        labMenuDetail.lab_name = (string)reader["lab_name"];

                        labMenuDetails.Add(labMenuDetail);
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }

            return labMenuDetails;
        }

        public List<LabResult> GetLabResult(string hn,string regNo,string req_no)
        {

            string SQLCommand = @"select  isnull(Labres_d.req_no,'') as req_no
                                        ,isnull(SUBSTRING(Labres_d.res_date,7,2)+'/'+SUBSTRING(Labres_d.res_date,5,2)+'/'+SUBSTRING(Labres_d.res_date,1,4),'') as res_date
                                        ,isnull(Labres_d.res_time,'') as res_time
                                        ,isnull(rtrim(Labres_d.result_name),'')as result_name  
                                        ,isnull(CASE WHEN Labres_d.resNormal ='Y' THEN 'ปกติ'ELSE 'ไม่ปกติ' END,'')as resNormal  
                                        ,isnull(CASE WHEN Labres_d.real_res='' 
                                        THEN (SELECT top 1 resText FROM Labres_m WHERE Labres_m.reqNo=Labres_d.req_no AND Labres_m.resultName=Labres_d.result_name)
                                        WHEN s.result_name LIKE '%HIV%' THEN '*รับผลที่ห้องแลป*'
                                        ELSE real_res END,'')as real_res  
                                        ,isnull(CASE WHEN Labres_d.low_normal='999999.999' THEN '' ELSE Labres_d.low_normal END ,'')as low_normal 
                                        ,isnull(CASE WHEN Labres_d.high_normal='999999.999' THEN '' ELSE Labres_d.high_normal END ,'')as high_normal 
                                        
                                        ,isnull(Labres_d.lab_type,'') as lab_type ,isnull(Labres_d.lab_code,'') as lab_code,isnull(Labres_d.remark,'') as  remark  

                                        from Labres_d(nolock) 
                                        LEFT JOIN Labre_s s(nolock) on Labres_d.lab_code=s.lab_code 
										AND Labres_d.res_item=s.res_run_no
                                        where Labres_d.hn=@hn and Labres_d.reg_flag=@regNo and 
										Labres_d.req_no=@req_no  order by s.printSeq ";


            List<LabResult> labResults = new List<LabResult>();
            using (var db = new DBHelper(GlobalVar.connectionString))
            {
                try
                {
                    db.AddParameter("@hn", hn);
                    db.AddParameter("@regNo", regNo);
                    db.AddParameter("@req_no", req_no);

                    var reader = db.ExecuteReader(SQLCommand);


                    while (reader.Read())
                    {
                        LabResult labResult = new LabResult();
                        labResult.req_no = (string)reader["req_no"];
                        labResult.res_date = (string)reader["res_date"];
                        labResult.res_time = (string)reader["res_time"];
                        labResult.result_name = (string)reader["result_name"];
                        labResult.resNormal = (string)reader["resNormal"];
                        labResult.real_res = (string)reader["real_res"];
                        labResult.low_normal = (string)reader["low_normal"];
                        labResult.high_normal = (string)reader["high_normal"];
                        labResult.lab_type = (string)reader["lab_type"];
                        labResult.lab_code = (string)reader["lab_code"];
                        labResult.remark = (string)reader["remark"];

                        labResults.Add(labResult);
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }

            return labResults;
        }

        public List<LabCompare> GetLabCompare(string hn, string regNo, string req_no)
        {

            string SQLCommand = @"CREATE TABLE [dbo].[#tempLabShow](
	                                [labType] [varchar](6) COLLATE Thai_BIN NULL,
	                                [labName] [varchar](100) COLLATE Thai_BIN NULL,
	                                [req_no1] [varchar](8) COLLATE Thai_BIN NULL,
	                                [data1] [varchar](1000) COLLATE Thai_BIN NULL,
	                                [req_no2] [varchar](8) COLLATE Thai_BIN NULL,
	                                [data2] [varchar](1000) COLLATE Thai_BIN NULL,
	                                [req_no3] [varchar](8) COLLATE Thai_BIN NULL,
	                                [data3] [varchar](1000) COLLATE Thai_BIN NULL,
	                                [req_no4] [varchar](8) COLLATE Thai_BIN NULL,
	                                [data4] [varchar](1000) COLLATE Thai_BIN NULL,
	                                [req_no5] [varchar](8) COLLATE Thai_BIN NULL,
	                                [data5] [varchar](1000) COLLATE Thai_BIN NULL,
                                )
                                insert into #tempLabShow(labType,labName,req_no1,data1)
                                SELECT Labres_d.lab_type,Labres_d.result_name,Labres_d.req_no
                                --,Labres_d.real_res 
                                ,isnull(
                                CASE 
                                WHEN Labres_d.lab_code IN ('SE012 ','SE013 ','SE014 ','SL110 ','SUP02 ','BLOD11') AND (Labres_d.lab_type = 'SE' OR Labres_d.lab_type ='BB'  OR Labres_d.lab_type ='SL' OR Labres_d.lab_type ='SUP') THEN '*รับผลที่ห้องแลป*'
                                WHEN  Labres_d.real_res='' AND Labres_d.lab_code NOT IN ('SE012 ','SE013 ','SE014 ','SL110 ','SUP02 ','BLOD11') THEN (SELECT top 1 resText FROM Labres_m WHERE Labres_m.reqNo=Labres_d.req_no AND Labres_m.resultName=Labres_d.result_name) 
                                ELSE real_res END
                                ,'')as real_res  
                                FROM Labres_d (NOLOCK)  
                                LEFT JOIN Labre_s s(nolock) on Labres_d.lab_code=s.lab_code   AND Labres_d.res_item=s.res_run_no
                                WHERE  (hn = @hn) AND (reg_flag = @regNo)  
                                AND (req_no = @req_no ) order by s.printSeq

                                update #tempLabShow  set req_no2=(SELECT top 1
                                Labres_d.req_no FROM Labres_d (NOLOCK)  
                                WHERE  (hn = @hn) AND (Labres_d.lab_type=#tempLabShow.labType)  
                                AND (req_no < #tempLabShow.req_no1) ORDER by req_no DESC)


                                update #tempLabShow  set data2=(SELECT  
                                --Labres_d.real_res 
                                isnull(
                                CASE 
                                WHEN Labres_d.lab_code IN ('SE012 ','SE013 ','SE014 ','SL110 ','SUP02 ','BLOD11') AND (Labres_d.lab_type = 'SE' OR Labres_d.lab_type ='BB'  OR Labres_d.lab_type ='SL' OR Labres_d.lab_type ='SUP') THEN '*รับผลที่ห้องแลป*'
                                WHEN  Labres_d.real_res='' AND Labres_d.lab_code NOT IN ('SE012 ','SE013 ','SE014 ','SL110 ','SUP02 ','BLOD11') THEN (SELECT top 1 resText FROM Labres_m WHERE Labres_m.reqNo=Labres_d.req_no AND Labres_m.resultName=Labres_d.result_name) 
                                ELSE real_res END
                                ,'')as real_res  
                                FROM Labres_d (NOLOCK)  
                                WHERE  (hn =  @hn) AND (Labres_d.lab_type=#tempLabShow.labType)  
                                AND (req_no =#tempLabShow.req_no2) and result_name= #tempLabShow.labName)



                                update #tempLabShow  set req_no3=(SELECT top 1
                                Labres_d.req_no FROM Labres_d (NOLOCK)  
                                WHERE  (hn = @hn) AND (Labres_d.lab_type=#tempLabShow.labType)  
                                AND (req_no < #tempLabShow.req_no2) ORDER by req_no DESC)


                                update #tempLabShow  set data3=(SELECT  
                                --Labres_d.real_res 
                                isnull(
                                CASE 
                                WHEN Labres_d.lab_code IN ('SE012 ','SE013 ','SE014 ','SL110 ','SUP02 ','BLOD11') AND (Labres_d.lab_type = 'SE' OR Labres_d.lab_type ='BB'  OR Labres_d.lab_type ='SL' OR Labres_d.lab_type ='SUP') THEN '*รับผลที่ห้องแลป*'
                                WHEN  Labres_d.real_res='' AND Labres_d.lab_code NOT IN ('SE012 ','SE013 ','SE014 ','SL110 ','SUP02 ','BLOD11') THEN (SELECT top 1 resText FROM Labres_m WHERE Labres_m.reqNo=Labres_d.req_no AND Labres_m.resultName=Labres_d.result_name) 
                                ELSE real_res END
                                ,'')as real_res  
                                FROM Labres_d (NOLOCK)  
                                WHERE  (hn =  @hn) AND (Labres_d.lab_type=#tempLabShow.labType)  
                                AND (req_no =#tempLabShow.req_no3) and result_name= #tempLabShow.labName)


                                update #tempLabShow  set req_no4=(SELECT top 1
                                Labres_d.req_no FROM Labres_d (NOLOCK)  
                                WHERE  (hn =  @hn) AND (Labres_d.lab_type=#tempLabShow.labType)  
                                AND (req_no < #tempLabShow.req_no3) ORDER by req_no DESC)


                                update #tempLabShow  set data4=(SELECT  
                                --Labres_d.real_res
                                isnull(
                                CASE 
                                WHEN Labres_d.lab_code IN ('SE012 ','SE013 ','SE014 ','SL110 ','SUP02 ','BLOD11') AND (Labres_d.lab_type = 'SE' OR Labres_d.lab_type ='BB'  OR Labres_d.lab_type ='SL' OR Labres_d.lab_type ='SUP') THEN '*รับผลที่ห้องแลป*'
                                WHEN  Labres_d.real_res='' AND Labres_d.lab_code NOT IN ('SE012 ','SE013 ','SE014 ','SL110 ','SUP02 ','BLOD11') THEN (SELECT top 1 resText FROM Labres_m WHERE Labres_m.reqNo=Labres_d.req_no AND Labres_m.resultName=Labres_d.result_name) 
                                ELSE real_res END
                                ,'')as real_res  
                                FROM Labres_d (NOLOCK)  
                                WHERE  (hn = @hn) AND (Labres_d.lab_type=#tempLabShow.labType)  
                                AND (req_no =#tempLabShow.req_no4) and result_name= #tempLabShow.labName)


                                update #tempLabShow  set req_no5=(SELECT top 1
                                Labres_d.req_no FROM Labres_d (NOLOCK)  
                                WHERE  (hn =  @hn) AND (Labres_d.lab_type=#tempLabShow.labType)  
                                AND (req_no < #tempLabShow.req_no4) ORDER by req_no DESC)


                                update #tempLabShow  set data5=(SELECT  
                                --Labres_d.real_res 
                                isnull(
                                CASE 
                                WHEN Labres_d.lab_code IN ('SE012 ','SE013 ','SE014 ','SL110 ','SUP02 ','BLOD11') AND (Labres_d.lab_type = 'SE' OR Labres_d.lab_type ='BB'  OR Labres_d.lab_type ='SL' OR Labres_d.lab_type ='SUP') THEN '*รับผลที่ห้องแลป*'
                                WHEN  Labres_d.real_res='' AND Labres_d.lab_code NOT IN ('SE012 ','SE013 ','SE014 ','SL110 ','SUP02 ','BLOD11') THEN (SELECT top 1 resText FROM Labres_m WHERE Labres_m.reqNo=Labres_d.req_no AND Labres_m.resultName=Labres_d.result_name) 
                                ELSE real_res END
                                ,'')as real_res  
                                FROM Labres_d (NOLOCK)  
                                WHERE  (hn =  @hn ) AND (Labres_d.lab_type=#tempLabShow.labType)  
                                AND (req_no =#tempLabShow.req_no5) and result_name= #tempLabShow.labName)


                                select 'วันที่ตรวจ' as labName
                                ,isnull((select  RIGHT(Labreq_h.req_date,2)+'/'+substring(Labreq_h.req_date,5,2) +'/'+left(Labreq_h.req_date,4) 
                                from Labreq_h(NOLOCK) where req_no=tLab.req_no1),'') as data1
                                ,isnull((select  RIGHT(Labreq_h.req_date,2)+'/'+substring(Labreq_h.req_date,5,2) +'/'+left(Labreq_h.req_date,4) 
                                from Labreq_h(NOLOCK) where req_no=tLab.req_no2),'') as data2
                                ,isnull((select  RIGHT(Labreq_h.req_date,2)+'/'+substring(Labreq_h.req_date,5,2) +'/'+left(Labreq_h.req_date,4) 
                                from Labreq_h(NOLOCK) where req_no=tLab.req_no3),'') as data3
                                ,isnull((select  RIGHT(Labreq_h.req_date,2)+'/'+substring(Labreq_h.req_date,5,2) +'/'+left(Labreq_h.req_date,4) 
                                from Labreq_h(NOLOCK) where req_no=tLab.req_no4),'')as data4
                                ,isnull((select  RIGHT(Labreq_h.req_date,2)+'/'+substring(Labreq_h.req_date,5,2) +'/'+left(Labreq_h.req_date,4) 
                                from Labreq_h(NOLOCK) where req_no=tLab.req_no5),'') as data5  FROM
                                (select top 1 req_no1,req_no2,req_no3,req_no4,req_no5 
                                from  #tempLabShow) as tLab
                                union ALL
                                select 
                                isnull(labName,'') as labName
                                ,isnull(data1,'') as data1
                                ,isnull(data2,'') as data2
                                ,isnull(data3,'') as data3
                                ,isnull(data4,'') as data4
                                ,isnull(data5,'') as data5 
                                from #tempLabShow

                                DROP table #tempLabShow ";


            List<LabCompare> labCompares = new List<LabCompare>();
            using (var db = new DBHelper(GlobalVar.connectionString))
            {
                try
                {
                    db.AddParameter("@hn", hn);
                    db.AddParameter("@regNo", regNo);
                    db.AddParameter("@req_no", req_no);

                    var reader = db.ExecuteReader(SQLCommand);


                    while (reader.Read())
                    {
                        LabCompare labCompare = new LabCompare();
                        labCompare.labName = (string)reader["labName"];
                        labCompare.data1 = (string)reader["data1"];
                        labCompare.data2 = (string)reader["data2"];
                        labCompare.data3 = (string)reader["data3"];
                        labCompare.data4 = (string)reader["data4"];
                        labCompare.data5 = (string)reader["data5"];

                        labCompares.Add(labCompare);
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }

            return labCompares;
        }
    }
}