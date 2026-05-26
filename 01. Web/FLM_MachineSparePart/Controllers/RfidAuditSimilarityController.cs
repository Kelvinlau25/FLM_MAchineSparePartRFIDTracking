using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FILM_Sparepart_MVC.Controllers
{
    public class RfidAuditSimilarityController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<RfidAuditSimilarityController> _logger;

        public RfidAuditSimilarityController(IConfiguration configuration, ILogger<RfidAuditSimilarityController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
    }
}