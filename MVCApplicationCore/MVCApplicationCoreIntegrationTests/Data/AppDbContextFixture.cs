using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MVCApplicationCore.Data;
using MVCApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCApplicationCoreIntegrationTests.Data
{
    public class AppDbContextFixture : IDisposable
    {
        public AppDbContext _context { get; private set; }
        
        public AppDbContextFixture()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(configuration.GetConnectionString("IntegrationTestDb"))
                .Options;

            _context = new AppDbContext(dbContextOptions);

            // Optionally, clear and seed the test database
            SeedTestDatabase();
        }

        private void SeedTestDatabase()
        {
            // Clear existing data and add test data
            _context.Categories.RemoveRange(_context.Categories);
            _context.Products.RemoveRange(_context.Products);
            _context.SaveChanges();

            var category1 = new Category { Name = "Category 1", Description = "Description 1", FileName = "" };
            var category2 = new Category { Name = "Category 2", Description = "Description 2", FileName = "" };

            _context.Categories.AddRange(category1, category2);
            _context.Products.AddRange(
                new Product { ProductName = "Product 1", Category = category1, ProductDescription = "Description 1", ProductPrice=10M },
                new Product { ProductName = "Product 2", Category = category1, ProductDescription = "Description 2", ProductPrice = 20M },
                new Product { ProductName = "Product 3", Category = category2, ProductDescription = "Description 3", ProductPrice = 30M }
            );
            
            _context.SaveChanges();
        }

        public void Dispose()
        {
            // Optionally, clean up the database
            _context.Database.ExecuteSqlRaw("TRUNCATE TABLE Products");
            _context.Database.ExecuteSqlRaw("DELETE FROM Categories");
            _context.Dispose();
        }
    }
}
