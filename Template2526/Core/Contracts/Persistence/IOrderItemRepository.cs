using Base.Contracts.Persistence;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Contracts
{
    public interface IOrderItemRepository : IGenericRepository<OrderItem>
    {
        Task<List<OrderItem>> GetByOrderIdAsync(int orderId);
        void Insert(OrderItem orderItem);
        Task DeleteByIdAsync(int id);
        //Task<OrderItem?> GetByIdAsync(int id);
        void Delete(OrderItem orderItem);
    }
}
