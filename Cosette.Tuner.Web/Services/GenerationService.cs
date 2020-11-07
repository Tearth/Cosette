using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cosette.Tuner.Web.Database;
using Cosette.Tuner.Web.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Cosette.Tuner.Web.Services
{
    public class GenerationService
    {
        private DatabaseContext _databaseContext;

        public GenerationService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task Add(GenerationModel generationModel)
        {
            await _databaseContext.Generations.AddAsync(generationModel);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task<List<GenerationModel>> GetBest(int testId, int count)
        {
            return await _databaseContext.Generations
                .Where(p => p.TestId == testId)
                .OrderByDescending(p => p.BestFitness)
                .Include(p => p.BestGenes)
                .Take(count)
                .ToListAsync();
        }
    }
}
