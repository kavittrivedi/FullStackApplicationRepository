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
    public class AuthControllerTests
    {
        [Fact]
        public void Register_ReturnsViewResult()
        {
            // Arrange
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("your-endpoint-url");

            var target = new AuthController(mockHttpClientService.Object, mockConfiguration.Object);

            // Act
            var actual = target.Register() as ViewResult;

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<ViewResult>(actual);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
        }

        [Fact]
        public void Register_WithValidModelState_ReturnsRedirectToActionResult()
        {
            // Arrange
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockHttpContext = new Mock<HttpContext>();
            //var mockHttpRequest = new Mock<HttpRequest>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("your-endpoint-url");

            var tempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), tempDataProvider.Object);
            var target = new AuthController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object },

            };

            var viewModel = new RegisterViewModel();

            // Set up response
            var successResponseContent = new ServiceResponse<string> { Message = "Success Message" };
            var successResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(successResponseContent))
            };
            mockHttpClientService.Setup(s => s.PostHttpResponseMessage(It.IsAny<string>(), It.IsAny<RegisterViewModel>(), It.IsAny<HttpRequest>()))
                .Returns(successResponse);

            // Act
            var actual = target.Register(viewModel) as RedirectToActionResult;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal("RegisterSuccess", actual.ActionName);
            Assert.Null(target.TempData["ErrorMessage"]);
            Assert.Equal("Success Message", target.TempData["SuccessMessage"]);
            mockHttpClientService.Verify(s => s.PostHttpResponseMessage(It.IsAny<string>(), It.IsAny<RegisterViewModel>(), It.IsAny<HttpRequest>()), Times.Once);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
        }

        [Fact]
        public void Register_WithInvalidModelState_ReturnsViewResult()
        {
            // Arrange
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("your-endpoint-url");
            var target = new AuthController(mockHttpClientService.Object, mockConfiguration.Object);
            target.ModelState.AddModelError("TestError", "Test Error Message");

            var viewModel = new RegisterViewModel();

            // Act
            var actual = target.Register(viewModel) as ViewResult;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(viewModel, actual.Model);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
        }

        [Fact]
        public void RegisterSuccess_ReturnsViewResult()
        {
            // Arrange
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("your-endpoint-url");
            var target = new AuthController(mockHttpClientService.Object, mockConfiguration.Object);

            // Act
            var actual = target.RegisterSuccess() as ViewResult;

            // Assert
            Assert.NotNull(actual);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
        }

        [Fact]
        public void Login_ReturnsViewResult()
        {
            // Arrange
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("your-endpoint-url");
            var target = new AuthController(mockHttpClientService.Object, mockConfiguration.Object);

            // Act
            var actual = target.Login() as ViewResult;

            // Assert
            Assert.NotNull(actual);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
        }

        [Fact]
        public void Login_RedirectsToCategoryIndex_WhenLoginSucceeds()
        {
            // Arrange
            var viewModel = new LoginViewModel { Username = "Username", Password = "Password" };
            var mockToken = "mockToken";
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockResponseCookies = new Mock<IResponseCookies>();
            mockResponseCookies.Setup(c => c.Append("jwtToken", mockToken, It.IsAny<CookieOptions>()));

            var httpResponse = new Mock<HttpResponse>();
            httpResponse.SetupGet(r => r.Cookies).Returns(mockResponseCookies.Object);

            var httpContext = new Mock<HttpContext>();
            httpContext.SetupGet(c => c.Response).Returns(httpResponse.Object);

            var target = new AuthController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                },
            };

            var responseContent = JsonConvert.SerializeObject(new ServiceResponse<string>
            {
                Success = true,
                Data = mockToken // Provide appropriate token here
            });
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseContent)
            };

            mockHttpClientService.Setup(s => s.PostHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>())).Returns(responseMessage);

            // Act
            var actual = target.Login(viewModel) as RedirectToActionResult;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal("Index", actual.ActionName);
            Assert.Equal("Category", actual.ControllerName);

            // Check if the JWT token cookie is set in the response
            mockResponseCookies.Verify(c => c.Append("jwtToken", mockToken, It.IsAny<CookieOptions>()), Times.Once);
            mockHttpClientService.Verify(s => s.PostHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>()), Times.Once);
        }

        [Fact]
        public void Login_WithInvalidViewModel_ReturnsViewResult()
        {
            // Arrange
            var viewModel = new LoginViewModel { Username ="username", Password="wrongpassword" };
            var errorMessage = "Invalid username or password!";
            var httpClientMock = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var tempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), tempDataProvider.Object);
            var target = new AuthController(httpClientMock.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                },
                TempData = tempData
            };

            var responseContent = JsonConvert.SerializeObject(new ServiceResponse<string>
            {
                Success = false,
                Message = errorMessage
            });
            var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(responseContent)
            };

            httpClientMock.Setup(c => c.PostHttpResponseMessage(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<HttpRequest>()))
                .Returns(responseMessage);

            // Act
            var actual = target.Login(viewModel) as ViewResult;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(errorMessage, target.TempData["ErrorMessage"]);
        }
    }
}
