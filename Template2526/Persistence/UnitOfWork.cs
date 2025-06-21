
using Base.Helper;
using Base.Persistence;
using Core.Contracts;
using Core.Contracts.Persistence;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence
{
    public class UnitOfWork : BaseUnitOfWork, IUnitOfWork
    {
        public ApplicationDbContext ApplicationDbContext => (ApplicationDbContext)BaseApplicationDbContext;

        public ICustomerRepository Customers { get; }
        public IOrderItemRepository OrderItems { get; }
        public IOrderRepository Orders { get; }
        public IProductRepository Products { get; }

        public UnitOfWork(
            ApplicationDbContext applicationDbContext,
            ICustomerRepository customers,
            IOrderItemRepository orderItems,
            IOrderRepository orders,
            IProductRepository products)
            : base(applicationDbContext)
        {
            Customers = customers;
            OrderItems = orderItems;
            Orders = orders;
            Products = products;
        }


        /// <summary>
        /// Initializes the database with data from CSV files.  
        /// </summary>
        /// <returns></returns>
        public async Task FillDbAsync()
        {
            await DeleteDatabaseAsync();
            await MigrateDatabaseAsync();

            var products = (await MyFile.ReadStringMatrixFromCsvAsync("Products.csv", true))
                .Select(l => new Product()
                {
                    ProductNr = l[0],
                    Name = l[1],
                    Price = Double.Parse(l[2])
                }).ToList();



            var orderItemsCsv = await MyFile.ReadStringMatrixFromCsvAsync("OrderItems.csv", true);
            var productCategoryCsv = await MyFile.ReadStringMatrixFromCsvAsync("ProductCategory.csv", true);

            var customers = orderItemsCsv.GroupBy(line => new
            {
                FirstName = line[4],
                LastName = line[3],
                CustomerNr = line[2]
            })
                .Select(grp => new Customer()
                {
                    CustomerNr = grp.Key.CustomerNr,
                    FirstName = grp.Key.FirstName,
                    LastName = grp.Key.LastName
                }).ToList();

            var orders = orderItemsCsv.GroupBy(line => new
            {
                OrderNr = line[0],
                Date = DateTime.Parse(line[1]),
                CustomerNr = line[2],
                OrderType = line[7]
            })
                .Select(grp => new Order()
                {
                    OrderNr = grp.Key.OrderNr,
                    Date = grp.Key.Date,
                    Customer = customers.First(c => c.CustomerNr == grp.Key.CustomerNr),
                    OrderType = (OrderType)Enum.Parse(typeof(OrderType), grp.Key.OrderType, true)
                }).ToList();

            var orderItems = orderItemsCsv.Select(l => new OrderItem()
            {
                Order = orders.First(o => o.OrderNr == l[0]),
                Product = products.First(p => p.ProductNr == l[5]),
                Amount = Int32.Parse(l[6])
            }).ToList();

            var categories = productCategoryCsv.GroupBy(l => l[1]).Select
                (grp => new Category()
                {
                    CategoryName = grp.Key,
                    Products = grp.Select(productLine => products.Single(p => p.ProductNr == productLine[0])).ToList()
                });

            ApplicationDbContext.Products.AddRange(products);
            ApplicationDbContext.Customers.AddRange(customers);
            ApplicationDbContext.Orders.AddRange(orders);
            ApplicationDbContext.OrderItems.AddRange(orderItems);
            ApplicationDbContext.Categories.AddRange(categories);

            await SaveChangesAsync();
        }

    }
}
