using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MVCApplicationCore.Data;
using MVCApplicationCore.Data.Implementation;
using MVCApplicationCore.Models;

namespace MVCApplicationCoreIntegrationTests
{
    //[Collection("Non-Parallel Collection")]
    public class ProductRepositoryTests : IDisposable
    {
        private readonly IAppDbContext _context;

        public ProductRepositoryTests()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(configuration.GetConnectionString("IntegrationTestDb"))
                .Options;

            _context = new AppDbContext(dbContextOptions);

            // Seed initial data
            SeedData();
        }

        [Fact]
        public void GetAllProducts_ReturnsAllProductsWithCategories()
        {
            // Arrange
            var productRepository = new ProductRepository(_context);

            // Act
            var products = productRepository.GetAllProducts();

            // Assert
            Assert.NotNull(products);
            Assert.Equal(2, products.Count());
            Assert.All(products, p => Assert.NotNull(p.Category));
        }

        [Fact]
        public void GetProductById_ReturnsCorrectProductWithCategory()
        {
            // Arrange
            var productRepository = new ProductRepository(_context);
            var seededProduct = _context.Products.First();

            // Act
            var product = productRepository.GetProductById(seededProduct.ProductId);

            // Assert
            Assert.NotNull(product);
            Assert.Equal(seededProduct.ProductId, product.ProductId);
            Assert.Equal(seededProduct.ProductName, product.ProductName);
            Assert.Equal(seededProduct.ProductDescription, product.ProductDescription);
            Assert.NotNull(product.Category);
            Assert.Equal(seededProduct.Category.Name, product.Category.Name);
            Assert.Equal(seededProduct.Category.Description, product.Category.Description);
        }

        [Fact]
        public void InsertProduct_AddsProductToDatabase()
        {
            // Arrange
            var productRepository = new ProductRepository(_context);
            var category = _context.Categories.First();
            var newProduct = new Product
            {
                ProductName = "New Product",
                ProductDescription = "New Product Description",
                CategoryId = category.CategoryId,
                ProductPrice = 10M,
                InStock = true,
                IsActive = true,
            };

            // Act
            var result = productRepository.InsertProduct(newProduct);
            var insertedProduct = _context.Products.FirstOrDefault(p => p.ProductName == newProduct.ProductName);

            // Assert
            Assert.True(result);
            Assert.NotNull(insertedProduct);
            Assert.Equal(newProduct.ProductName, insertedProduct.ProductName);
            Assert.Equal(newProduct.ProductDescription, insertedProduct.ProductDescription);
            Assert.Equal(newProduct.CategoryId, insertedProduct.CategoryId);
        }

        [Fact]
        public void UpdateProduct_UpdatesProductInDatabase()
        {
            // Arrange
            var productRepository = new ProductRepository(_context);
            var existingProduct = _context.Products.First();
            existingProduct.ProductName = "Updated Product";
            existingProduct.ProductDescription = "Updated Product Description";

            // Act
            var result = productRepository.UpdateProduct(existingProduct);
            var updatedProduct = _context.Products.FirstOrDefault(p => p.ProductId == existingProduct.ProductId);

            // Assert
            Assert.True(result);
            Assert.NotNull(updatedProduct);
            Assert.Equal("Updated Product", updatedProduct.ProductName);
            Assert.Equal("Updated Product Description", updatedProduct.ProductDescription);
        }

        [Fact]
        public void DeleteProduct_DeletesProductFromDatabase()
        {
            // Arrange
            var productRepository = new ProductRepository(_context);
            var existingProduct = _context.Products.First();

            // Act
            var result = productRepository.DeleteProduct(existingProduct.ProductId);
            var deletedProduct = _context.Products.Find(existingProduct.ProductId);

            // Assert
            Assert.True(result);
            Assert.Null(deletedProduct);
        }

        [Fact]
        public void GetProductByCategoryIdAndProductName_ReturnsCorrectProduct()
        {
            // Arrange
            var productRepository = new ProductRepository(_context);
            var category = _context.Categories.First();
            var expectedProduct = _context.Products.First(p => p.CategoryId == category.CategoryId);

            // Act
            var actualProduct = productRepository.GetProductByCategoryIdAndProductName(category.CategoryId, expectedProduct.ProductName);

            // Assert
            Assert.NotNull(actualProduct);
            Assert.Equal(expectedProduct.ProductId, actualProduct.ProductId);
            Assert.Equal(expectedProduct.ProductName, actualProduct.ProductName);
        }

        [Fact]
        public void GetProductByCategoryIdAndProductName_ReturnsNullForNonExistentProduct()
        {
            // Arrange
            var productRepository = new ProductRepository(_context);
            var category = _context.Categories.First();

            // Act
            var actualProduct = productRepository.GetProductByCategoryIdAndProductName(category.CategoryId, "NonExistentProduct");

            // Assert
            Assert.Null(actualProduct);
        }

        [Fact]
        public void GetProductByCategoryIdAndProductName_WithDifferentProductId_ReturnsCorrectProduct()
        {
            // Arrange
            var productRepository = new ProductRepository(_context);
            var category = _context.Categories.First();
            var productToCompare = _context.Products.First(p => p.CategoryId == category.CategoryId);
            var newProduct = new Product
            {
                ProductName = "New Test Product",
                ProductDescription = "New Test Description",
                CategoryId = category.CategoryId
            };

            _context.Products.Add(newProduct);
            _context.SaveChanges();

            // Act
            var actualProduct = productRepository.GetProductByCategoryIdAndProductName(productToCompare.ProductId, category.CategoryId, newProduct.ProductName);

            // Assert
            Assert.NotNull(actualProduct);
            Assert.Equal(newProduct.ProductId, actualProduct.ProductId);
            Assert.Equal(newProduct.ProductName, actualProduct.ProductName);
        }

        [Fact]
        public void GetProductByCategoryIdAndProductName_WithDifferentProductId_ReturnsNullForSameProductId()
        {
            // Arrange
            var productRepository = new ProductRepository(_context);
            var category = _context.Categories.First();
            var productToCompare = _context.Products.First(p => p.CategoryId == category.CategoryId);

            // Act
            var actualProduct = productRepository.GetProductByCategoryIdAndProductName(productToCompare.ProductId, category.CategoryId, productToCompare.ProductName);

            // Assert
            Assert.Null(actualProduct);
        }

        public void Dispose()
        {
            _context.Database.ExecuteSqlRaw("TRUNCATE TABLE Products");
            _context.Database.ExecuteSqlRaw("DELETE FROM Categories");
            _context.Dispose();
        }

        private void SeedData()
        {
            if (!_context.Products.Any())
            {
                var category = new Category
                {
                    Name = "Test Category",
                    Description = "Test Category Description",
                    FileName = "",
                };
                _context.Categories.Add(category);
                _context.SaveChanges();

                _context.Products.AddRange(
                    new Product
                    {
                        ProductName = "Test Product 1",
                        ProductDescription = "Test Product 1 Description",
                        CategoryId = category.CategoryId,
                        ProductPrice = 10M,
                        InStock = true,
                        IsActive = true,
                    },
                    new Product
                    {
                        ProductName = "Test Product 2",
                        ProductDescription = "Test Product 2 Description",
                        CategoryId = category.CategoryId,
                        ProductPrice = 11M,
                        InStock = true,
                        IsActive = true,
                    }
                );

                _context.SaveChanges();
            }
        }
    }
}
