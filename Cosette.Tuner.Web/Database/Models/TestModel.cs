using System;
using System.Collections.Generic;
using Cosette.Tuner.Common.Requests;

namespace Cosette.Tuner.Web.Database.Models
{
    public class TestModel
    {
        public int Id { get; set; }
        public DateTime CreationTimeUtc { get; set; }
        public TestType Type { get; set; }

        public virtual List<GenerationModel> Generation { get; set; }
    }
}
