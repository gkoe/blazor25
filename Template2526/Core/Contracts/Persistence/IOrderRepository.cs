using Base.Contracts.Persistence;
using Core.DataTransferObjects;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Core.Contracts
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<SalesStatisticDto> GetSalesStatisticAsync();
    }
}
