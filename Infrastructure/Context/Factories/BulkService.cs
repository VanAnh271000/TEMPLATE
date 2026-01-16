using Application.Interfaces.Commons;
using EFCore.BulkExtensions;
using Infrastructure.Context;

namespace Infrastructure.Context.Factories
{
    public class BulkService : IBulkService
    {
        private readonly ApplicationDbContext _context;
        public BulkService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task BulkInsertAsync<T>(IList<T> entities) where T : class
        {
            if (!entities.Any()) return;

            await _context.BulkInsertAsync(entities, bulkConfig =>
            {
                bulkConfig.BatchSize = 5000;
                bulkConfig.SetOutputIdentity = true;
            });
        }

        public async Task BulkUpdateAsync<T>(IList<T> entities) where T : class
        {
            if (!entities.Any()) return;

            await _context.BulkUpdateAsync(entities, bulkConfig =>
            {
                bulkConfig.BatchSize = 5000;
                bulkConfig.SetOutputIdentity = true;
            });
        }

    }
}
