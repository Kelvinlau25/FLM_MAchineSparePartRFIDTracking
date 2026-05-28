using System;
using System.Web.Mvc;
using FILM_Sparepart_MVC.Services;

namespace FILM_Sparepart_MVC.Controllers
{
    public class RfidReaderController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult LoadReaders()
        {
            try
            {
                var service = new RFIDService();
                var readers = service.LoadReaderConfigurations();
                return Json(new { success = true, data = readers });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult ConnectReader(string readerIP)
        {
            try
            {
                var service = new RFIDService();
                var result = service.ConnectReader(readerIP);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DisconnectReader(string readerIP)
        {
            try
            {
                var service = new RFIDService();
                var result = service.DisconnectReader(readerIP);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult ConnectAll()
        {
            try
            {
                var service = new RFIDService();
                var results = service.ConnectAll();
                return Json(results);
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DisconnectAll()
        {
            try
            {
                var service = new RFIDService();
                var results = service.DisconnectAll();
                return Json(results);
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }
    }
}
