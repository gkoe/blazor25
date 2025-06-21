namespace Core.Contracts.Persistence
{
    public interface IUnitOfWork : IDisposable
    {

        ICustomerRepository Customers { get; }
        IOrderItemRepository OrderItems { get; }
        IOrderRepository Orders { get; }
        IProductRepository Products { get; }

        Task<int> SaveChangesAsync();

        Task DeleteDatabaseAsync();
        Task MigrateDatabaseAsync();

        Task FillDbAsync();
    }
}
