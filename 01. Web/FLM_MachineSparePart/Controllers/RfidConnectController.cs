using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using FILM_Sparepart_MVC.Services;

namespace FILM_Sparepart_MVC.Controllers
{
    public class RfidConnectController : Controller
    {
        private readonly RFIDService _rfidService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<RfidConnectController> _logger;

        public RfidConnectController(RFIDService rfidService, IConfiguration configuration, ILogger<RfidConnectController> logger)
        {
            _rfidService = rfidService;
            _configuration = configuration;
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("RFID Connect page accessed");
            try
            {
                var model = _rfidService.GetDefaultReaders();
                _logger.LogInformation("Retrieved {Count} RFID readers", model?.Count ?? 0);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading RFID readers");
                throw;
            }
        }
    }
}