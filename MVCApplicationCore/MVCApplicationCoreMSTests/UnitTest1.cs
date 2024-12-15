using Microsoft.AspNetCore.Mvc;
using Moq;
using MVCApplicationCore.Controllers;
using MVCApplicationCore.Models;
using MVCApplicationCore.Services.Contract;

namespace MVCApplicationCoreMSTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
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
            Assert.IsNotNull(actual);
            Assert.AreEqual("CategoryList", actual.ViewName);
            Assert.IsNotNull(actual.Model);
            Assert.AreEqual(categories, actual.Model);
            mockCategoryService.Verify(c => c.GetCategories(), Times.Once);
        }

        [TestMethod]
        public void TestMethod2()
        {
        }
    }
}