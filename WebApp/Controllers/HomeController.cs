using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Neo4JSample;
using Neo4JSample.Model;
using Neo4JSample.Settings;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ConnectionSettings settings = ConnectionSettings.CreateBasicAuth("bolt://hobby-pfoigaoofoojgbkejnnpffdl.dbs.graphenedb.com:24787", "mateo", "b.W43aW5nzmslv.h1dNlQp5mVRQcJVM");

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public JsonResult GetMovies()
        {
            using (var client = new Neo4JClient(settings))
            {
                IList<Movie> result = client.GetSpecificMovies(null);
                return Json(result);
            }
        }

        [HttpGet]
        public JsonResult GetPersons(string movieType, string movieTitle, string personsRelation)
        {
            using (var client = new Neo4JClient(settings))
            {
                IList<Person> result = client.GetSpecificPersons(movieType, movieTitle, personsRelation);
                return Json(result);
            }
        }
    }
}
