using DatabaseHelper;
using HosSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HosSystem.Repository
{
    public class PhysicianRepo
    {
        public Physician CheckLoin(string userId, string password)
        {

            string SQLCommand = @"select docCode,docName,docLName,isnull(docDept,'') as docDept,doctitle from DOCC where ltrim(docCode) =@UserID and docPwdNew =@Password";

            Physician physician = new Physician();
            using (var db = new DBHelper(GlobalVar.connectionString))
            {
                try
                {
                    db.AddParameter("@UserID", userId);
                    db.AddParameter("@Password", password);
                    var reader = db.ExecuteReader(SQLCommand);


                    if (reader.Read())
                    {
                        //  physician = new Profile();
                        physician.DocCode = (string)reader["docCode"];
                        physician.DocLName = (string)reader["docName"];
                        physician.DocLName = (string)reader["docLName"];
                        physician.DocTitle = (string)reader["docTitle"];
                        physician.DocDept = (string)reader["docDept"];

                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }

            return physician;
        }
    }
}