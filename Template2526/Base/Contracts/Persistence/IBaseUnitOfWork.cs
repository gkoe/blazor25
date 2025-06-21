using Base.Persistence;

using System;
using System.Threading.Tasks;

namespace Base.Contracts.Persistence
{
    public interface IBaseUnitOfWork : IDisposable
    {
        BaseApplicationDbContext BaseApplicationDbContext { get; init; }

        Task<int> SaveChangesAsync();
        Task DeleteDatabaseAsync();
        Task MigrateDatabaseAsync();
        Task CreateDatabaseAsync();
    }
}
