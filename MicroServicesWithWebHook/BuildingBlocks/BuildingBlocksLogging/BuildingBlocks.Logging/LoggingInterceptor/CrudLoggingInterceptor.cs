using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Logging.LoggingInterceptor
{
    public class CrudLoggingInterceptor : SaveChangesInterceptor
    {
        private readonly ILogger<CrudLoggingInterceptor> _logger;

        public CrudLoggingInterceptor(ILogger<CrudLoggingInterceptor> logger)
        {
            this._logger = logger;
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            LogCrudOperation(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void LogCrudOperation(DbContext? dbContext)
        {
            if (dbContext is null)
            {
                return;
            }

            foreach (var entry in dbContext.ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                    case EntityState.Modified:
                    case EntityState.Deleted:

                        this._logger.LogInformation("Database CRUD: [{State}] operation performed on [{EntityName}] at {Time}", entry.State, entry.Entity.GetType().Name, DateTime.UtcNow);
                        break;
                }
            }
        }
    }
}
