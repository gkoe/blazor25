using System.ComponentModel.DataAnnotations;
using Base.Contracts.Persistence;
using Base.Contracts.Validation;

using Microsoft.EntityFrameworkCore;

namespace Base.Persistence
{
    public class BaseUnitOfWork(BaseApplicationDbContext dbContext) : IBaseUnitOfWork
    {
        public BaseApplicationDbContext BaseApplicationDbContext { get; init; } = dbContext;
        private bool _disposed;


        public async Task<int> SaveChangesAsync()
        {
            // Geänderte Entities ermitteln
            var entities = BaseApplicationDbContext.ChangeTracker.Entries()
                .Where(entity => entity.State == EntityState.Added || entity.State == EntityState.Modified)
                .Select(e => e.Entity)
                .ToArray();  
            foreach (var entity in entities)
            {
                var validationContext = new ValidationContext(entity, null, null);
                List<ValidationException> validationExceptions = [];
                // Datenbankabhängige Validierungen durchführen und in ValidationExceptions sammeln
                if (entity is IDatabaseValidatableObject dbValidatableObject) // UnitOfWork injizieren, wenn Interface implementiert ist
                {
                    var result = await dbValidatableObject.ValidateAsync(this);
                    if (result != null && result.ErrorMessage?.Length >0)
                    {
                        var memberNames = new List<string>();
                        validationExceptions.Add(new ValidationException(result, null,
                                    result.MemberNames));
                            memberNames.AddRange(result.MemberNames);
                    }
                }
                // Standard-Validierungen durchführen und ebenfalls in ValidationExceptions sammeln
                var validationResults = new List<ValidationResult>();
                var isValid = Validator.TryValidateObject(entity, validationContext, validationResults,
                    validateAllProperties: true);
                if (!isValid)
                {
                    var memberNames = new List<string>();
                    //List<ValidationException> validationExceptions = [];
                    foreach (ValidationResult validationResult in validationResults)
                    {
                        validationExceptions.Add(new ValidationException(validationResult, null,
                                validationResult.MemberNames));
                        memberNames.AddRange(validationResult.MemberNames);
                    }
                }
                // Wenn ValidationExceptions vorhanden sind, Exception werfen
                if (validationExceptions.Count != 0)
                {
                    if (validationExceptions.Count == 1)  // eine Validationexception werfen
                    {
                        throw validationExceptions.Single();
                    }
                    else  // AggregateException mit allen ValidationExceptions als InnerExceptions werfen
                    {
                        throw new ValidationException($"Entity validation failed",
                            new AggregateException(validationExceptions));
                    }
                }

            }
            return await BaseApplicationDbContext.SaveChangesAsync();
        }

        public async Task DeleteDatabaseAsync() => await BaseApplicationDbContext.Database.EnsureDeletedAsync();
        public async Task MigrateDatabaseAsync() => await BaseApplicationDbContext.Database.MigrateAsync();
        public async Task CreateDatabaseAsync() => await BaseApplicationDbContext.Database.EnsureCreatedAsync();

        #region Dispose
        public async ValueTask DisposeAsync()
        {
            await DisposeAsync(true);
        }

        protected virtual async ValueTask DisposeAsync(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    await BaseApplicationDbContext.DisposeAsync();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            BaseApplicationDbContext.Dispose();
            GC.SuppressFinalize(this);
        }


        #endregion

    }
}
