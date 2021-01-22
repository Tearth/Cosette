using System;
using Cosette.Tuner.Common.Requests;

namespace Cosette.Tuner.Web.ViewModels
{
    public class TestViewModel
    {
        public int Id { get; set; }
        public DateTime CreationTimeUtc { get; set; }
        public TestType Type { get; set; }
    }
}
