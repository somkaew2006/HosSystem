using DatabaseHelper;
using HosSystem.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Profile;

namespace HosSystem.Repository
{
    public class ProfileRepo
    {
        public Profile CheckLoin(string userId, string password)
        {

            string SQLCommand = @"select UserCode,Password,Upassword,firstName,lastName,deptCode from profile where UserCode=@UserID and Upassword =@Password; ";

            Profile profile = new Profile();
            using (var db = new DBHelper(GlobalVar.connectionString))
            {
                try
                {
                    db.AddParameter("@UserID", userId);
                    db.AddParameter("@Password", password);
                    var reader = db.ExecuteReader(SQLCommand);


                    if (reader.Read())
                    {
                        //  profile = new Profile();
                        profile.UserID = (string)reader["UserCode"];
                        profile.UserName = (string)reader["firstName"] + " " + (string)reader["lastName"];
                        profile.deptCode = (string)reader["deptCode"];
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }

            return profile;
        }

        public DateTime GetCurrentDate()
        {

            string SQLCommand = @"select getdate() as CurrentDate ";

            
            DateTime currentDate = DateTime.MinValue;
            using (var db = new DBHelper(GlobalVar.connectionString))
            {
                try
                {
                    var reader = db.ExecuteReader(SQLCommand);


                    if (reader.Read())
                    {
                        //  profile = new Profile();
                        currentDate=(DateTime)reader["CurrentDate"];
                       
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }

            return currentDate;
         
        }

        //return YYYYMMDD แบบ พ.ศ
        public String GetCurrentDateStringYYYYMMDD()
        {

            string SQLCommand = @"SELECT  CAST(Year(getdate() )+543 as VARCHAR)  + RIGHT('0' + CAST(Month(getdate() ) AS VARCHAR), 2) +RIGHT('0' + CAST(Day(getdate() ) AS VARCHAR), 2) as CurrentDate  ";


            string currentDateString = "";
            using (var db = new DBHelper(GlobalVar.connectionString))
            {
                try
                {
                    var reader = db.ExecuteReader(SQLCommand);


                    if (reader.Read())
                    {
                        currentDateString = (String)reader["CurrentDate"];
                         
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }

            return currentDateString;

        }

    }//end class
}