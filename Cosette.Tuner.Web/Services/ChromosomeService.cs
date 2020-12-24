using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cosette.Tuner.Web.Database;
using Cosette.Tuner.Web.Database.Models;
using Microsoft.EntityFrameworkCore;

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

        public async Task<List<ChromosomeModel>> GetAll(int testId)
        {
            return await _databaseContext.Chromosomes
                .Where(p => p.TestId == testId)
                .Include(p => p.SelfPlayStatistics)
                .Include(p => p.Genes)
                .ToListAsync();
        }

        public async Task<List<ChromosomeModel>> GetBest(int testId, int count)
        {
            return await _databaseContext.Chromosomes
                .Where(p => p.TestId == testId)
                .OrderByDescending(p => p.Fitness)
                .Include(p => p.Genes)
                .Take(count)
                .ToListAsync();
        }
    }
}