using FILM_Sparepart_MVC.Models;
using FILM_Sparepart_MVC.Services;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace FILM_Sparepart_MVC.Controllers
{
    public class RfidReaderController : Controller
    {
        private readonly RFIDService _rfidService = new RFIDService();

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetConfig()
        {
            try
            {
                var configList = _rfidService.GetRFIDConfig();
                return Json(new { success = true, data = configList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetReaderStatuses()
        {
            try
            {
                var statuses = _rfidService.GetReaderStatuses();
                return Json(new { success = true, data = statuses }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
