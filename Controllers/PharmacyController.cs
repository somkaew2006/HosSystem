using HosSystem.Models;
using HosSystem.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HosSystem.Controllers
{
    public class PharmacyController : Controller
    {
        // GET: Pharmacy
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CheckLogin(string Ward, string UserId, string Password)
        {
            Profile profile = new ProfileRepo().CheckLoin(UserId, Password);
            if (profile.UserID != null)
            {
                HttpContext.Session["UserId"] = profile.UserID;
                HttpContext.Session["UserName"] = profile.UserName;
              //  HttpContext.Session["Ward"] = Ward;

                return RedirectToAction("ViewPharmacy");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult ViewPharmacy(string Search)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login");
            }
            ViewBag.Search = Search;

            ViewPharmacy viewPharmacy = new ViewPharmacy();

            viewPharmacy.pTLOrderTopTen = new OrderRepo().GetTopTenOrder();
            if (Search !=null)
            {
                viewPharmacy.pTLOrderInfo = new OrderRepo().GetOrderSearch(Search);
            }

            return View(viewPharmacy); 
        }

        [HttpPost]
        public JsonResult AcceptOrder(string OrderID)
        {

            string UserID = (string)Session["UserId"];
            Result result = new OrderRepo().AcceptOrder(OrderID, UserID);

            return Json(result);
        }

        public ActionResult ViewConsult(string HN)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login");
            }
            string UserID = (string)Session["UserId"];

            ViewData viewData = new ViewData();
            viewData.UserID = UserID;

            viewData.patient = new Patient();
            
            if (HN!=null)
            {
                viewData.patient = new IpdRepo().GetPartialByHN(HN);
            }
            
             
            return View(viewData);
        }

         [HttpPost]
        public JsonResult CreateConsult(string AN,string UserID,string cs_header ,string  cs_detail)
        {

            //string UserID = (string)Session["UserId"];
            Result result = new ConsultRepo().CreateConsult(AN, UserID, cs_header, cs_detail);

            return Json(result);
        }
    }
}