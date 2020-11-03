using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Cosette.Tuner.Web.Controllers
{
    public class ApiController : Controller
    {
        public IActionResult Index()
        {
            return new OkResult();
        }
    }
}
