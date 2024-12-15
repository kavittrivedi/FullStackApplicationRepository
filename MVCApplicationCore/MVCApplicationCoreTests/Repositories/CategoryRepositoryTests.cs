using Microsoft.EntityFrameworkCore;
using Moq;
using MVCApplicationCore.Data;
using MVCApplicationCore.Data.Implementation;
using MVCApplicationCore.Models;
using NuGet.ContentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCApplicationCoreTests.Repositories
{
    public class CategoryRepositoryTests
    {

        [Fact]
        public void GetAll_ReturnsCategories_WhenCategoriesExist()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category{ CategoryId=1, Name="Category 1" },
                new Category{ CategoryId=2, Name="Category 2" },
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Category>>();
            // This line is setting up our fake database to act like a real one. When our program asks for all the categories,
            // our fake database will give it the list of categories we already set up. This helps us test our program's behavior
            // without needing a real database.
            mockDbSet.As<IQueryable<Category>>().Setup(c => c.GetEnumerator()).Returns(categories.GetEnumerator());

            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.Setup(c => c.Categories).Returns(mockDbSet.Object);
            var target = new CategoryRepository(mockAppDbContext.Object);
            // Act
            var actual = target.GetAll();
            // Assert
            Assert.NotNull(actual);
            Assert.Equal(categories.Count(), actual.Count());
            mockAppDbContext.Verify(c => c.Categories, Times.Once);
            mockDbSet.As<IQueryable<Category>>().Verify(c => c.GetEnumerator(), Times.Once);
        }

        [Fact]
        public void GetAll_ReturnsEmptyList_WhenNoCategoriesExist()
        {
            // Arrange
            var categories = new List<Category>().AsQueryable();

            var mockDbSet = new Mock<DbSet<Category>>();
            mockDbSet.As<IQueryable<Category>>().Setup(m => m.GetEnumerator()).Returns(categories.GetEnumerator());

            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.Setup(db => db.Categories).Returns(mockDbSet.Object);
            var categoryRepository = new CategoryRepository(mockAppDbContext.Object);

            // Act
            var result = categoryRepository.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            mockDbSet.As<IQueryable<Category>>().Verify(m => m.GetEnumerator(), Times.Once);
            mockAppDbContext.Verify(db => db.Categories, Times.Once);
        }

        [Fact]
        public void GetPaginatedCategories_ReturnsCorrectCategories_WhenCategoriesExist()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category{ CategoryId=1, Name="Category 1" },
                new Category{ CategoryId=2, Name="Category 2" },
                new Category{ CategoryId=3, Name="Category 3" },
            }.AsQueryable();
            var mockDbSet = new Mock<DbSet<Category>>();
            // This line sets up the Provider property of the mocked DbSet<Category> to return the Provider property of the categories IQueryable collection when accessed.
            // The Provider property is used by LINQ query execution.
            mockDbSet.As<IQueryable<Category>>().Setup(c => c.Provider).Returns(categories.Provider);
            // This line sets up the Expression property of the mocked DbSet<Category> to return the Expression property of the categories IQueryable collection when accessed.
            // The Expression property represents the LINQ expression tree associated with the IQueryable collection.
            mockDbSet.As<IQueryable<Category>>().Setup(c => c.Expression).Returns(categories.Expression);

            // By setting up these properties, we ensure that when methods or properties of the DbSet<Category> are invoked in the unit test,
            // they behave as expected, providing access to the LINQ query provider and expression. This allows us to mimic the behavior of a
            // real database context in our unit tests.


            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.Setup(c => c.Categories).Returns(mockDbSet.Object);
            var target = new CategoryRepository(mockAppDbContext.Object);

            // Act
            var actual = target.GetPaginatedCategories(1, 2);

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(2, actual.Count());
            mockDbSet.As<IQueryable<Category>>().Verify(c => c.Provider, Times.Once);
            mockDbSet.As<IQueryable<Category>>().Verify(c => c.Expression, Times.Once);
            mockAppDbContext.Verify(c => c.Categories, Times.Once);
        }

        [Fact]
        public void InsertCategory_ReturnTrue()
        {
            // Arrange
            var mockDbSet = new Mock<DbSet<Category>>();
            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.SetupGet(c => c.Categories).Returns(mockDbSet.Object);
            mockAppDbContext.Setup(c => c.SaveChanges()).Returns(1);
            var target = new CategoryRepository(mockAppDbContext.Object);
            var category = new Category { CategoryId = 1, Name = "Category 1" };
            // Act
            var actual = target.InsertCategory(category);

            // Assert
            Assert.True(actual);
            mockDbSet.Verify(c => c.Add(category), Times.Once);
            mockAppDbContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Fact]
        public void InsertCategory_ReturnsFalse()
        {
            // Arrange
            var mockAppDbContext = new Mock<IAppDbContext>();
            var categoryRepository = new CategoryRepository(mockAppDbContext.Object);

            // Act
            var result = categoryRepository.InsertCategory(null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void UpdateCategory_ReturnTrue()
        {
            // Arrange
            var mockDbSet = new Mock<DbSet<Category>>();
            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.SetupGet(c => c.Categories).Returns(mockDbSet.Object);
            mockAppDbContext.Setup(c => c.SaveChanges()).Returns(1);
            var target = new CategoryRepository(mockAppDbContext.Object);
            var category = new Category { CategoryId = 1, Name = "Category 1" };

            // Act
            var actual = target.UpdateCategory(category);

            // Assert
            Assert.True(actual);
            mockDbSet.Verify(c => c.Update(category), Times.Once);
            mockAppDbContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Fact]
        public void UpdateCategory_ReturnsFalse()
        {
            // Arrange
            var mockAppDbContext = new Mock<IAppDbContext>();
            var categoryRepository = new CategoryRepository(mockAppDbContext.Object);

            // Act
            var actual = categoryRepository.UpdateCategory(null);

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void DeleteCategory_ReturnsTrue()
        {
            // Arrange
            var id = 1;
            var category = new Category { CategoryId = id, Name = "Category 1" };
            var categories = new List<Category> { category };
            var mockDbSet = new Mock<DbSet<Category>>();
            mockDbSet.Setup(c => c.Find(id)).Returns(category);

            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.SetupGet(c => c.Categories).Returns(mockDbSet.Object);
            mockAppDbContext.Setup(c => c.SaveChanges()).Returns(1);
            var target = new CategoryRepository(mockAppDbContext.Object);
            // Act
            var actual = target.DeleteCategory(id);

            // Assert
            Assert.True(actual);
            mockDbSet.Verify(c => c.Find(id), Times.Once);
            mockAppDbContext.VerifyGet(c => c.Categories, Times.Exactly(2));
            mockAppDbContext.Verify(c => c.SaveChanges(), Times.Once);
            mockDbSet.Verify(c => c.Remove(category), Times.Once);
        }

        [Fact]
        public void DeleteCategory_ReturnsFalse_WhenCategoryDoesNotExist()
        {
            // Arrange
            var categories = new List<Category>();

            var mockDbSet = new Mock<DbSet<Category>>();
            mockDbSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns<object[]>(ids => categories.Find(c => c.CategoryId == (int)ids[0]));

            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.SetupGet(db => db.Categories).Returns(mockDbSet.Object);

            var target = new CategoryRepository(mockAppDbContext.Object);

            int categoryId = 1;

            // Act
            var actual = target.DeleteCategory(categoryId);

            // Assert
            Assert.False(actual);
            mockDbSet.Verify(db => db.Remove(It.IsAny<Category>()), Times.Never);
            mockAppDbContext.Verify(db => db.SaveChanges(), Times.Never);
        }

        [Fact]
        public void CategoryExists_ReturnsTrue_WhenCategoryExists()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category{CategoryId=1, Name="Category 1" },
                new Category{CategoryId=2, Name="Category 2" },
            }.AsQueryable();
            var mockDbSet = new Mock<DbSet<Category>>();
            // This line sets up the Provider property of the mocked DbSet<Category> to return the Provider property of the categories IQueryable collection when accessed.
            // The Provider property is used by LINQ query execution.
            mockDbSet.As<IQueryable<Category>>().Setup(c => c.Provider).Returns(categories.Provider);
            // This line sets up the Expression property of the mocked DbSet<Category> to return the Expression property of the categories IQueryable collection when accessed.
            // The Expression property represents the LINQ expression tree associated with the IQueryable collection.
            mockDbSet.As<IQueryable<Category>>().Setup(c => c.Expression).Returns(categories.Expression);

            // By setting up these properties, we ensure that when methods or properties of the DbSet<Category> are invoked in the unit test,
            // they behave as expected, providing access to the LINQ query provider and expression. This allows us to mimic the behavior of a
            // real database context in our unit tests.
            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.SetupGet(c => c.Categories).Returns(mockDbSet.Object);
            var target = new CategoryRepository(mockAppDbContext.Object);
            var name = "Category 1";

            // Act
            var actual = target.CategoryExists(name);
            
            // Assert
            Assert.True(actual);
            mockDbSet.As<IQueryable<Category>>().Verify(c => c.Provider, Times.Once);
            mockDbSet.As<IQueryable<Category>>().Verify(c => c.Expression, Times.Once);
            mockAppDbContext.VerifyGet(c => c.Categories, Times.Once);
        }

        [Fact]
        public void CategoryExists_ReturnsFalse_WhenCategoryDoesNotExist()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { CategoryId = 1, Name = "Category 1" },
                new Category { CategoryId = 2, Name = "Category 2" }
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Category>>();
            mockDbSet.As<IQueryable<Category>>().Setup(m => m.Provider).Returns(categories.Provider);
            mockDbSet.As<IQueryable<Category>>().Setup(m => m.Expression).Returns(categories.Expression);

            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.Setup(db => db.Categories).Returns(mockDbSet.Object);

            var target = new CategoryRepository(mockAppDbContext.Object);

            string categoryName = "Category 3"; // Category with name "Category 3" does not exist

            // Act
            var actual = target.CategoryExists(categoryName);

            // Assert
            Assert.False(actual);

            // Verification
            mockDbSet.As<IQueryable<Category>>().Verify(m => m.Provider, Times.Once);
            mockDbSet.As<IQueryable<Category>>().Verify(m => m.Expression, Times.Once);
            mockAppDbContext.Verify(db => db.Categories, Times.Once);
        }

        [Fact]
        public void CategoryExists_ReturnsTrue_WhenCategoryExistsWithDifferentIdAndSameName()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { CategoryId = 1, Name = "Category 1" },
                new Category { CategoryId = 2, Name = "Category 2" }
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Category>>();
            mockDbSet.As<IQueryable<Category>>().Setup(m => m.Provider).Returns(categories.Provider);
            mockDbSet.As<IQueryable<Category>>().Setup(m => m.Expression).Returns(categories.Expression);

            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.Setup(db => db.Categories).Returns(mockDbSet.Object);

            var categoryRepository = new CategoryRepository(mockAppDbContext.Object);

            int categoryId = 2; // Existing category's id
            string categoryName = "Category 1"; // Category with same name but different id exists

            // Act
            var actual = categoryRepository.CategoryExists(categoryId, categoryName);

            // Assert
            Assert.True(actual);
            mockDbSet.As<IQueryable<Category>>().Verify(m => m.Provider, Times.Once);
            mockDbSet.As<IQueryable<Category>>().Verify(m => m.Expression, Times.Once);
            mockAppDbContext.Verify(db => db.Categories, Times.Once);
        }

        [Fact]
        public void CategoryExists_ReturnsFalse_WhenCategoryDoesNotExistOrHasSameId()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { CategoryId = 1, Name = "Category 1" },
                new Category { CategoryId = 2, Name = "Category 2" }
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Category>>();
            mockDbSet.As<IQueryable<Category>>().Setup(m => m.Provider).Returns(categories.Provider);
            mockDbSet.As<IQueryable<Category>>().Setup(m => m.Expression).Returns(categories.Expression);

            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.Setup(db => db.Categories).Returns(mockDbSet.Object);

            var target = new CategoryRepository(mockAppDbContext.Object);

            int categoryId = 1; // Existing category's id
            string categoryName = "Category 3"; // Non-existing category's name

            // Act
            var actual = target.CategoryExists(categoryId, categoryName);

            // Assert
            Assert.False(actual);
            mockDbSet.As<IQueryable<Category>>().Verify(m => m.Provider, Times.Once);
            mockDbSet.As<IQueryable<Category>>().Verify(m => m.Expression, Times.Once);
            mockAppDbContext.Verify(db => db.Categories, Times.Once);
        }
    }
}
