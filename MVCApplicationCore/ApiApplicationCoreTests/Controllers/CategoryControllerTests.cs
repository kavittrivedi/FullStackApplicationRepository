using ApiApplicationCore.Controllers;
using ApiApplicationCore.Dtos;
using ApiApplicationCore.Models;
using ApiApplicationCore.Services.Contract;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiApplicationCoreTests.Controllers
{
    public class CategoryControllerTests
    {
        [Fact]
        public void GetAllCategories_ReturnsNotFound_WhenServiceFails()
        {
            // Arrange
            var expectedServiceResponse = new ServiceResponse<IEnumerable<CategoryDto>>
            {
                Message = "No record found!",
                Success = false
            };

            var mockCategoryService = new Mock<ICategoryService>();
            mockCategoryService.Setup(service => service.GetCategories())
                               .Returns(expectedServiceResponse);

            var controller = new CategoryController(mockCategoryService.Object);

            // Act
            var actual = controller.GetAllCategories() as ObjectResult;

            // Assert
            Assert.NotNull(actual);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actual);
            Assert.IsType<ServiceResponse<IEnumerable<CategoryDto>>>(notFoundResult.Value);
            Assert.False(((ServiceResponse<IEnumerable<CategoryDto>>)notFoundResult.Value).Success);
            mockCategoryService.Verify(service => service.GetCategories(), Times.Once);
        }

        [Fact]
        public void GetAllCategories_ReturnsOk_WhenServiceSucceeds()
        {
            // Arrange
            var categoryDtoList = new List<CategoryDto>
            {
               new  CategoryDto{ CategoryId=1, CategoryName="Category 1", CategoryDescription="Description 1" },
               new  CategoryDto{ CategoryId=1, CategoryName="Category 1", CategoryDescription="Description 1" },
            };

            var expectedServiceResponse = new ServiceResponse<IEnumerable<CategoryDto>>
            {
                Message = string.Empty,
                Success = true,
                Data = categoryDtoList
            };
            var mockCategoryService = new Mock<ICategoryService>();
            mockCategoryService.Setup(service => service.GetCategories())
                               .Returns(expectedServiceResponse);

            var controller = new CategoryController(mockCategoryService.Object);

            // Act
            var actual = controller.GetAllCategories() as ObjectResult;

            // Assert
            Assert.NotNull(actual);
            var okResult = Assert.IsType<OkObjectResult>(actual);
            Assert.IsType<ServiceResponse<IEnumerable<CategoryDto>>>(okResult.Value);
            Assert.True(((ServiceResponse<IEnumerable<CategoryDto>>)okResult.Value).Success);
            Assert.Equal(categoryDtoList, ((ServiceResponse<IEnumerable<CategoryDto>>)okResult.Value).Data);
            mockCategoryService.Verify(service => service.GetCategories(), Times.Once);
        }

        [Theory]
        [InlineData("Category already exists.")]
        [InlineData("Something went wrong, please try after sometime.")]
        public void AddCategory_ReturnsBadRequest_WhenServiceFails(string message)
        {
            // Arrange
            var categoryDto = new AddCategoryDto
            {
                CategoryName = "Test Category",
                CategoryDescription = "Test Description"
            };
            var expectedServiceResponse = new ServiceResponse<string>
            {
                Message = message,
                Success = false
            };
            var mockCategoryService = new Mock<ICategoryService>();
            mockCategoryService.Setup(service => service.AddCategory(It.IsAny<Category>()))
                               .Returns(expectedServiceResponse);

            var controller = new CategoryController(mockCategoryService.Object);

            // Act
            var result = controller.AddCategory(categoryDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<ServiceResponse<string>>(badRequestResult.Value);
            Assert.False(((ServiceResponse<string>)badRequestResult.Value).Success);
            mockCategoryService.Verify(service => service.AddCategory(It.IsAny<Category>()), Times.Once);
        }

        [Fact]
        public void AddCategory_ReturnsOk_WhenServiceSucceeds()
        {
            // Arrange
            var categoryDto = new AddCategoryDto
            {
                CategoryName = "Category 1",
                CategoryDescription = "Description 1"
            };
            var expectedServiceResponse = new ServiceResponse<string> 
            { 
                Message = "Category saved successfully.",
                Success = true 
            };
            var mockCategoryService = new Mock<ICategoryService>();
            mockCategoryService.Setup(service => service.AddCategory(It.IsAny<Category>()))
                               .Returns(expectedServiceResponse);

            var controller = new CategoryController(mockCategoryService.Object);

            // Act
            var actual = controller.AddCategory(categoryDto) as ObjectResult;

            // Assert
            Assert.NotNull(actual);
            var okResult = Assert.IsType<OkObjectResult>(actual);
            Assert.IsType<ServiceResponse<string>>(okResult.Value);
            Assert.True(((ServiceResponse<string>)okResult.Value).Success);
        }

        [Fact]
        public void RemoveCategory_ReturnsBadRequest_WhenIdIsZero()
        {
            // Arrange
            var id = 0;
            var mockCategoryService = new Mock<ICategoryService>();
            var controller = new CategoryController(mockCategoryService.Object);

            // Act
            var result = controller.RemoveCategory(id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Please enter proper data.", badRequestResult.Value);
        }

        [Fact]
        public void RemoveCategory_ReturnsBadRequest_WhenServiceFails()
        {
            // Arrange
            var id = 1; // Assuming valid ID
            var mockCategoryService = new Mock<ICategoryService>();
            mockCategoryService.Setup(service => service.RemoveCategory(id))
                               .Returns(new ServiceResponse<string> { Success = false });
            var controller = new CategoryController(mockCategoryService.Object);

            // Act
            var result = controller.RemoveCategory(id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<ServiceResponse<string>>(badRequestResult.Value);
            Assert.False(((ServiceResponse<string>)badRequestResult.Value).Success);
        }

        [Fact]
        public void RemoveCategory_ReturnsOk_WhenServiceSucceeds()
        {
            // Arrange
            var id = 1; // Assuming valid ID
            var mockCategoryService = new Mock<ICategoryService>();
            mockCategoryService.Setup(service => service.RemoveCategory(id))
                               .Returns(new ServiceResponse<string> { Success = true });
            var controller = new CategoryController(mockCategoryService.Object);

            // Act
            var result = controller.RemoveCategory(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<ServiceResponse<string>>(okResult.Value);
            Assert.True(((ServiceResponse<string>)okResult.Value).Success);
        }
    }
}
