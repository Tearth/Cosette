﻿using System;
using System.Collections.Generic;

namespace Cosette.Tuner.Web.Database.Models
{
    public class TestModel
    {
        public int Id { get; set; }
        public DateTime CreationTimeUtc { get; set; }

        public virtual List<GenerationModel> Generation { get; set; }
    }
}
