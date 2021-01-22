using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Cosette.Tuner.Common.Requests;
using Cosette.Tuner.Common.Responses;
using Cosette.Tuner.Web.Database.Models;
using Cosette.Tuner.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cosette.Tuner.Web.Controllers
{
    public class ApiController : Controller
    {
        private IMapper _mapper;
        private TestService _testService;
        private ChromosomeService _chromosomeService;
        private GenerationService _generationService;

        public ApiController(IMapper mapper, TestService testService, ChromosomeService chromosomeService, GenerationService generationService)
        {
            _mapper = mapper;
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
        public async Task<IActionResult> RegisterTest([FromBody] RegisterTestRequest requestData)
        {
            var response = new TestDataResponse
            {
                Id = await _testService.GenerateNewTest(requestData.Type)
            };

            return new JsonResult(response);
        }

        [HttpPost]
        [Route("api/generation")]
        public async Task<IActionResult> Generation([FromBody] GenerationDataRequest requestData)
        {
            var generationModel = _mapper.Map<GenerationModel>(requestData);
            await _generationService.Add(generationModel);

            return new OkResult();
        }

        [HttpPost]
        [Route("api/chromosome")]
        public async Task<IActionResult> Chromosome([FromBody] ChromosomeDataRequest requestData)
        {
            var chromosomeModel = _mapper.Map<ChromosomeModel>(requestData);
            await _chromosomeService.Add(chromosomeModel);

            return new OkResult();
        }
    }
}
