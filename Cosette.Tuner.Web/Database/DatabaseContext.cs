using Cosette.Tuner.Web.Database.Models;
using Microsoft.EntityFrameworkCore;

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

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {

        }
    }
}
