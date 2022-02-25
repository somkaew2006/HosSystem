using HosSystem.Models;
using HosSystem.Repository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace HosSystem.Controllers
{
    public class WardController : Controller
    {
        // GET: Ward
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

         
        public JsonResult GetWard()
        {
            List<DropDownList> dropDownLists = new WardRepo().GetWard();
            return Json(dropDownLists, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CheckLogin(string Ward, string UserId, string Password)
        {
            Profile profile = new ProfileRepo().CheckLoin(UserId, Password);
            if (profile.UserID != null)
            {
                HttpContext.Session["UserId"] = profile.UserID;
                HttpContext.Session["UserName"] = profile.UserName;
                HttpContext.Session["Ward"] = Ward;

                return RedirectToAction("Create");
            }
            else
            {
                return RedirectToAction("Login");
            }


           

        }
        public ActionResult ListPatient()
        {
            return View();
        }

        public ActionResult Create(string HN)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login");
            }

            ViewBag.HN = HN;
            string ward = (string)Session["Ward"];
            List<Patient> patients = new IpdRepo().GetPartial(ward);

            return View(patients);

           


        }




        [HttpPost]
        public JsonResult CreateVitalsign(ipdVitalSign ipdVitalSign)
        {
            //if (Session["UserId"] == null)
            //{
            //    Result result =new r
            //}
            ipdVitalSign.UserID = (string)Session["UserID"];
            ipdVitalSign.UserName = (string)Session["UserName"];
            ipdVitalSign.CreateOn = DateTime.Now;

            Result result = new IpdRepo().CreateVitalsign(ipdVitalSign);

            return Json(result);
        }

        [HttpPost]
        public JsonResult CreateTemperature(ipdTemperature ipdTemperature)
        {
            //if (Session["UserId"] == null)
            //{
            //    return RedirectToAction("Login");
            //}
            ipdTemperature.UserID = (string)Session["UserID"];
            ipdTemperature.UserName = (string)Session["UserName"];
            ipdTemperature.CreateOn = DateTime.Now;

            Result result = new IpdRepo().CreateTemperature(ipdTemperature);

            return Json(result);
        }

        [HttpPost]
        public JsonResult CreateBody(ipdBody ipdBody)
        {
            //if (Session["UserId"] == null)
            //{
            //    return RedirectToAction("Login");
            //}
            ipdBody.UserID = (string)Session["UserID"];
            ipdBody.UserName = (string)Session["UserName"];
            ipdBody.CreateOn = DateTime.Now;

            Result result = new IpdRepo().CreateBody(ipdBody);

            return Json(result);
        }

        [HttpPost]
        public JsonResult CreatePain(ipdPain ipdPain)
        {
            //if (Session["UserId"] == null)
            //{
            //    return RedirectToAction("Login");
            //}
            ipdPain.UserID = (string)Session["UserID"];
            ipdPain.UserName = (string)Session["UserName"];
            ipdPain.CreateOn = DateTime.Now;

            Result result = new IpdRepo().CreatePain(ipdPain);

            return Json(result);
        }

        [HttpPost]
        public JsonResult CreateFluid(ipdFluid ipdFluid)
        {
            //if (Session["UserId"] == null)
            //{
            //    return RedirectToAction("Login");
            //}
            ipdFluid.UserID = (string)Session["UserID"];
            ipdFluid.UserName = (string)Session["UserName"];
            ipdFluid.CreateOn = DateTime.Now;

            Result result = new IpdRepo().CreateFluid(ipdFluid);

            return Json(result);
        }
        [HttpPost]
        public JsonResult CreateOther(ipdOther ipdOther)
        {
            //if (Session["UserId"] == null)
            //{
            //    return RedirectToAction("Login");
            //}
            ipdOther.UserID = (string)Session["UserID"];
            ipdOther.UserName = (string)Session["UserName"];
            ipdOther.CreateOn = DateTime.Now;

            Result result = new IpdRepo().CreateOther(ipdOther);

            return Json(result);
        }

        public JsonResult GetTemperature(string AN)
        {
            PTLEntities context = new PTLEntities();
            var ipdTemperatures = context.ipdTemperatures.OrderByDescending(c => c.Seq).Where(c => c.AN == AN).Take(20).Select(c => new
            {
                Seq = c.Seq,
                Value = c.Temperature
            }).OrderBy(c => c.Seq).ToList();

            return Json(ipdTemperatures, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPain(string AN)
        {
            PTLEntities context = new PTLEntities();
            var ipdPains = context.ipdPains.OrderByDescending(c => c.Seq).Where(c => c.AN == AN).Take(20).Select(c => new
            {
                Seq = c.Seq,
                Value = c.Pain
            }).OrderBy(c => c.Seq).ToList();

            return Json(ipdPains, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetWeight(string AN)
        {
            PTLEntities context = new PTLEntities();
            var ipdBodies = context.ipdBodies.OrderByDescending(c => c.Seq).Where(c => c.AN == AN).Take(20).Select(c => new
            {
                Seq = c.Seq,
                Value = c.Weight
            }).OrderBy(c => c.Seq).ToList();

            return Json(ipdBodies, JsonRequestBehavior.AllowGet);
        }
       
        public JsonResult GetVitalSign(string AN)
        {
            PTLEntities context = new PTLEntities();


            var ipdVitalSign = context.ipdVitalSigns.OrderByDescending(c => c.Seq).Where(c => c.AN == AN).Take(20).Select(c => new
            {
                Seq = c.Seq,
                ValueSystolic = c.Systolic,
                ValueDiastotic = c.Diastotic,
                ValMAP = c.MAP,
                ValO2Sat=c.O2Sat,
                ValO2SatPreduct =c.O2SatPreduct,
                ValO2SatPostduct= c.O2SatPostduct,
                ValPlus =c.Plus,
                ValRespirations =c.Respirations
            }).OrderBy(c => c.Seq).ToList();

            return Json(ipdVitalSign, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFluid(string AN)
        {
            PTLEntities context = new PTLEntities();
            var ipdFluids = context.ipdFluids.OrderByDescending(c => c.Seq).Where(c => c.AN == AN).Take(20).Select(c => new
            {
                Seq = c.Seq,
                ValOralFluid = c.OralFluid,
                ValParenteral= c.Parenteral,
                ValUnine =c.Unine,
                ValEmesis =c.Emesis,
                ValDrainage =c.Drainage,
                ValAspiration =c.Aspiration,
                ValStools =c.Stools,
                ValUrine =c.Urine
            }).OrderBy(c => c.Seq).ToList();

            return Json(ipdFluids, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOthers(string AN)
        {
            PTLEntities context = new PTLEntities();
            var ipdBodies = context.ipdOthers.OrderByDescending(c => c.Seq).Where(c => c.AN == AN).Take(20).Select(c => new
            {
                Seq = c.Seq,
                ValHF = c.HF,
                ValEpisiotomy = c.Episiotomy,
                ValLochia =c.Lochia ,
                ValSOS =c.SOS
            }).OrderBy(c => c.Seq).ToList();

            return Json(ipdBodies, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }
        public ActionResult ViewAcceptOrder(string Search)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login");
            }

            ViewBag.Search = Search;

            
          

            ViewPharmacy viewPharmacy = new ViewPharmacy();

            viewPharmacy.WardID = (string)Session["Ward"]; 
            viewPharmacy.pTLOrderTopTen = new OrderRepo().GetTopTenOrder(viewPharmacy.WardID);
            if (Search != null)
            {
                viewPharmacy.pTLOrderInfo = new OrderRepo().GetOrderSearch(Search, viewPharmacy.WardID);
            }
            return View(viewPharmacy);
        }

       
        [HttpPost]
        public JsonResult AcceptOrder(string OrderID)
        {

            string UserID = (string)Session["UserId"];
            Result result = new OrderRepo().AcceptOrderByNures(OrderID, UserID);

            return Json(result);
        }
    }
}