using DatabaseHelper;
using HosSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HosSystem.Repository
{
    public class IpdRepo
    {
        public List<Patient> GetPartial(string WardID)
        {

            string SQLCommand = @"select   isnull(ipd.current_bed,'zz') as bed,  ladmit_n as AN,pat.hn as HN,pat.firstName +' ' + lastName as codename,regist_flag,age,ward_id,current_room  
                                    from [dbo].[Ipd_h] ipd left join [dbo].[PATIENT] pat on ipd.hn = pat.hn
                                    where  ladmit_n<>'NULL' and  rtrim(ward_id) =@WardID   and  ( discharge_date ='' or discharge_date is null) and regist_flag <>''
                                    order by bed ";
         


            List<Patient> patients = new List<Patient>();
            using (var db = new DBHelper(GlobalVar.connectionString))
            {
                try
                {
                    db.AddParameter("@WardID", WardID);
                    var reader = db.ExecuteReader(SQLCommand);


                    while (reader.Read())
                    {
                        Patient patient = new Patient();
                        patient.HN = (string)reader["HN"];
                        patient.AN = (string)reader["AN"];
                        patient.RegNo = (string)reader["regist_flag"];
                        patient.Name = (string)reader["codename"];
                        patient.Age = (string)reader["Age"];
                        patient.Ward = WardID;
                        patient.Room = (string)reader["current_room"];
                        patient.Bed = (string)reader["bed"];

                        patients.Add(patient);
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }

            return patients;
        }

        public Patient GetPartial(string hn,string regNo)
        {

            string SQLCommand = @"select   isnull(ipd.current_bed,'zz') as bed,  ladmit_n as AN,pat.hn as HN,pat.firstName +' ' + lastName as codename,regist_flag,age,ward_id,current_room  
                                    from [dbo].[Ipd_h] ipd left join [dbo].[PATIENT] pat on ipd.hn = pat.hn
                                    where  ipd.hn=@HN and  regist_flag=@RegNo ";
          


            Patient patient = new Patient();
            using (var db = new DBHelper(GlobalVar.connectionString))
            {
                try
                {
                    db.AddParameter("@HN", hn);
                    db.AddParameter("@RegNo", regNo);
                    var reader = db.ExecuteReader(SQLCommand);

                    if (reader.Read())
                    {
                       
                        patient.HN = (string)reader["HN"];
                        patient.AN = (string)reader["AN"];
                        patient.RegNo = (string)reader["regist_flag"];
                        patient.Name = (string)reader["codename"];
                        patient.Age = (string)reader["Age"];
                        patient.Room = (string)reader["current_room"];
                        patient.Bed = (string)reader["bed"];

                        
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }

            return patient;
        }

        public Patient GetPartialByHN(string hn)
        {

            string SQLCommand = @"select  isnull(ipd.current_bed,'zz') as bed,  ladmit_n as AN,pat.hn as HN,pat.firstName +' ' + lastName as codename,regist_flag,age,ward_id,current_room  
                                    from [dbo].[Ipd_h] ipd left join [dbo].[PATIENT] pat on ipd.hn = pat.hn
                                     where  ltrim(ipd.hn)=@HN and ladmit_n<>'NULL'     and  ( discharge_date ='' or discharge_date is null)  ";
          

            Patient patient = new Patient();
            using (var db = new DBHelper(GlobalVar.connectionString))
            {
                try
                {
                    db.AddParameter("@HN", hn);
                    var reader = db.ExecuteReader(SQLCommand);

                    if (reader.Read())
                    {

                        patient.HN = (string)reader["HN"];
                        patient.AN = (string)reader["AN"];
                        patient.RegNo = (string)reader["regist_flag"];
                        patient.Name = (string)reader["codename"];
                        patient.Age = (string)reader["Age"];
                        patient.Room = (string)reader["current_room"];
                        patient.Bed = (string)reader["bed"];


                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }

            return patient;
        }


        public Result CreateVitalsign(ipdVitalSign ipdVitalSign)
        {
            using (var context = new PTLEntities())
            {
                Result result = new Result();
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        // var model = new Personnel();
                        int maxSeq = 0;
                        if (context.ipdVitalSigns.Where(c => c.AN == ipdVitalSign.AN).Count() != 0)
                        {
                            maxSeq = Convert.ToInt32(context.ipdVitalSigns.Where(c => c.AN == ipdVitalSign.AN).Max(c => c.Seq));
                        }
                        maxSeq++;
                        //string pid = maxPid.ToString().PadLeft(4, '0');

                        ipdVitalSign.Seq = maxSeq;
                        //model.Pname = personnel.Pname;
                        //model.Puser = personnel.Puser;
                        //model.Ppass = personnel.Ppass;

                        context.ipdVitalSigns.Add(ipdVitalSign);
                        context.SaveChanges();
                        trans.Commit();

                        result.Flag = true;
                        return result;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        result.Flag = false;
                        result.Message = ex.ToString();
                        return result;
                    }

                }
            }
        }
        public Result CreateTemperature(ipdTemperature ipdTemperature)
        {
            using (var context = new PTLEntities())
            {
                Result result = new Result();
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        // var model = new Personnel();
                        int maxSeq = 0;
                        if (context.ipdTemperatures.Where(c => c.AN == ipdTemperature.AN).Count() != 0)
                        {
                            maxSeq = Convert.ToInt32(context.ipdTemperatures.Where(c => c.AN == ipdTemperature.AN).Max(c => c.Seq));
                        }
                        maxSeq++;
                        //string pid = maxPid.ToString().PadLeft(4, '0');

                        ipdTemperature.Seq = maxSeq;
                        //model.Pname = personnel.Pname;
                        //model.Puser = personnel.Puser;
                        //model.Ppass = personnel.Ppass;

                        context.ipdTemperatures.Add(ipdTemperature);
                        context.SaveChanges();
                        trans.Commit();

                        result.Flag = true;
                        return result;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        result.Flag = false;
                        result.Message = ex.ToString();
                        return result;
                    }

                }
            }
        }
        public Result CreateBody(ipdBody ipdBody)
        {
            using (var context = new PTLEntities())
            {
                Result result = new Result();
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        // var model = new Personnel();
                        int maxSeq = 0;
                        if (context.ipdBodies.Where(c => c.AN == ipdBody.AN).Count() != 0)
                        {
                            maxSeq = Convert.ToInt32(context.ipdBodies.Where(c => c.AN == ipdBody.AN).Max(c => c.Seq));
                        }
                        maxSeq++;
                        //string pid = maxPid.ToString().PadLeft(4, '0');

                        ipdBody.Seq = maxSeq;


                        context.ipdBodies.Add(ipdBody);
                        context.SaveChanges();
                        trans.Commit();

                        result.Flag = true;
                        return result;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        result.Flag = false;
                        result.Message = ex.ToString();
                        return result;
                    }

                }
            }
        }
        public Result CreatePain(ipdPain ipdPain)
        {
            using (var context = new PTLEntities())
            {
                Result result = new Result();
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        // var model = new Personnel();
                        int maxSeq = 0;
                        if (context.ipdPains.Where(c => c.AN == ipdPain.AN).Count() != 0)
                        {
                            maxSeq = Convert.ToInt32(context.ipdPains.Where(c => c.AN == ipdPain.AN).Max(c => c.Seq));
                        }
                        maxSeq++;
                        //string pid = maxPid.ToString().PadLeft(4, '0');

                        ipdPain.Seq = maxSeq;
                        //model.Pname = personnel.Pname;
                        //model.Puser = personnel.Puser;
                        //model.Ppass = personnel.Ppass;

                        context.ipdPains.Add(ipdPain);
                        context.SaveChanges();
                        trans.Commit();

                        result.Flag = true;
                        return result;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        result.Flag = false;
                        result.Message = ex.ToString();
                        return result;
                    }

                }
            }
        }
        public Result CreateFluid(ipdFluid ipdFluide)
        {
            using (var context = new PTLEntities())
            {
                Result result = new Result();
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        // var model = new Personnel();
                        int maxSeq = 0;
                        if (context.ipdFluids.Where(c => c.AN == ipdFluide.AN).Count() != 0)
                        {
                            maxSeq = Convert.ToInt32(context.ipdFluids.Where(c => c.AN == ipdFluide.AN).Max(c => c.Seq));
                        }
                        maxSeq++;
                        //string pid = maxPid.ToString().PadLeft(4, '0');

                        ipdFluide.Seq = maxSeq;
                        //model.Pname = personnel.Pname;
                        //model.Puser = personnel.Puser;
                        //model.Ppass = personnel.Ppass;

                        context.ipdFluids.Add(ipdFluide);
                        context.SaveChanges();
                        trans.Commit();

                        result.Flag = true;
                        return result;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        result.Flag = false;
                        result.Message = ex.ToString();
                        return result;
                    }

                }
            }
        }
        public Result CreateOther(ipdOther ipdOther)
        {
            using (var context = new PTLEntities())
            {
                Result result = new Result();
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        // var model = new Personnel();
                        int maxSeq = 0;
                        if (context.ipdOthers.Where(c => c.AN == ipdOther.AN).Count() != 0)
                        {
                            maxSeq = Convert.ToInt32(context.ipdOthers.Where(c => c.AN == ipdOther.AN).Max(c => c.Seq));
                        }
                        maxSeq++;
                        //string pid = maxPid.ToString().PadLeft(4, '0');

                        ipdOther.Seq = maxSeq;
                        //model.Pname = personnel.Pname;
                        //model.Puser = personnel.Puser;
                        //model.Ppass = personnel.Ppass;

                        context.ipdOthers.Add(ipdOther);
                        context.SaveChanges();
                        trans.Commit();

                        result.Flag = true;
                        return result;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        result.Flag = false;
                        result.Message = ex.ToString();
                        return result;
                    }

                }
            }
        }


    }
}