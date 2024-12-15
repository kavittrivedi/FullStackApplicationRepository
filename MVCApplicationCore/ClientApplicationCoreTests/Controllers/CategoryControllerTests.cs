using AutoFixture;
using ClientApplicationCore.Controllers;
using ClientApplicationCore.Infrastructure;
using ClientApplicationCore.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using System.Net;

namespace ClientApplicationCoreTests.Controllers
{
    public class CategoryControllerTests
    {
        [Fact]
        public void Index_ReturnsCategories()
        {
            // Arrange
            var expectedCategories = new List<CategoryViewModel>
            {
                new CategoryViewModel{ CategoryId= 1, CategoryName="Category 1" },
                new CategoryViewModel{ CategoryId= 2, CategoryName="Category 2" },
            };

            var expectedResponse = new ServiceResponse<IEnumerable<CategoryViewModel>>
            {
                Success = true,
                Data = expectedCategories
            };
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var mockHttpContext = new Mock<HttpContext>();
            var mockHttpRequest = new Mock<HttpRequest>();
            mockHttpClientService
                .Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<CategoryViewModel>>>(It.IsAny<string>(), HttpMethod.Get, mockHttpContext.Object.Request, null, 60))
                .Returns(expectedResponse);

            var target = new CategoryController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };

            // Act
            var actual = target.Index() as ViewResult;

            // Assert
            Assert.NotNull(actual);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService
                .Verify(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<CategoryViewModel>>>(It.IsAny<string>(), HttpMethod.Get, mockHttpContext.Object.Request, null, 60), Times.Once);

        }

        [Fact]
        public void Index_ReturnsCategoriesWithFixture()
        {
            // Arrange
            //var expectedCategories = new List<CategoryViewModel>
            //{
            //    new CategoryViewModel{ CategoryId= 1, CategoryName="Category 1" },
            //    new CategoryViewModel{ CategoryId= 2, CategoryName="Category 2" },
            //};
            var fixture = new Fixture();
            var expectedCategories = fixture.CreateMany<CategoryViewModel>();

            var expectedResponse = new ServiceResponse<IEnumerable<CategoryViewModel>>
            {
                Success = true,
                Data = expectedCategories
            };
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var mockHttpContext = new Mock<HttpContext>();
            var mockHttpRequest = new Mock<HttpRequest>();
            mockHttpClientService
                .Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<CategoryViewModel>>>(It.IsAny<string>(), HttpMethod.Get, mockHttpContext.Object.Request, null, 60))
                .Returns(expectedResponse);

            var target = new CategoryController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };

            // Act
            var actual = target.Index() as ViewResult;

            // Assert
            Assert.NotNull(actual);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService
                .Verify(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<CategoryViewModel>>>(It.IsAny<string>(), HttpMethod.Get, mockHttpContext.Object.Request, null, 60), Times.Once);

        }

        [Fact]
        public void Index_ReturnsEmptyView_OnException()
        {
            // Arrange
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockHttpContext = new Mock<HttpContext>();
            var exception = new Exception("Object reference not set to an instance of an object");
            
            mockHttpClientService.Setup(x => x.ExecuteApiRequest<ServiceResponse<IEnumerable<CategoryViewModel>>>(
                It.IsAny<string>(), HttpMethod.Get, mockHttpContext.Object.Request, null, 60)).Throws(exception);

            var configurationMock = new Mock<IConfiguration>();

            var target = new CategoryController(mockHttpClientService.Object, configurationMock.Object);

            // Act
            var result = target.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<CategoryViewModel>>(viewResult.Model);
            Assert.Empty(model);
            
        }

        [Fact]
        public void Create__RedirectToActionResult_WhenCategorySavedSuccessFully()
        {
            // Arrange
            //var viewModel = new AddCategoryViewModel { CategoryName = "Category 1", CategoryDescription = "Description 1" };
            var fixture = new Fixture();
            var viewModel = fixture.Build<AddCategoryViewModel>().With(c => c.CategoryName).Without(c => c.CategoryDescription).Create();

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var successMessage = "Category saved successfully";
            var expectedServiceResonse = new ServiceResponse<string>
            {
                Message = successMessage,
            };

            var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResonse))
            };
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpClientService.Setup(c => c.PostHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>())).Returns(expectedResponse);
            var mockTepDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTepDataProvider.Object);
            var target = new CategoryController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };

            // Act
            var actual = target.Create(viewModel) as RedirectToActionResult;

            // Assert
            Assert.NotNull(actual);
            Assert.True(target.ModelState.IsValid);
            Assert.Equal("Index", actual.ActionName);
            Assert.Equal(successMessage, target.TempData["SuccessMessage"]);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.PostHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>()), Times.Once);
        }

        [Fact]
        public void Create__RedirectToActionResult_WhenCategorySavedSuccessFullyUsingFixture()
        {
            // Arrange
            //var viewModel = new AddCategoryViewModel { CategoryName = "Category 1", CategoryDescription = "Description 1" };

            var fixture = new Fixture();
            var viewModel = fixture.Create<AddCategoryViewModel>();

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var successMessage = "Category saved successfully";
            var expectedServiceResonse = new ServiceResponse<string>
            {
                Message = successMessage,
            };

            var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResonse))
            };
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpClientService.Setup(c => c.PostHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>())).Returns(expectedResponse);
            var mockTepDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTepDataProvider.Object);
            var target = new CategoryController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };

            // Act
            var actual = target.Create(viewModel) as RedirectToActionResult;

            // Assert
            Assert.NotNull(actual);
            Assert.True(target.ModelState.IsValid);
            Assert.Equal("Index", actual.ActionName);
            Assert.Equal(successMessage, target.TempData["SuccessMessage"]);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.PostHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>()), Times.Once);
        }

        [Fact]
        public void Create__RedirectToActionResult_WhenCategorySavedSuccessFullyUsingFixtureBuild()
        {
            // Arrange
            //var viewModel = new AddCategoryViewModel { CategoryName = "Category 1", CategoryDescription = "Description 1" };

            var fixture = new Fixture();
            var viewModel = fixture.Build<AddCategoryViewModel>().With(c => c.CategoryName).Without(c => c.CategoryDescription).Create();

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var successMessage = "Category saved successfully";
            var expectedServiceResonse = new ServiceResponse<string>
            {
                Message = successMessage,
            };

            var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResonse))
            };
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpClientService.Setup(c => c.PostHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>())).Returns(expectedResponse);
            var mockTepDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTepDataProvider.Object);
            var target = new CategoryController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };

            // Act
            var actual = target.Create(viewModel) as RedirectToActionResult;

            // Assert
            Assert.NotNull(actual);
            Assert.True(target.ModelState.IsValid);
            Assert.Equal("Index", actual.ActionName);
            Assert.Equal(successMessage, target.TempData["SuccessMessage"]);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.PostHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>()), Times.Once);
        }

        [Fact]
        public void Details_RedirectToAction_WhenCategoryNotExists()
        {
            // Arrange
            int categoryId = 1;
            var expectedSuccessResponseContent = new ServiceResponse<UpdateCategoryViewModel>
            {
                Success = false,
                Message = string.Empty,
            };

            var expectedSuccessResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = null
            };
            var mockHttpContext = new Mock<HttpContext>();
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockHttpClientService.Setup(c => c.GetHttpResponseMessage<UpdateCategoryViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedSuccessResponse);
            var mockTepDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTepDataProvider.Object);
            var target = new CategoryController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object },
                TempData = tempData,
            };

            // Act
            var actual = target.Details(categoryId) as RedirectToActionResult;
            // Assert
            Assert.NotNull(actual);
            Assert.Equal("Index", actual.ActionName);
            mockHttpClientService.Verify(c => c.GetHttpResponseMessage<UpdateCategoryViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);
        }

        [Fact]
        public void Details_ReturnViewWithCategory_WhenCategoryExists()
        {
            // Arrange
            int categoryId = 1;
            var updateCategoryViewModel = new UpdateCategoryViewModel
            {
                CategoryId = categoryId,
                CategoryName = "Category 1",
                CategoryDescription = "Description 1"
            };

            var expectedSuccessResponseContent = new ServiceResponse<UpdateCategoryViewModel>
            {
                Success = true,
                Message = string.Empty,
                Data = updateCategoryViewModel
            };

            var expectedSuccessResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedSuccessResponseContent))
            };
            var mockHttpContext = new Mock<HttpContext>();
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockHttpClientService.Setup(c => c.GetHttpResponseMessage<UpdateCategoryViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedSuccessResponse);
            var target = new CategoryController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object },
            };

            // Act
            var actual = target.Details(categoryId) as ViewResult;
            // Assert
            Assert.NotNull(actual);
            Assert.Equal(updateCategoryViewModel.CategoryId, ((UpdateCategoryViewModel)actual.Model).CategoryId);
            Assert.Equal(updateCategoryViewModel.CategoryName, ((UpdateCategoryViewModel)actual.Model).CategoryName);
            Assert.Equal(updateCategoryViewModel.CategoryDescription, ((UpdateCategoryViewModel)actual.Model).CategoryDescription);
            mockHttpClientService.Verify(c => c.GetHttpResponseMessage<UpdateCategoryViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);
        }

        [Fact]
        public void Edit_ReturnViewWithCategory_WhenCategoryExists()
        {
            // Arrange
            int categoryId = 1;
            var updateCategoryViewModel = new UpdateCategoryViewModel
            {
                CategoryId = categoryId,
                CategoryName = "Category 1",
                CategoryDescription = "Description 1"
            };

            var expctedSuccessResponseContent = new ServiceResponse<UpdateCategoryViewModel>
            {
                Success = true,
                Message = string.Empty,
                Data = updateCategoryViewModel
            };

            var expectedSuccessResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expctedSuccessResponseContent))
            };
            var mockHttpContext = new Mock<HttpContext>();
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockHttpClientService.Setup(c => c.GetHttpResponseMessage<UpdateCategoryViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedSuccessResponse);
            var target = new CategoryController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object },
            };

            // Act
            var actual = target.Edit(categoryId) as ViewResult;
            // Assert
            Assert.NotNull(actual);
            Assert.Equal(updateCategoryViewModel.CategoryId, ((UpdateCategoryViewModel)actual.Model).CategoryId);
            Assert.Equal(updateCategoryViewModel.CategoryName, ((UpdateCategoryViewModel)actual.Model).CategoryName);
            Assert.Equal(updateCategoryViewModel.CategoryDescription, ((UpdateCategoryViewModel)actual.Model).CategoryDescription);
            mockHttpClientService.Verify(c => c.GetHttpResponseMessage<UpdateCategoryViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);
        }

        [Fact]
        public void Delete_ReturnViewWithCategory_WhenCategoryExists()
        {
            // Arrange
            int categoryId = 1;
            var categoryViewModel = new CategoryViewModel
            {
                CategoryId = categoryId,
                CategoryName = "Category 1",
                CategoryDescription = "Description 1"
            };

            var expectedSuccessResponseContent = new ServiceResponse<CategoryViewModel>
            {
                Success = true,
                Message = string.Empty,
                Data = categoryViewModel
            };

            var expectedSuccessResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedSuccessResponseContent))
            };
            var mockHttpContext = new Mock<HttpContext>();
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockHttpClientService.Setup(c => c.GetHttpResponseMessage<CategoryViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedSuccessResponse);
            var target = new CategoryController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object },
            };

            // Act
            var actual = target.Delete(categoryId) as ViewResult;
            // Assert
            Assert.NotNull(actual);
            Assert.Equal(categoryViewModel.CategoryId, ((CategoryViewModel)actual.Model).CategoryId);
            Assert.Equal(categoryViewModel.CategoryName, ((CategoryViewModel)actual.Model).CategoryName);
            Assert.Equal(categoryViewModel.CategoryDescription, ((CategoryViewModel)actual.Model).CategoryDescription);
            mockHttpClientService.Verify(c => c.GetHttpResponseMessage<CategoryViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);
        }

        [Fact]
        public void DeleteConfirmed_ReturnsRedirectToActionResult_WhenDeleteSucceeds()
        {
            // Arrange
            var categoryId = 1;
            var successMessage = "Category deleted successfully.";
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockHttpContext = new Mock<HttpContext>();
            var mockRequest = new Mock<HttpRequest>();
            mockHttpContext.Setup(c => c.Request).Returns(mockRequest.Object);
            var mockTempData = new TempDataDictionary(mockHttpContext.Object, Mock.Of<ITempDataProvider>());
            var expectedResponse = new ServiceResponse<string> { Success = true, Message = successMessage };
            mockHttpClientService
            .Setup(s => s.ExecuteApiRequest<ServiceResponse<string>>(It.IsAny<string>(), It.IsAny<HttpMethod>(), mockRequest.Object, It.IsAny<object>(), It.IsAny<int>()))
            .Returns(expectedResponse);
            var controller = new CategoryController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object },
                TempData = mockTempData
            };

            // Act
            var result = controller.DeleteConfirmed(categoryId);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Equal(successMessage, mockTempData["SuccessMessage"]);
        }
    }
}
