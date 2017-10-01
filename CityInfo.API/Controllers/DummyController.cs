using Microsoft.AspNetCore.Mvc;
using CityInfo.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CityInfo.API.Controllers
{
    public class DummyController : Controller
    {
        private CityInfoContext _context;
        private ILogger<DummyController> _logger;

        public DummyController(
            CityInfoContext context,
            ILogger<DummyController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Route("api/testdatabase")]
        public IActionResult TestDatabase()
        {
            _logger.LogInformation($"create database");
            return Ok();
        }
    }
}
