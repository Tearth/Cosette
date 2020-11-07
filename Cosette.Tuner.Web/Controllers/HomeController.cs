﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Cosette.Tuner.Web.Database;
using Cosette.Tuner.Web.Services;
using Cosette.Tuner.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Cosette.Tuner.Web.Controllers
{
    public class HomeController : Controller
    {
        private IMapper _mapper;
        private TestService _testService;
        private ChromosomeService _chromosomeService;
        private GenerationService _generationService;

        public HomeController(IMapper mapper, TestService testService, ChromosomeService chromosomeService, GenerationService generationService)
        {
            _mapper = mapper;
            _testService = testService;
            _chromosomeService = chromosomeService;
            _generationService = generationService;
        }

        [HttpGet]
        [Route("{id?}")]
        public async Task<IActionResult> Index(int? id)
        {
            var test = id.HasValue ? await _testService.GetTestById(id.Value) : await _testService.GetLastTest();
            var allTests = await _testService.GetAll(true);
            var allGenerations = await _generationService.GetAll(test.Id);
            var bestGenerations = await _generationService.GetBest(test.Id, 5);
            var bestChromosomes = await _chromosomeService.GetBest(test.Id, 5);

            return View(new MainViewModel
            {
                LastTest = _mapper.Map<TestViewModel>(test),
                Tests = _mapper.Map<List<TestViewModel>>(allTests),
                AllGenerations = _mapper.Map<List<GenerationViewModel>>(allGenerations),
                BestGenerations = _mapper.Map<List<GenerationViewModel>>(bestGenerations),
                BestChromosomes = _mapper.Map<List<ChromosomeViewModel>>(bestChromosomes),
            });
        }
    }
}
