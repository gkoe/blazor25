using Base.Contracts.Persistence;
using Base.Persistence;
using Core.Contracts;
using Core.DataTransferObjects;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Persistence
{
    public class OrderRepository(ApplicationDbContext dbContext, ILogger<OrderRepository> logger)
            : GenericRepository<Order>(dbContext, logger), IOrderRepository
    {
        //public DbContext Context { get; } = dbContext;
        private ApplicationDbContext DbContext => (ApplicationDbContext)Context;

        public void Delete(Order order)
        {
            DbContext.Orders.Remove(order);
        }


        /// <summary>
        /// Berechnet die Verkaufsstatistik
        /// </summary>
        /// <returns></returns>
        public async Task<SalesStatisticDto> GetSalesStatisticAsync()
        {
            var res = new SalesStatisticDto
            {
                TotalSales = await DbContext.OrderItems.SumAsync(oi => oi.Amount * oi.Product!.Price)
            };
            var topProduct = await DbContext.OrderItems.GroupBy(oi =>
              new
              {
                  oi.ProductId,
                  ProductName = oi.Product!.Name
              }).Select(grp =>
              new
              {
                  grp.Key.ProductId,
                  grp.Key.ProductName,
                  TotalSales = grp.Sum(oi => oi.Amount * oi.Product!.Price)
              }).OrderByDescending(p => p.TotalSales)
              .FirstOrDefaultAsync();

            if (topProduct != null)
            {
                res.BestProduct = topProduct.ProductName;
                res.BestProductSales = topProduct.TotalSales;
            }

            res.CustomerTotalOrders = await DbContext.OrderItems.GroupBy(oi => new
            {
                oi.Order!.CustomerId,
                CustomerName = oi.Order.Customer!.FirstName + " " + oi.Order.Customer.LastName
            }).Select(grp =>
              new CustomerTotalOrderDto()
              {
                  CustomerName=grp.Key.CustomerName,
                  NumberOfOrders=grp.Count(),
                  TotalSales = grp.Sum(oi=>oi.Amount*oi.Product!.Price)
              }).OrderByDescending(c=>c.TotalSales).ToListAsync();
                  

            return res;
        }
    }
}
