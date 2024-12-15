using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using MVCApplicationCore.Controllers;
using MVCApplicationCore.Models;
using MVCApplicationCore.Services.Contract;
using MVCApplicationCore.ViewModels;
using System.Net;

namespace MVCApplicationCoreTests.Controllers
{
    public class CategoryControllerTests
    {
        [Fact]
        public void Index1_ReturnsViewWithCategories_WhenCategoriesExists()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category{ CategoryId=1, Name="Category 1" },
                new Category{ CategoryId=2, Name="Category 2" },
            };

            var mockCategoryService = new Mock<ICategoryService>();
            var target = new CategoryController(mockCategoryService.Object);
            mockCategoryService.Setup(c => c.GetCategories()).Returns(categories);

            // Act
            var actual = target.Index1() as ViewResult;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal("CategoryList", actual.ViewName);
            Assert.NotNull(actual.Model);
            Assert.Equal(categories, actual.Model);
            mockCategoryService.Verify(c => c.GetCategories(), Times.Once);
        }

        [Fact]
        public void Index1_ReturnsViewWithEmptyList_WhenNoCategoriesExist()
        {
            // Arrange
            var mockCategoryService = new Mock<ICategoryService>();
            var target = new CategoryController(mockCategoryService.Object);
            mockCategoryService.Setup(c => c.GetCategories()).Returns(new List<Category>());

            // Act
            var actual = target.Index1() as ViewResult;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal("CategoryList", actual.ViewName);
            Assert.NotNull(actual.Model);
            Assert.Equal(new List<Category>(), actual.Model);
            mockCategoryService.Verify(c => c.GetCategories(), Times.Once);
        }

        [Fact]
        public void Index1_ReturnsViewWithEmptyList_WHenCategoriesIsNull()
        {
            // Arrange
            List<Category> categories = new List<Category>();
            var mockCategoryService = new Mock<ICategoryService>();
            var target = new CategoryController(mockCategoryService.Object);
            mockCategoryService.Setup(c => c.GetCategories()).Returns((List<Category>)null);

            // Act
            var actual = target.Index1() as ViewResult;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal("CategoryList", actual.ViewName);
            Assert.NotNull(actual.Model);
            Assert.Equal(categories, actual.Model);
            mockCategoryService.Verify(c => c.GetCategories(), Times.Once);
        }

        [Fact]
        public void Create_ReturnsView()
        {
            // Arrange
            var mockCategoryService = new Mock<ICategoryService>();
            var target = new CategoryController(mockCategoryService.Object);

            // Act
            var actual = target.Create() as ViewResult;

            // Assert
            Assert.NotNull(actual);
        }

        [Fact]
        public void Create_ReturnsView_withModelStateIsInvalid()
        {
            // Arrange
            var categoryViewModel = new CategoryViewModel { Description = "Category 1" };
            var mockCategoryService = new Mock<ICategoryService>();
            var target = new CategoryController(mockCategoryService.Object);

            target.ModelState.AddModelError("Name", "Name is required.");

            // Act
            var actual = target.Create(categoryViewModel) as ViewResult;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(categoryViewModel, actual.Model);
            Assert.False(target.ModelState.IsValid);
        }

        [Fact]
        public void Create_ReturnsRedirectToActionResult_WhenCategoryAddedSuccessfully()
        {
            // Arrange
            var categoryViewModel = new CategoryViewModel { Name = "Category 1", Description = "Description 1" };
            var category = new Category()
            {
                Name = categoryViewModel.Name,
                Description = categoryViewModel.Description
            };

            var mockCategoryService = new Mock<ICategoryService>();
            var mockTepDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTepDataProvider.Object);
            var target = new CategoryController(mockCategoryService.Object)
            {
                TempData = tempData
            };

            mockCategoryService.Setup(c => c.AddCategory(It.IsAny<Category>(), It.IsAny<IFormFile>())).Returns("Category saved successfully.");

            // Act
            var actual = target.Create(categoryViewModel) as RedirectToActionResult;

            // Assert
            Assert.NotNull(actual);
            Assert.True(target.ModelState.IsValid);
            Assert.Equal("Index", actual.ActionName);
            mockCategoryService.Verify(c => c.AddCategory(It.IsAny<Category>(), It.IsAny<IFormFile>()), Times.Once);
        }

        [Theory]
        [InlineData("Category already exists.")]
        [InlineData("Something went wrong, please try after sometime.")]
        public void Create_SetsErrorMessageInTempData_WhenCategoryAdditionFails(string errorMessage)
        {
            // Arrange
            var categoryViewModel = new CategoryViewModel { Name = "Test Category", Description = "Test Description" };
            var mockCategoryService = new Mock<ICategoryService>();
            mockCategoryService.Setup(svc => svc.AddCategory(It.IsAny<Category>(), It.IsAny<IFormFile>())).Returns(errorMessage);
            var tempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), tempDataProvider.Object);

            var target = new CategoryController(mockCategoryService.Object)
            {
                TempData = tempData
            };

            // Act
            var result = target.Create(categoryViewModel) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(categoryViewModel, result.Model);
            Assert.Equal(errorMessage, target.TempData["ErrorMessage"]);
        }

        [Fact]
        public void Edit_ReturnsNotFound_WhenCategoryNotFound()
        {
            // Arrange
            var mockCategoryService = new Mock<ICategoryService>();
            mockCategoryService.Setup(c => c.GetCategory(1)).Returns<Category>(null);
            var target = new CategoryController(mockCategoryService.Object);
            // Act
            var actual = target.Edit(1) as NotFoundResult;
            // Assert
            Assert.NotNull(actual);
            Assert.Equal((int)HttpStatusCode.NotFound, actual.StatusCode);
            mockCategoryService.Verify(c => c.GetCategory(1), Times.Once);
        }

        [Fact]
        public void Edit_ReturnsViewWithCategory_WhenCategoryExists()
        {
            // Arrange
            var categoryId = 1;
            var category = new Category { CategoryId = categoryId, Name = "Category 1" };
            var mockCategoryService = new Mock<ICategoryService>();
            mockCategoryService.Setup(svc => svc.GetCategory(categoryId)).Returns(category);

            var target = new CategoryController(mockCategoryService.Object);

            // Act
            var result = target.Edit(categoryId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(category, result.Model);
        }

        [Fact]
        public void Edit_ReturnsRedirectToActionResult_WhenCategoryModificationSucceeds()
        {
            // Arrange
            var category = new Category { CategoryId = 1, Name = "Test Category" };
            var mockCategoryService = new Mock<ICategoryService>();
            mockCategoryService.Setup(svc => svc.ModifyCategory(category)).Returns("Success");

            var tempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), tempDataProvider.Object);

            var target = new CategoryController(mockCategoryService.Object)
            {
                TempData = tempData
            };

            // Act
            var result = target.Edit(category);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Null(redirectToActionResult.ControllerName);
        }

        [Theory]
        [InlineData("Category already exists.")]
        [InlineData("Something went wrong, please try after sometime.")]
        public void Edit_SetErrorMessageInTempData_WhenModificationFails(string errorMessage)
        {
            // Arrange
            var categoryId = 1;
            var category = new Category()
            {
                CategoryId = categoryId,
                Name = "Category 1",
                Description = "Description 1"
            };
            var mockCategoryService = new Mock<ICategoryService>();
            var mockTepDataProvider = new Mock<ITempDataProvider>();
            mockCategoryService.Setup(c => c.ModifyCategory(category)).Returns(errorMessage);
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTepDataProvider.Object);
            var target = new CategoryController(mockCategoryService.Object)
            {
                TempData = tempData
            };

            // Act
            var actual = target.Edit(category) as ViewResult;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(category, actual.Model);
            mockCategoryService.Verify(c => c.ModifyCategory(category), Times.Once);
        }

        [Fact]
        public void Details_ReturnsNotFound_WhenCategoryNotFound()
        {
            // Arrange
            int categoryId = 1;
            var mockCategoryService = new Mock<ICategoryService>();
            mockCategoryService.Setup(svc => svc.GetCategory(categoryId)).Returns<Category>(null);
            var target = new CategoryController(mockCategoryService.Object);

            // Act
            var result = target.Details(categoryId) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            mockCategoryService.Verify(svc => svc.GetCategory(categoryId), Times.Once);
        }

        [Fact]
        public void Details_ReturnsViewWithCategory_WhenCategoryExists()
        {
            // Arrange
            int categoryId = 1;
            var category = new Category { CategoryId = categoryId, Name = "Test Category" };
            var mockCategoryService = new Mock<ICategoryService>();
            mockCategoryService.Setup(svc => svc.GetCategory(categoryId)).Returns(category);
            var controller = new CategoryController(mockCategoryService.Object);

            // Act
            var result = controller.Details(categoryId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(category, result.Model);
            mockCategoryService.Verify(svc => svc.GetCategory(categoryId), Times.Once);
        }

        [Fact]
        public void Delete_ReturnsNotFound_WhenCategoryNotFound()
        {
            // Arrange
            int categoryId = 1;
            var mockCategoryService = new Mock<ICategoryService>();
            mockCategoryService.Setup(svc => svc.GetCategory(categoryId)).Returns<Category>(null);
            var target = new CategoryController(mockCategoryService.Object);

            // Act
            var result = target.Delete(categoryId) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            mockCategoryService.Verify(svc => svc.GetCategory(categoryId), Times.Once);
        }

        [Fact]
        public void Delete_ReturnsViewWithCategory_WhenCategoryExists()
        {
            // Arrange
            int categoryId = 1;
            var category = new Category { CategoryId = categoryId, Name = "Test Category" };
            var mockCategoryService = new Mock<ICategoryService>();
            mockCategoryService.Setup(svc => svc.GetCategory(categoryId)).Returns(category);
            var target = new CategoryController(mockCategoryService.Object);

            // Act
            var result = target.Delete(categoryId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(category, result.Model);
            mockCategoryService.Verify(svc => svc.GetCategory(categoryId), Times.Once);
        }

        [Fact]
        public void DeleteConfirmed_RedirectsToIndex_WhenCategoryRemovedSuccessfully()
        {
            // Arrange
            int categoryId = 1;
            var mockCategoryService = new Mock<ICategoryService>();
            mockCategoryService.Setup(svc => svc.RemoveCategory(categoryId)).Returns("Category deleted successfully.");
            var tempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), tempDataProvider.Object);

            var target = new CategoryController(mockCategoryService.Object)
            {
                TempData = tempData
            };

            // Act
            var result = target.DeleteConfirmed(categoryId) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            mockCategoryService.Verify(svc => svc.RemoveCategory(categoryId), Times.Once);
        }

        [Fact]
        public void DeleteConfirmed_SetsSuccessMessageInTempData_WhenCategoryRemovedSuccessfully()
        {
            // Arrange
            int categoryId = 1;
            var mockCategoryService = new Mock<ICategoryService>();
            mockCategoryService.Setup(svc => svc.RemoveCategory(categoryId)).Returns("Category deleted successfully.");
            var tempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), tempDataProvider.Object);

            var target = new CategoryController(mockCategoryService.Object)
            {
                TempData = tempData
            };

            // Act
            var result = target.DeleteConfirmed(categoryId) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Category deleted successfully.", target.TempData["SuccessMessage"]);
            mockCategoryService.Verify(svc => svc.RemoveCategory(categoryId), Times.Once);
        }

        [Fact]
        public void DeleteConfirmed_SetsErrorMessageInTempData_WhenCategoryRemovalFails()
        {
            // Arrange
            int categoryId = 1;
            var mockCategoryService = new Mock<ICategoryService>();
            mockCategoryService.Setup(svc => svc.RemoveCategory(categoryId)).Returns("Error: Category not found.");
            var tempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), tempDataProvider.Object);

            var target = new CategoryController(mockCategoryService.Object)
            {
                TempData = tempData
            };

            // Act
            var result = target.DeleteConfirmed(categoryId) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Error: Category not found.", target.TempData["ErrorMessage"]);
            mockCategoryService.Verify(svc => svc.RemoveCategory(categoryId), Times.Once);
        }
    }
}