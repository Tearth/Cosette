using System.Collections.Generic;

namespace Cosette.Tuner.Web.ViewModels
{
    public class MainViewModel
    {
        public TestViewModel LastTest { get; set; }
        public List<TestViewModel> Tests { get; set; }
    }
}
