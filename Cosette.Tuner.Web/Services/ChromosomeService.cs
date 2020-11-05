using System.Threading.Tasks;
using Cosette.Tuner.Web.Database;
using Cosette.Tuner.Web.Database.Models;

namespace Cosette.Tuner.Web.Services
{
    public class ChromosomeService
    {
        private DatabaseContext _databaseContext;

        public ChromosomeService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task Add(ChromosomeModel chromosomeModel)
        {
            await _databaseContext.Chromosomes.AddAsync(chromosomeModel);
            await _databaseContext.SaveChangesAsync();
        }
    }
}