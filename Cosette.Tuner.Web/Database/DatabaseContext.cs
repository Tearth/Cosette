using System;
using Cosette.Tuner.Web.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cosette.Tuner.Web.Database
{
    public class DatabaseContext : DbContext
    {
        public virtual DbSet<ChromosomeGeneModel> ChromosomeGenes { get; set; }
        public virtual DbSet<ChromosomeModel> Chromosomes { get; set; }
        public virtual DbSet<EngineStatisticsModel> EngineStatistics { get; set; }
        public virtual DbSet<GenerationGeneModel> GenerationGenes { get; set; }
        public virtual DbSet<GenerationModel> Generations { get; set; }
        public virtual DbSet<TestModel> Tests { get; set; }

        public static readonly ILoggerFactory DatabaseLoggerFactory = LoggerFactory.Create(builder => { builder.AddDebug(); });

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(DatabaseLoggerFactory);
        }
    }
}
