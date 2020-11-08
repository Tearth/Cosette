using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cosette.Tuner.Web.Database;
using Cosette.Tuner.Web.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace Cosette.Tuner.Web.Services
{
    public class TestService
    {
        private DatabaseContext _databaseContext;

        public TestService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<int> GenerateNewTest()
        {
            var entityTracking = await _databaseContext.Tests.AddAsync(new TestModel
            {
                CreationTimeUtc = DateTime.Now,
            });

            await _databaseContext.SaveChangesAsync();
            return entityTracking.Entity.Id;
        }

        public async Task<List<TestModel>> GetAll()
        {
            return await _databaseContext.Tests.ToListAsync();
        }

        public async Task<TestModel> GetLastTest()
        {
            return await _databaseContext.Tests.OrderByDescending(p => p.Id).FirstOrDefaultAsync();
        }

        public async Task<TestModel> GetTestById(int testId)
        {
            return await _databaseContext.Tests.FirstOrDefaultAsync(p => p.Id == testId);
        }
    }
}