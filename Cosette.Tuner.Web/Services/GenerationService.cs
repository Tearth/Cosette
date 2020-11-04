using System.Threading.Tasks;
using Cosette.Tuner.Web.Database;
using Cosette.Tuner.Web.Database.Models;

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
            await _databaseContext.AddAsync(generationModel);
            await _databaseContext.SaveChangesAsync();
        }
    }
}
