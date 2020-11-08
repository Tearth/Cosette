using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Cosette.Tuner.Web.Database;
using Cosette.Tuner.Web.Services;
using Cosette.Tuner.Web.ViewModels;
using Cosette.Tuner.Web.ViewModels.ChartJs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Cosette.Tuner.Web.Controllers
{
    public class HomeController : Controller
    {
        private IMapper _mapper;
        private TestService _testService;
        private ChromosomeService _chromosomeService;
        private GenerationService _generationService;
        private ChartJsService _chartJsService;

        public HomeController(IMapper mapper, TestService testService, ChromosomeService chromosomeService, GenerationService generationService, ChartJsService chartJsService)
        {
            _mapper = mapper;
            _testService = testService;
            _chromosomeService = chromosomeService;
            _generationService = generationService;
            _chartJsService = chartJsService;
        }

        [HttpGet]
        [Route("{id?}")]
        public async Task<IActionResult> Index(int? id)
        {
            var test = id.HasValue ? await _testService.GetTestById(id.Value) : await _testService.GetLastTest();
            var allTests = await _testService.GetAll();
            var allGenerations = await _generationService.GetAll(test.Id);
            var bestGenerations = await _generationService.GetBest(test.Id, 5);
            var allChromosomes = await _chromosomeService.GetAll(test.Id);
            var bestChromosomes = await _chromosomeService.GetBest(test.Id, 5);

            var generationFitnessLabels = Enumerable.Range(0, allGenerations.Count).Select(p => p.ToString()).ToList();
            var generationFitnessValues = allGenerations.Select(p => p.BestFitness).ToList();
            var generationFitnessDatasets = new List<ChartJsDataset<int>>
            {
                _chartJsService.GenerateDataset("Generation fitness", generationFitnessValues, "#4dc9f6", "#4dc9f6")
            };
            var generationFitnessData = _chartJsService.GenerateData(generationFitnessLabels, generationFitnessDatasets);

            return View(new MainViewModel
            {
                LastTest = _mapper.Map<TestViewModel>(test),
                Tests = _mapper.Map<List<TestViewModel>>(allTests),
                AllGenerations = _mapper.Map<List<GenerationViewModel>>(allGenerations),
                BestGenerations = _mapper.Map<List<GenerationViewModel>>(bestGenerations),
                AllChromosomes = _mapper.Map<List<ChromosomeViewModel>>(allChromosomes),
                BestChromosomes = _mapper.Map<List<ChromosomeViewModel>>(bestChromosomes),

                GenerationFitnessChartJson = JsonConvert.SerializeObject(generationFitnessData)
            });
        }
    }
}
