using Base.Contracts.Persistence;
using Base.Persistence;
using Core.Contracts;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Persistence
{
    public class OrderItemRepository(ApplicationDbContext dbContext, ILogger<OrderItemRepository> logger) : GenericRepository<OrderItem>(dbContext, logger), IOrderItemRepository
    {
        //public DbContext Context { get; } = dbContext;
        private ApplicationDbContext DbContext => (ApplicationDbContext)Context;

        //public Task<EntityEntry<OrderItem>> AddAsync(OrderItem entity)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task AddRangeAsync(IEnumerable<OrderItem> entities)
        //{
        //    throw new NotImplementedException();
        //}

        //public void Attach(OrderItem entity)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<int> CountAsync(Expression<Func<OrderItem, bool>>? filter = null, params string[] includeProperties)
        //{
        //    throw new NotImplementedException();
        //}

        public void Delete(OrderItem orderItem)
        {
            DbContext.OrderItems.Remove(orderItem);
        }

        public async Task DeleteByIdAsync(int orderItemId)
        {
            var orderItemToDelete = await DbContext.OrderItems.SingleOrDefaultAsync(o => o.Id == orderItemId);
            if (orderItemToDelete!=null)
               DbContext.OrderItems.Remove(orderItemToDelete);
        }

        //public Task<bool> ExistsAsync(int id)
        //{
        //    throw new NotImplementedException();
        //}

        //public async Task<OrderItem?> GetByIdAsync(int id)
        //{
        //    return await DbContext.OrderItems.SingleOrDefaultAsync(item => item.Id == id);
        //}

        public async Task<List<OrderItem>> GetByOrderIdAsync(int orderId)
        {
            return await DbContext.OrderItems.AsNoTracking().Include(oi => oi.Product).Where(oi => oi.OrderId == orderId).OrderBy(oi => oi.Product!.Name).ToListAsync();
        }

        //public bool HasChanges()
        //{
        //    throw new NotImplementedException();
        //}

        public void Insert(OrderItem newOrderItem)
        {
            DbContext.OrderItems.Add(newOrderItem);
        }

        //public bool Delete(int id)
        //{
        //    throw new NotImplementedException();
        //}

        //public void Delete(OrderItem entityToDelete)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
