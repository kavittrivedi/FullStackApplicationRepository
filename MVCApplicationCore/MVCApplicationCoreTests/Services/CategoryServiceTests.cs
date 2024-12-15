using Microsoft.AspNetCore.Http;
using Moq;
using MVCApplicationCore.Data.Contract;
using MVCApplicationCore.Models;
using MVCApplicationCore.Services.Implementation;

namespace MVCApplicationCoreTests.Services
{
    public class CategoryServiceTests
    {
        [Fact]
        public void GetCategories_ReturnEmptyList_WhenNoCategoriesExist()
        {
            // Arrange
            var mockCategoryReposiory = new Mock<ICategoryRepository>();
            mockCategoryReposiory.Setup(c => c.GetAll()).Returns<IEnumerable<Category>>(null);
            var target = new CategoryService(mockCategoryReposiory.Object);
            // Act
            var actual = target.GetCategories();

            // Assert
            Assert.NotNull(actual);
            Assert.Empty(actual);
            mockCategoryReposiory.Verify(c => c.GetAll(), Times.Once);
        }

        [Fact]
        public void GetCategories_ReturnsCategoriesWithDefaultImage_WhenCategoriesExistWithoutFileName()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { CategoryId = 1 , Name = "Category 1", FileName=string.Empty },
                new Category { CategoryId = 2 , Name = "Category 2", FileName=string.Empty },
            };

            var mockCategoryReposiory = new Mock<ICategoryRepository>();
            mockCategoryReposiory.Setup(c => c.GetAll()).Returns(categories);
            var target = new CategoryService(mockCategoryReposiory.Object);

            // Act
            var actual = target.GetCategories();

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(categories.Count(), actual.Count());
            foreach (var category in actual)
            {
                Assert.Equal("DefaultImage.png", category.FileName);
            }
            mockCategoryReposiory.Verify(c => c.GetAll(), Times.Once);
        }

        [Fact]
        public void GetCategories_ReturnsCategoriesWithoutModifyingFileName_WhenCategoriesExistWithFileName()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { CategoryId = 1, Name = "Category 1", FileName = "Image1.png" },
                new Category { CategoryId = 2, Name = "Category 2", FileName = "Image2.png" }
            };

            var mockCategoryRepository = new Mock<ICategoryRepository>();
            mockCategoryRepository.Setup(repo => repo.GetAll()).Returns(categories);
            var categoryService = new CategoryService(mockCategoryRepository.Object);

            // Act
            var result = categoryService.GetCategories();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(categories, result);
            mockCategoryRepository.Verify(repo => repo.GetAll(), Times.Once);
        }

        [Fact]
        public void GetPaginatedCategories_ReturnsCategories_WHenCategoriesExists()
        {
            // Arrange
            int page = 1;
            int pageSize = 10;

            var categories = new List<Category>
            {
                new Category{ CategoryId=1, Name="Category 1"  },
                new Category{ CategoryId=2, Name="Category 2"  },
            };

            var mockCategoryReposiory = new Mock<ICategoryRepository>();
            mockCategoryReposiory.Setup(c => c.GetPaginatedCategories(page, pageSize)).Returns(categories);
            var target = new CategoryService(mockCategoryReposiory.Object);
            // Act
            var actual = target.GetPaginatedCategories(page, pageSize);

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(categories, actual);
            mockCategoryReposiory.Verify(c => c.GetPaginatedCategories(page, pageSize), Times.Once);
        }

        [Fact]
        public void GetPaginatedCategories_ReturnsEmpty_WHenNoCategoriesExists()
        {
            // Arrange
            int page = 1;
            int pageSize = 10;

            var categories = new List<Category>();


            var mockCategoryReposiory = new Mock<ICategoryRepository>();
            mockCategoryReposiory.Setup(c => c.GetPaginatedCategories(page, pageSize)).Returns(categories);
            var target = new CategoryService(mockCategoryReposiory.Object);
            // Act
            var actual = target.GetPaginatedCategories(page, pageSize);

            // Assert
            Assert.NotNull(actual);
            Assert.Empty(actual);
            mockCategoryReposiory.Verify(c => c.GetPaginatedCategories(page, pageSize), Times.Once);
        }

        [Fact]
        public void TotalCategories_ReturnsZero_WhenNoCategoriesExist()
        {
            // Arrange
            var mockCategoryRepository = new Mock<ICategoryRepository>();
            mockCategoryRepository.Setup(repo => repo.TotalCategories()).Returns(0);
            var categoryService = new CategoryService(mockCategoryRepository.Object);

            // Act
            var result = categoryService.TotalCategories();

            // Assert
            Assert.Equal(0, result);
            mockCategoryRepository.Verify(repo => repo.TotalCategories(), Times.Once);
        }

        [Fact]
        public void TotalCategories_ReturnsTotalCount_WhenCategoriesExist()
        {
            // Arrange
            int totalCount = 5;
            var mockCategoryRepository = new Mock<ICategoryRepository>();
            mockCategoryRepository.Setup(repo => repo.TotalCategories()).Returns(totalCount);
            var categoryService = new CategoryService(mockCategoryRepository.Object);

            // Act
            var result = categoryService.TotalCategories();

            // Assert
            Assert.Equal(totalCount, result);
            mockCategoryRepository.Verify(repo => repo.TotalCategories(), Times.Once);
        }

        [Fact]
        public void AddCategory_ReturnsCategorySavedSuccessfully_WhenCategoryIsAddedSuccessFully()
        {
            // Arrange 
            var category = new Category { CategoryId = 1, Name = "Category 1" };
            var mockCategoryReposiory = new Mock<ICategoryRepository>();
            var file = new Mock<IFormFile>();
            mockCategoryReposiory.Setup(c => c.CategoryExists(category.Name)).Returns(false);
            mockCategoryReposiory.Setup(c => c.InsertCategory(category)).Returns(true);
            var target = new CategoryService(mockCategoryReposiory.Object);
            // Act
            var actial = target.AddCategory(category, file.Object);
            // Assert
            Assert.NotNull(actial);
            Assert.Equal("Category saved successfully.", actial);
            mockCategoryReposiory.Verify(c => c.CategoryExists(category.Name), Times.Once);
            mockCategoryReposiory.Verify(c => c.InsertCategory(category), Times.Once);
        }

        [Fact]
        public void AddCategory_ReturnsCategoryAlreadyExists_WhenCategoryAlreadyExists()
        {
            // Arrange
            var category = new Category { CategoryId = 1, Name = "Test Category" };
            var file = new Mock<IFormFile>();
            var mockCategoryRepository = new Mock<ICategoryRepository>();
            mockCategoryRepository.Setup(repo => repo.CategoryExists(category.Name)).Returns(true);
            var categoryService = new CategoryService(mockCategoryRepository.Object);

            // Act
            var result = categoryService.AddCategory(category, file.Object);

            // Assert
            Assert.Equal("Category already exists.", result);
            mockCategoryRepository.Verify(repo => repo.CategoryExists(category.Name), Times.Once);
        }

        [Fact]
        public void AddCategory_ReturnsSomethingWentWrongMessage_WhenCategoryInsertionFails()
        {
            // Arrange
            var category = new Category { CategoryId = 1, Name = "Test Category" };
            var file = new Mock<IFormFile>();
            var mockCategoryRepository = new Mock<ICategoryRepository>();
            mockCategoryRepository.Setup(repo => repo.CategoryExists(category.Name)).Returns(false);
            mockCategoryRepository.Setup(repo => repo.InsertCategory(category)).Returns(false);
            var categoryService = new CategoryService(mockCategoryRepository.Object);

            // Act
            var result = categoryService.AddCategory(category, file.Object);

            // Assert
            Assert.Equal("Something went wrong, please try after sometime.", result);
            mockCategoryRepository.Verify(repo => repo.CategoryExists(category.Name), Times.Once);
            mockCategoryRepository.Verify(repo => repo.InsertCategory(category), Times.Once);
        }

        [Fact]
        public void AddCategory_ReturnsCategorySavedSuccessfully_WhenFileIsNull()
        {
            // Arrange
            var category = new Category { CategoryId = 1, Name = "Test Category" };
            IFormFile file = null;
            var mockCategoryRepository = new Mock<ICategoryRepository>();
            mockCategoryRepository.Setup(repo => repo.CategoryExists(category.Name)).Returns(false);
            mockCategoryRepository.Setup(repo => repo.InsertCategory(category)).Returns(true);
            var categoryService = new CategoryService(mockCategoryRepository.Object);

            // Act
            var result = categoryService.AddCategory(category, file);

            // Assert
            Assert.Equal("Category saved successfully.", result);
            mockCategoryRepository.Verify(repo => repo.CategoryExists(category.Name), Times.Once);
            mockCategoryRepository.Verify(repo => repo.InsertCategory(category), Times.Once);
        }

        [Fact]
        public void AddCategory_ReturnsCategorySavedSuccessfully_WhenFileIsNotNullAndLengthIsZero()
        {
            // Arrange
            var category = new Category { CategoryId = 1, Name = "Test Category" };
            var file = new Mock<IFormFile>();
            file.Setup(f => f.Length).Returns(0);
            var mockCategoryRepository = new Mock<ICategoryRepository>();
            mockCategoryRepository.Setup(repo => repo.CategoryExists(category.Name)).Returns(false);
            mockCategoryRepository.Setup(repo => repo.InsertCategory(category)).Returns(true);
            var categoryService = new CategoryService(mockCategoryRepository.Object);

            // Act
            var result = categoryService.AddCategory(category, file.Object);

            // Assert
            Assert.Equal("Category saved successfully.", result);

            mockCategoryRepository.Verify(repo => repo.CategoryExists(category.Name), Times.Once);
            mockCategoryRepository.Verify(repo => repo.InsertCategory(category), Times.Once);
        }

        [Fact]
        public void ModifyCategory_ReturnsCategoryUpdatedSuccessfully_WhenCategoryIsUpdatedSuccessFully()
        {
            // Arrange
            var existingCategory = new Category() { CategoryId = 1, Name = "Category 1" };
            var updatedcategory = new Category() { CategoryId = 1, Name = "Category 1", Description = "Updated Description" };
            var mockCategoryReposiory = new Mock<ICategoryRepository>();
            mockCategoryReposiory.Setup(c => c.CategoryExists(updatedcategory.CategoryId, updatedcategory.Name)).Returns(false);
            mockCategoryReposiory.Setup(c => c.GetCategory(updatedcategory.CategoryId)).Returns(existingCategory);
            mockCategoryReposiory.Setup(c => c.UpdateCategory(existingCategory)).Returns(true);
            var target = new CategoryService(mockCategoryReposiory.Object);
            // Act
            var actual = target.ModifyCategory(updatedcategory);

            // Assert
            Assert.NotNull(actual);
            Assert.Equal("Category updated successfully.", actual);
            mockCategoryReposiory.Verify(c => c.CategoryExists(updatedcategory.CategoryId, updatedcategory.Name), Times.Once);
            mockCategoryReposiory.Verify(c => c.GetCategory(updatedcategory.CategoryId), Times.Once);
            mockCategoryReposiory.Verify(c => c.UpdateCategory(existingCategory), Times.Once);
        }

        [Fact]
        public void ModifyCategory_ReturnsCategoryAlreadyExists_WhenCategoryWithSameNameAlreadyExists()
        {
            // Arrange
            var category = new Category { CategoryId = 1, Name = "Existing Name", Description = "Updated Description" };
            var mockCategoryRepository = new Mock<ICategoryRepository>();
            mockCategoryRepository.Setup(repo => repo.CategoryExists(category.CategoryId, category.Name)).Returns(true);
            var categoryService = new CategoryService(mockCategoryRepository.Object);

            // Act
            var result = categoryService.ModifyCategory(category);

            // Assert
            Assert.Equal("Category already exists.", result);
            mockCategoryRepository.Verify(repo => repo.CategoryExists(category.CategoryId, category.Name), Times.Once);
        }

        [Fact]
        public void ModifyCategory_ReturnsSomethingWentWrongMessage_WhenCategoryUpdateFails()
        {
            // Arrange
            var category = new Category { CategoryId = 1, Name = "Updated Name", Description = "Updated Description" };
            var mockCategoryRepository = new Mock<ICategoryRepository>();
            mockCategoryRepository.Setup(repo => repo.CategoryExists(category.CategoryId, category.Name)).Returns(false);
            mockCategoryRepository.Setup(repo => repo.GetCategory(category.CategoryId)).Returns(category);
            mockCategoryRepository.Setup(repo => repo.UpdateCategory(category)).Returns(false);
            var categoryService = new CategoryService(mockCategoryRepository.Object);

            // Act
            var result = categoryService.ModifyCategory(category);

            // Assert
            Assert.Equal("Something went wrong, please try after sometime.", result);
            mockCategoryRepository.Verify(repo => repo.CategoryExists(category.CategoryId, category.Name), Times.Once);
            mockCategoryRepository.Verify(repo => repo.GetCategory(category.CategoryId), Times.Once);
            mockCategoryRepository.Verify(repo => repo.UpdateCategory(category), Times.Once);
        }

        [Fact]
        public void ModifyCategory_ReturnsSomethingWentWrongMessage_WhenCategoryDoesNotExist()
        {
            // Arrange
            var category = new Category { CategoryId = 1, Name = "Updated Name", Description = "Updated Description" };
            var mockCategoryRepository = new Mock<ICategoryRepository>();
            mockCategoryRepository.Setup(repo => repo.CategoryExists(category.CategoryId, category.Name)).Returns(false);
            mockCategoryRepository.Setup(repo => repo.GetCategory(category.CategoryId)).Returns<Category>(null);
            var categoryService = new CategoryService(mockCategoryRepository.Object);

            // Act
            var result = categoryService.ModifyCategory(category);

            // Assert
            Assert.Equal("Something went wrong, please try after sometime.", result);
            mockCategoryRepository.Verify(repo => repo.CategoryExists(category.CategoryId, category.Name), Times.Once);
            mockCategoryRepository.Verify(repo => repo.GetCategory(category.CategoryId), Times.Once);
        }

        [Fact]
        public void RemoveCategory_ReturnsCategoryDeletedSuccessfully_WhenCategoryIsDeletedSuccessfully()
        {
            // Arrange
            int categoryId = 1;
            var mockCategoryRepository = new Mock<ICategoryRepository>();
            mockCategoryRepository.Setup(repo => repo.DeleteCategory(categoryId)).Returns(true);
            var categoryService = new CategoryService(mockCategoryRepository.Object);

            // Act
            var result = categoryService.RemoveCategory(categoryId);

            // Assert
            Assert.Equal("Category deleted successfully.", result);
            mockCategoryRepository.Verify(repo => repo.DeleteCategory(categoryId), Times.Once);
        }

        [Fact]
        public void RemoveCategory_ReturnsSomethingWentWrongMessage_WhenCategoryDeletionFails()
        {
            // Arrange
            int categoryId = 1;
            var mockCategoryRepository = new Mock<ICategoryRepository>();
            mockCategoryRepository.Setup(repo => repo.DeleteCategory(categoryId)).Returns(false);
            var categoryService = new CategoryService(mockCategoryRepository.Object);

            // Act
            var result = categoryService.RemoveCategory(categoryId);

            // Assert
            Assert.Equal("Something went wrong, please try after sometimes.", result);
            mockCategoryRepository.Verify(repo => repo.DeleteCategory(categoryId), Times.Once);
        }
    }
}