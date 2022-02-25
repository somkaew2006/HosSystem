using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HosSystem.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Services.Protocols;
using WebGrease;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace HosSystem.Repository
{
    public class UserRepo
    {
        //public static async ipdEmployee Login(string ward, string userId, string password)
        //{
        //    ipdEmployee ipdEmployee;
        //    using (var client = new HttpClient())
        //    {
        //        client.BaseAddress = new Uri("https://localhost:44390/api/");
        //        client.DefaultRequestHeaders.Accept.Clear();
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        //GET Method  
        //        HttpResponseMessage response = await client.GetAsync("api/Department/1");
        //        if (response.IsSuccessStatusCode)
        //        {
        //            //Departmentdepartment = awaitresponse.Content.ReadAsAsync<Department>();
        //            //Console.WriteLine("Id:{0}\tName:{1}", department.DepartmentId, department.DepartmentName);
        //            //Console.WriteLine("No of Employee in Department: {0}", department.Employees.Count);
                    
        //            string Result = await response.Content.ReadAsStringAsync();
        //            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
        //              ipdEmployee = JsonConvert.DeserializeObject<ipdEmployee>(Result);

        //            return ipdEmployee; 
        //        }
        //        else
        //        {
        //            //Console.WriteLine("Internal server Error");
        //            return null;
        //        }
        //        return ipdEmployee;
        //    }
        //}
    }
}