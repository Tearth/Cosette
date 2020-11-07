using System;
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

        public async Task<IActionResult> Index()
        {
            var viewModel = new MainViewModel
            {
                Tests = _mapper.Map<List<TestViewModel>>(await _testService.GetAll(true))
            };

            return View(viewModel);
        }
    }
}
