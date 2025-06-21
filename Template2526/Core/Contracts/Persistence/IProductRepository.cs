using Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Contracts
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllWithCategoriesAsync();
        Task<List<Product>> GetAllOrderedByNameAsync();
    }
}
