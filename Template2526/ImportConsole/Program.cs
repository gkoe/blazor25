using Base.Contracts.Persistence;
using Base.Persistence;
using Core.Contracts;
using Core.Contracts.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Persistence;


Console.WriteLine("Import der Buchungen in die Datenbank");
Console.WriteLine("Daten werden eingelesen!\n");

// Konfiguration laden
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

var serviceCollection = new ServiceCollection();
// Logging hinzufügen
serviceCollection.AddLogging(builder =>
{
    builder.AddSimpleConsole(options =>
    {
        // Format auf eine Zeile beschränken
        options.SingleLine = true;

        // Optional: Kategorie und Zeitstempel ausschalten oder anpassen
        options.TimestampFormat = "[HH:mm:ss] ";
        options.IncludeScopes = false;
    });
});

// DbContext mit ConnectionString aus Konfiguration registrieren
serviceCollection.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

serviceCollection.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
serviceCollection.AddScoped<ICustomerRepository, CustomerRepository>();
serviceCollection.AddScoped<IOrderItemRepository, OrderItemRepository>();
serviceCollection.AddScoped<IOrderRepository, OrderRepository>();
serviceCollection.AddScoped<IProductRepository, ProductRepository>();

serviceCollection.AddScoped<IUnitOfWork, UnitOfWork>();

// ServiceProvider bauen
var serviceProvider = serviceCollection.BuildServiceProvider();

// Logger zum direkten Gebrauch (z. B. im Program.Main)
var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
logger.LogInformation("DI-Infrastruktur aufgebaut");

// Scoped-Dienst abrufen
using var scope = serviceProvider.CreateScope();
var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

await uow.FillDbAsync();
var products = await uow.Products.GetAllWithCategoriesAsync();
logger.LogInformation("Es wurden {Count} Produkte eingelesen!", products.Count);

foreach (var product in products)
{
    var categoryList = string.Join(" ", product.Categories.Select(c => c.CategoryName));
    logger.LogInformation("{Nr,4} {Name,-20} {Price,-20} {Categories}",
        product.ProductNr, product.Name, product.Price, categoryList);
}

logger.LogInformation("<Eingabetaste drücken>");
