using System;
using System.Collections.Generic;

namespace Cosette.Tuner.Web.Database.Models
{
    public class TestModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreationTimeUtc { get; set; }

        public List<GenerationModel> Generation { get; set; }
    }
}
