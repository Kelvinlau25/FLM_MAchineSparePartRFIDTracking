using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using FILM_Sparepart_MVC.Services;
using Newtonsoft.Json;

namespace FILM_Sparepart_MVC.Controllers
{
    public class RfidReaderController : Controller
    {
        private readonly RFIDService _rfidService = RFIDService.Instance;

        /// <summary>
        /// Main RFID Reader Management page (replicates Form1.vb view)
        /// </summary>
        public ActionResult Index()
        {
            ViewBag.Title = "RFID Reader Management";
            return View();
        }

        /// <summary>
        /// Load RFID configuration from DB
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> LoadConfig()
        {
            var result = _rfidService.LoadConfiguration();
            return Json(new
            {
                success = result.Success,
                message = result.Message,
                readers = _rfidService.GetReaderList()
            });
        }

        /// <summary>
        /// Get current reader status list
        /// </summary>
        [HttpGet]
        public JsonResult GetReaders()
        {
            return Json(new
            {
                isInitialized = _rfidService.IsInitialized,
                readers = _rfidService.GetReaderList()
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Connect all readers
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> ConnectAll()
        {
            await _rfidService.ConnectAllAsync();
            return Json(new
            {
                success = true,
                message = "Connect all initiated.",
                readers = _rfidService.GetReaderList()
            });
        }

        /// <summary>
        /// Disconnect all readers
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> DisconnectAll()
        {
            await _rfidService.DisconnectAllAsync();
            return Json(new
            {
                success = true,
                message = "Disconnect all initiated.",
                readers = _rfidService.GetReaderList()
            });
        }

        /// <summary>
        /// Connect a single reader
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> ConnectReader(string ipAddress)
        {
            var result = await _rfidService.ConnectReaderAsync(ipAddress);
            return Json(new
            {
                success = result.Success,
                message = result.Message,
                readers = _rfidService.GetReaderList()
            });
        }

        /// <summary>
        /// Disconnect a single reader
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> DisconnectReader(string ipAddress)
        {
            var result = await _rfidService.DisconnectReaderAsync(ipAddress);
            return Json(new
            {
                success = result.Success,
                message = result.Message,
                readers = _rfidService.GetReaderList()
            });
        }
    }
}
