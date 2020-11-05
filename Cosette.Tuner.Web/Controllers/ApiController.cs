using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cosette.Tuner.Common.Requests;
using Cosette.Tuner.Common.Responses;
using Cosette.Tuner.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cosette.Tuner.Web.Controllers
{
    public class ApiController : Controller
    {
        private TestService _testService;
        private ChromosomeService _chromosomeService;
        private GenerationService _generationService;

        public ApiController(TestService testService, ChromosomeService chromosomeService, GenerationService generationService)
        {
            _testService = testService;
            _chromosomeService = chromosomeService;
            _generationService = generationService;
        }

        [HttpGet]
        [Route("api/ping")]
        public async Task<IActionResult> Ping()
        {
            return new OkResult();
        }

        [HttpPost]
        [Route("api/test/register")]
        public async Task<IActionResult> RegisterTest()
        {
            var response = new TestDataResponse
            {
                Id = await _testService.GenerateNewTest()
            };

            return new JsonResult(response);
        }

        [HttpPost]
        [Route("api/generation")]
        public async Task<IActionResult> Generation([FromBody] GenerationDataRequest requestData)
        {
            //await _generationService.Add();
            return new OkResult();
        }

        [HttpPost]
        [Route("api/chromosome")]
        public async Task<IActionResult> Chromosome([FromBody] ChromosomeDataRequest requestData)
        {
            //await _chromosomeService.Add()
            return new OkResult();
        }
    }
}
