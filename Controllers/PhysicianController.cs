using HosSystem.Models;
using HosSystem.Repository;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Web;
using System.Web.Mvc;

namespace HosSystem.Controllers
{
    public class PhysicianController : Controller
    {
        // GET: Physician
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

            Physician physician = new PhysicianRepo().CheckLoin(UserId, Password);
            if (physician.DocCode != null)
            {
                HttpContext.Session["UserId"] = physician.DocCode;
                HttpContext.Session["UserName"] = physician.FullName;
                HttpContext.Session["Ward"] = Ward;

                return RedirectToAction("CheckConsultStatus");
            }
            else
            {
                return RedirectToAction("Login");
            }


        }
        public ActionResult CheckConsultStatus(string HN, string RegNo, string AN)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login");
            }

            //มาจากหน้า login

            if (AN == null)
            {
                string ward = (string)Session["Ward"];

                Patient patient = new IpdRepo().GetPartial(ward).FirstOrDefault();

                if (patient==null)
                {
                    return RedirectToAction("ViewEmpty", "Physician");
                }
                HN = patient.HN;
                RegNo = patient.RegNo;
                AN = patient.AN;

                //HN = " 246423";
                //RegNo = "09";
                //AN = "6401489";


                bool IsConsultStausN = new ConsultRepo().IsConsultStatusN(AN);

                if (IsConsultStausN)
                {
                    //มี consult
                    return RedirectToAction("ViewData", "Physician", new { HN = HN, RegNo = RegNo, AN = AN });
                }
                else
                {
                    // เข้ามาหน้านี้เหมือน ยังไม่มีข้อมูลใน table consult กับ status ='A' ใน table consult
                    return RedirectToAction("ViewSummaryOrder", "Physician", new { HN = HN, RegNo = RegNo, AN = AN });
                }
            }
            // มาจากหน้า ViewData or ViewSummaryOrder
            else
            {
                // viewbag ตัวตอบบอกว่าไม่ได้มาจากหน้า login

                bool IsConsultStausN = new ConsultRepo().IsConsultStatusN(AN);
                if (IsConsultStausN)
                {
                    return RedirectToAction("ViewData", "Physician", new { HN = HN, RegNo = RegNo, AN = AN });
                }
                else
                {
                    // เข้ามาหน้านี้เหมือน ยังไม่มีข้อมูลใน table consult กับ status ='A' ใน table consult
                    return RedirectToAction("ViewSummaryOrder", "Physician", new { HN = HN, RegNo = RegNo, AN = AN });
                }

            }
        }

        public ActionResult ViewEmpty()
        {
            return View();
        }


        // ยังไม่ได้ทำ consult
        public ActionResult ViewData(string HN, string RegNo, string AN)
        {

            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login");
            }

            ViewBag.HN = HN;
            ViewBag.RegNo = RegNo;
            ViewBag.AN = AN;
            string ward = (string)Session["Ward"];
            ViewData viewData = new ViewData();
            viewData.patients = new IpdRepo().GetPartial(ward);
            viewData.ptl_Consults = new ConsultRepo().GetConsults(AN);

            return View(viewData);

        }

        // ทำ consult
        public ActionResult ViewSummaryOrder(string HN, string RegNo, string AN)
        {

            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login");
            }
            //  AN = "6212854";
            ViewBag.AN = AN;
            ViewBag.HN = HN;
            string ward = (string)Session["Ward"];
            ViewSummaryOrder viewSummaryOrder = new ViewSummaryOrder();
            viewSummaryOrder.patients = new IpdRepo().GetPartial(ward);
            // viewSummaryOrder.viewOrders = new OrderRepo().GetOrder(AN,1);
            //viewSummaryOrder.ipd_NnoteDs = new Ipd_NnoteDRepo().GetNnoteD(HN, RegNo);

            return View(viewSummaryOrder);

        }

        [HttpGet]
        public JsonResult GetMoreViewOrder(string AN, int EndRow)
        {


            List<PTLOrder> viewOrders = new OrderRepo().GetOrder(AN, EndRow);

            return Json(viewOrders, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetMoreViewNurseNote(string HN, string RegNo, int EndRow)
        {


            List<Ipd_NnoteD> ipd_NnoteDs = new Ipd_NnoteDRepo().GetNnoteD(HN, RegNo, EndRow);
            return Json(ipd_NnoteDs, JsonRequestBehavior.AllowGet);
        }


        // เช็คว่ามี order ของหมอคนดังกล่างค้างอยู่หรือเปล่า ถ้าค้างจะสร้าง order ซ้ำไม่ได้
        [HttpGet]
        public JsonResult CheckOrderStatus(string HN, string AN, string RegNo)
        {


            string docID = (string)Session["UserId"];
            Result result = new OrderRepo().CheckOrderStatus(docID, HN, AN, RegNo);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //create order and  UpdateConsultStatus
        [HttpPost]
        public JsonResult CreateOrderAndUpdateConsult(string AN, string RegNo, string HN, string Comment)
        {


            ptl_consult consult = new ptl_consult();
            consult.cs_status = "A";
            consult.acc_doc = (string)Session["UserId"];
            consult.order_id = "";
            consult.acc_date = new ProfileRepo().GetCurrentDateStringYYYYMMDD();
            consult.acc_time = new ProfileRepo().GetCurrentDate();


            string DocID = (string)Session["UserId"];
            Result result = new OrderRepo().CreateOrderAndUpdateConsult(AN, RegNo, HN, DocID, Comment);

            return Json(result);
        }

        [HttpGet]
        public JsonResult CheckOrderOwner(string OrderID)
        {
            string DocID = (string)Session["UserId"];
            Result result = new OrderRepo().CheckOrderOwner(DocID,OrderID);

            return Json(result,JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteOrder(PTLOrder order)
        {
            //showFlag='N' เป็น การ Delete
            
            //string DocID = (string)Session["UserId"];
            Result result = new OrderRepo().UpdateOrderStatus(order.OrderID);

            return Json(result);
        }

        [HttpPost]
        public ActionResult Upload(string baseData)
        {
            string AN = HttpContext.Request.Form["AN"];
            string HN = HttpContext.Request.Form["HN"];
            string RegNo = HttpContext.Request.Form["RegNo"];

            string ProgessNote = HttpContext.Request.Form["ProgessNote"];
            string OrderOneDay = HttpContext.Request.Form["OrderOneDay"];
            string OrderContinue = HttpContext.Request.Form["OrderContinue"];


            string time = DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Millisecond + "_";
            string FileName = "";
            Result result = new Result();
            try
            {
                if (HttpContext.Request.Files.AllKeys.Any())
                {

                    for (int i = 0; i <= HttpContext.Request.Files.Count; i++)
                    {
                        var file = HttpContext.Request.Files["files" + i];

                        if (file != null)
                        {
                            var fileSavePath = Path.Combine(Server.MapPath("/HosSystem/Uploads"), time + file.FileName);
                            //var fileSavePath = Path.Combine(Server.MapPath("/Uploads"), time + file.FileName);
                            file.SaveAs(fileSavePath);

                            FileName = time + file.FileName;


                        }
                    }
                }


            }
            catch (Exception ex)
            {
                result.Flag = false;
                result.Message = "step upload file " + ex.ToString();

            }


            //save database 
            string DocID = (string)Session["UserId"];

            try
            {
                result = new OrderRepo().CreateOrder(AN, RegNo, HN, DocID, OrderContinue, OrderOneDay, ProgessNote, FileName);
            }
            catch (Exception ex)
            {

                result.Flag = false;
                result.Message = "step save database " + ex.ToString();
            }



            return Json(result);
        }




        public ActionResult ViewLab(string hn, string regNo)
        {

            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login");
            }


            ViewLab viewLab = new ViewLab();
            //hn = " 289471";
            //regNo = "81";

            viewLab.labMenuHeads = new LabRepo().GetLabMenu(hn, regNo);
            viewLab.patient = new IpdRepo().GetPartial(hn, regNo);
            return View(viewLab);
        }

        [HttpGet]
        public JsonResult GetLabResult(string HN, string RegNo, string req_no)
        {
            List<LabResult> labResults = new LabRepo().GetLabResult(HN, RegNo, req_no);

            return Json(labResults, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetLabCompare(string HN, string RegNo, string req_no)
        {
            List<LabCompare> labCompares = new LabRepo().GetLabCompare(HN, RegNo, req_no);

            return Json(labCompares, JsonRequestBehavior.AllowGet);
        }

        public FileResult DownloadFile(string fileName)
        {
            //var filepath = System.IO.Path.Combine(Server.MapPath("/Uploads/"), fileName);
            var filepath = System.IO.Path.Combine(Server.MapPath("/HosSystem/Uploads/"), fileName);
            return File(filepath, MimeMapping.GetMimeMapping(filepath), fileName);
        }
    }
}