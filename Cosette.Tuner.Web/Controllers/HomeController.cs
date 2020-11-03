using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cosette.Tuner.Web.Database;
using Microsoft.AspNetCore.Mvc;

namespace Cosette.Tuner.Web.Controllers
{
    public class HomeController : Controller
    {
        public HomeController(DatabaseContext context)
        {

        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
