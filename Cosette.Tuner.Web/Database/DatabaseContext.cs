using Cosette.Tuner.Web.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Cosette.Tuner.Web.Database
{
    public class DatabaseContext : DbContext
    {
        public DbSet<ChromosomeGeneModel> ChromosomeGenes { get; set; }
        public DbSet<ChromosomeModel> Chromosomes { get; set; }
        public DbSet<EngineStatisticsModel> EngineStatistics { get; set; }
        public DbSet<GenerationGeneModel> GenerationGenes { get; set; }
        public DbSet<GenerationModel> Generations { get; set; }
        public DbSet<TestModel> Tests { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {

        }
    }
}
