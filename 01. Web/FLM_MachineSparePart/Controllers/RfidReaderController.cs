using FILM_Sparepart_MVC.Services;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

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

        public JsonResult GetStatus()
        {
            var readers = RFIDService.Instance.GetReaderList();
            return Json(readers, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Diagnostic()
        {
            var readers = RFIDService.Instance.GetReaderList();
            var result = readers.Select(r => new
            {
                r.IPAddress,
                r.Location,
                r.ReaderName,
                r.Status,
                r.IsConnected,
                SDKIsConnected = GetSDKConnectionState(r.IPAddress)
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private bool GetSDKConnectionState(string ipAddress)
        {
            // Access internal state via a new public method on RFIDService
            return RFIDService.Instance.IsReaderSDKConnected(ipAddress);
        }
    }
}
