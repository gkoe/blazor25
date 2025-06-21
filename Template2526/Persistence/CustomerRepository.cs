using Core.Contracts;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class CustomerRepository(ApplicationDbContext dbContext) : ICustomerRepository
    {
        public DbContext Context { get; } = dbContext;
        private ApplicationDbContext DbContext => (ApplicationDbContext)Context;
        public void Add(Customer customer)
        {
            DbContext.Customers.Add(customer);
        }

        public async Task<List<Customer>> GetAllAsync()
        {
            return await DbContext.Customers.OrderBy(c => c.LastName).ThenBy(c => c.FirstName).ToListAsync();

        }

        public async Task<bool> IsFullNameUniqueAsync(string firstName, string lastName)
        {
            return !await DbContext.Customers.AnyAsync( c => c.FirstName == firstName && 
                    c.LastName == lastName);
        }
    }
}
