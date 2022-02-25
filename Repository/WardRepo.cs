using DatabaseHelper;
using HosSystem.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc.Routing;

namespace HosSystem.Repository
{
    public class WardRepo
    {
        public List<DropDownList> GetWard()
        {

            string SQLCommand = @"select  rtrim(ward_id) as ward_id,rtrim(ward_name) as ward_name from [dbo].[Ward]  where isnull(ward_tot_bed,0)>0  order by ward_name";

            List<DropDownList> dropDownLists = new List<DropDownList>();
            using (var db = new DBHelper(GlobalVar.connectionString))
            {
                try
                {
                    //db.AddParameter("@UserID", userId);
                    //db.AddParameter("@Password", password);
                    var reader = db.ExecuteReader(SQLCommand);


                    while (reader.Read())
                    {
                        DropDownList dropDownList = new DropDownList();
                        dropDownList.Code =(string)reader["ward_id"];
                        dropDownList.Value =(string)reader["ward_name"];
                        dropDownLists.Add(dropDownList);
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }

            return dropDownLists;
        }
    }
}