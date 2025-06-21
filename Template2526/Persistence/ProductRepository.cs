using Core.Contracts;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Persistence
{
    public class ProductRepository(ApplicationDbContext dbContext) : IProductRepository
    {
        public DbContext Context { get; } = dbContext;
        private ApplicationDbContext DbContext => (ApplicationDbContext)Context;

        public async Task<List<Product>> GetAllWithCategoriesAsync()
        {
            return await DbContext.Products.Include(p=>p.Categories).OrderBy(p=>p.ProductNr).ToListAsync();
        }

        public async Task<List<Product>> GetAllOrderedByNameAsync()
        {
            return await DbContext.Products.OrderBy(p => p.ProductNr).ToListAsync();
        }
    }
}
