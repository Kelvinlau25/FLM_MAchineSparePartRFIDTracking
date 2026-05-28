using System;
using System.Web.Mvc;
using FILM_Sparepart_MVC.Services;

namespace FILM_Sparepart_MVC.Controllers
{
    public class RfidConnectController : Controller
    {
        private readonly RFIDService _rfidService;

        public RfidConnectController()
        {
            _rfidService = RFIDService.Instance;
        }

        public ActionResult Index()
        {
            try
            {
                var model = _rfidService.GetDefaultReaders();
                return View(model);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading RFID readers: " + ex.Message);
                throw;
            }
        }
    }
}