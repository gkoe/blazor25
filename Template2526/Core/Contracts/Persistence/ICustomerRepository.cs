using Core.Entities;

namespace Core.Contracts
{
    public interface ICustomerRepository
    {
        Task<List<Customer>> GetAllAsync();
        void Add(Customer customer);
        Task<bool> IsFullNameUniqueAsync(string firstName, string lastName);
    }
}
