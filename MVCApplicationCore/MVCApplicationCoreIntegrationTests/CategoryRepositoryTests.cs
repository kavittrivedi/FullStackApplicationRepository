using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MVCApplicationCore.Data;
using MVCApplicationCore.Data.Implementation;
using MVCApplicationCore.Models;

namespace MVCApplicationCoreIntegrationTests
{
    //[Collection("Non-Parallel Collection")]
    public class CategoryRepositoryTests : IDisposable
    {
        private readonly IAppDbContext _appDbContext;
        private readonly List<Category> _testCategories;
        public CategoryRepositoryTests()
        {
            var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json")
               .Build();

            var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
              .UseSqlServer(configuration.GetConnectionString("IntegrationTestDb"))
              .Options;

            _appDbContext = new AppDbContext(dbContextOptions);

            // Ensure database is created and seed data
            //_appDbContext.Database.EnsureDeleted();
            //_appDbContext.Database.EnsureCreated();

            // Seed data
            _testCategories = SeedDatabase();
        }

        [Fact]
        public void GetAll_ReturnsAllCategories()
        {
            // Arrange
            var categoryRepository = new CategoryRepository(_appDbContext);

            // Act
            IEnumerable<Category> categories = categoryRepository.GetAll();

            // Assert
            Assert.NotNull(categories);
            Assert.Equal(_testCategories.Count, categories.Count());
        }

        [Fact]
        public void TotalCategories_ReturnsCorrectCount()
        {
            // Arrange
            var categoryRepository = new CategoryRepository(_appDbContext);

            // Act
            int totalCategories = categoryRepository.TotalCategories();

            // Assert
            Assert.Equal(_testCategories.Count, totalCategories);
        }

        [Fact]
        public void GetPaginatedCategories_ReturnsCorrectCategories()
        {
            // Arrange
            var categoryRepository = new CategoryRepository(_appDbContext);
            int page = 1;
            int pageSize = 2;

            // Act
            IEnumerable<Category> paginatedCategories = categoryRepository.GetPaginatedCategories(page, pageSize);

            // Assert
            Assert.NotNull(paginatedCategories);
            Assert.Equal(pageSize, paginatedCategories.Count());
            Assert.Equal("Category 1", paginatedCategories.First().Name);
            Assert.Equal("Category 2", paginatedCategories.Last().Name);
        }

        [Fact]
        public void GetPaginatedCategories_ReturnsCorrectCategories_ForSecondPage()
        {
            // Arrange
            var categoryRepository = new CategoryRepository(_appDbContext);
            int page = 2;
            int pageSize = 2;

            // Act
            IEnumerable<Category> paginatedCategories = categoryRepository.GetPaginatedCategories(page, pageSize);

            // Assert
            Assert.NotNull(paginatedCategories);
            Assert.Equal(pageSize, paginatedCategories.Count());
            Assert.Equal("Category 3", paginatedCategories.First().Name);
            Assert.Equal("Category 4", paginatedCategories.Last().Name);
        }

        [Fact]
        public void InsertCategory_AddsCategoryToDatabase()
        {
            // Arrange
            var categoryRepository = new CategoryRepository(_appDbContext);
            var newCategory = new Category
            {
                Name = "New Category",
                Description = "New Category Description",
                FileName = ""
            };

            // Act
            bool result = categoryRepository.InsertCategory(newCategory);
            var insertedCategory = _appDbContext.Categories.FirstOrDefault(c => c.Name == "New Category");

            // Assert
            Assert.True(result);
            Assert.NotNull(insertedCategory);
            Assert.Equal("New Category", insertedCategory.Name);
            Assert.Equal("New Category Description", insertedCategory.Description);
        }

        [Fact]
        public void InsertCategory_ReturnsFalse_WhenCategoryIsNull()
        {
            // Arrange
            var categoryRepository = new CategoryRepository(_appDbContext);

            // Act
            bool result = categoryRepository.InsertCategory(null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void UpdateCategory_UpdatesCategoryInDatabase()
        {
            // Arrange
            var categoryRepository = new CategoryRepository(_appDbContext);
            var categoryToUpdate = _appDbContext.Categories.FirstOrDefault(c => c.Name == "Category 1");
            categoryToUpdate.Description = "Updated Description";

            // Act
            bool result = categoryRepository.UpdateCategory(categoryToUpdate);
            var updatedCategory = _appDbContext.Categories.FirstOrDefault(c => c.Name == "Category 1");

            // Assert
            Assert.True(result);
            Assert.NotNull(updatedCategory);
            Assert.Equal("Updated Description", updatedCategory.Description);
        }

        [Fact]
        public void UpdateCategory_ReturnsFalse_WhenCategoryIsNull()
        {
            // Arrange
            var categoryRepository = new CategoryRepository(_appDbContext);

            // Act
            bool result = categoryRepository.UpdateCategory(null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void DeleteCategory_DeletesCategoryInDatabase()
        {
            // Arrange
            var categoryRepository = new CategoryRepository(_appDbContext);
            var categoryToDelete = _appDbContext.Categories.FirstOrDefault(c => c.Name == "Category 1");

            // Act
            bool result = categoryRepository.DeleteCategory(categoryToDelete.CategoryId);
            var deletedCategory = _appDbContext.Categories.Find(categoryToDelete.CategoryId);

            // Assert
            Assert.True(result);
            Assert.Null(deletedCategory);
        }

        [Fact]
        public void DeleteCategory_ReturnsFalse_WhenCategoryDoesNotExist()
        {
            // Arrange
            var categoryRepository = new CategoryRepository(_appDbContext);

            // Act
            bool result = categoryRepository.DeleteCategory(0); // Assuming 0 is an ID that doesn't exist

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CategoryExists_ReturnsTrue_WhenCategoryExistsByName()
        {
            // Arrange
            var categoryRepository = new CategoryRepository(_appDbContext);

            // Act
            var result = categoryRepository.CategoryExists("Category 1");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CategoryExists_ReturnsFalse_WhenCategoryDoesNotExistByName()
        {
            // Arrange
            var categoryRepository = new CategoryRepository(_appDbContext);

            // Act
            var result = categoryRepository.CategoryExists("Nonexistent Category");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CategoryExists_ReturnsTrue_WhenCategoryExistsByIdAndName()
        {
            // Arrange
            var categoryRepository = new CategoryRepository(_appDbContext);
            var category = _appDbContext.Categories.FirstOrDefault(c => c.Name == "Category 1");

            // Act
            var result = categoryRepository.CategoryExists(category.CategoryId, "Category 1");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CategoryExists_ReturnsFalse_WhenCategoryDoesNotExistByIdAndName()
        {
            // Arrange
            var categoryRepository = new CategoryRepository(_appDbContext);

            // Act
            var result = categoryRepository.CategoryExists(999, "Nonexistent Category");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void GetCategoryProductCounts_ReturnsCorrectData()
        {
            // Arrange
            //var context = _fixture.Context;

            //// Act
            //var result = context.GetCategoryProductCounts();

            //// Assert
            //Assert.NotNull(result);
            //Assert.Equal(2, result.Count);
            //Assert.Contains(result, c => c.CategoryName == "Category 1" && c.ProductCount == 2);
            //Assert.Contains(result, c => c.CategoryName == "Category 2" && c.ProductCount == 1);


            // Arrange
            var categoryRepository = new CategoryRepository(_appDbContext);

            // Act
            IEnumerable<CategoryProductCount> categories = categoryRepository.GetCategoryProductCounts();

            // Assert
            Assert.NotNull(categories);
            Assert.Equal(_testCategories.Count, categories.Count());
        }

        public void Dispose()
        {
            // Cleanup database after each test
            //_appDbContext.Database.EnsureDeleted();
            //_appDbContext.Dispose();

            // Cleanup test data
            _appDbContext.Database.ExecuteSqlRaw("DELETE FROM Categories");
            _appDbContext.Dispose();
        }

        private List<Category> SeedDatabase()
        {
            var categories = new List<Category>
            {
                new Category { Name = "Category 1", Description = "Description 1", FileName="" },
                new Category { Name = "Category 2", Description = "Description 2", FileName="" },
                new Category { Name = "Category 3", Description = "Description 3", FileName="" },
                new Category { Name = "Category 4", Description = "Description 4", FileName="" },
                new Category { Name = "Category 5", Description = "Description 5", FileName="" },
                new Category { Name = "Category 6", Description = "Description 6", FileName="" },
            };
            _appDbContext.Categories.AddRange(categories);
            _appDbContext.SaveChanges();

            return categories;
        }
    }


    //[CollectionDefinition("Non-Parallel Collection", DisableParallelization = true)]
    //public class NonParallelCollection : ICollectionFixture<NonParallelCollectionFixture>
    //{
    //    // This class has no code, and is never created. Its purpose is
    //    // to be the place to apply [CollectionDefinition] and all
    //    // the ICollectionFixture<> interfaces.
    //}

    //public class NonParallelCollectionFixture
    //{
    //    // This class can contain setup and teardown logic for your collection if needed.
    //}
}