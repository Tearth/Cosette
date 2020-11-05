using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cosette.Tuner.Common.Requests;
using Cosette.Tuner.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cosette.Tuner.Web.Controllers
{
    public class ApiController : Controller
    {
        private GenerationService _generationService;

        public ApiController(GenerationService generationService)
        {
            _generationService = generationService;
        }

        [HttpGet]
        [Route("api/ping")]
        public IActionResult Ping()
        {
            return new OkResult();
        }

        [HttpPost]
        [Route("api/generation")]
        public IActionResult Generation([FromBody] GenerationDataRequest requestData)
        {
            return new OkResult();
        }

        [HttpPost]
        [Route("api/chromosome")]
        public IActionResult Chromosome([FromBody] ChromosomeDataRequest requestData)
        {
            return new OkResult();
        }
    }
}
