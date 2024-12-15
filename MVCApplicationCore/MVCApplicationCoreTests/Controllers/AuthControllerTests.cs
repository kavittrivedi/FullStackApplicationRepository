using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using MVCApplicationCore.Controllers;
using MVCApplicationCore.Services.Contract;
using MVCApplicationCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MVCApplicationCoreTests.Controllers
{
    public class AuthControllerTests
    {
        [Fact]
        public void Register_ReturnsViewResult()
        {
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            var target = new AuthController(mockAuthService.Object);

            // Act
            var actual = target.Register();

            // Assert
            Assert.IsType<ViewResult>(actual);
        }

        [Fact]
        public void Register_ReturnsViewResult_WhenModelStateIsNotValid()
        {
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            var target = new AuthController(mockAuthService.Object);
            target.ModelState.AddModelError("Email", "Email is required");

            // Act
            var actual = target.Register(new RegisterViewModel());

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.IsAssignableFrom<RegisterViewModel>(viewResult.Model);
        }

        [Fact]
        public void Register_ReturnsViewResultWithError_WhenRegistrationFails()
        {
            // Arrange
            var registerViewModel = new RegisterViewModel();
            var mockAuthService = new Mock<IAuthService>();
            mockAuthService.Setup(s => s.RegisterUserService(registerViewModel)).Returns("Registration failed message");

            var mockTepDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTepDataProvider.Object);
            var target = new AuthController(mockAuthService.Object)
            {
                TempData = tempData
            };

            // Act
            var actual = target.Register(registerViewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.Equal("Registration failed message", target.TempData["ErrorMessage"]);
            Assert.IsAssignableFrom<RegisterViewModel>(viewResult.Model);
            mockAuthService.Verify(s => s.RegisterUserService(registerViewModel), Times.Once);
        }

        [Fact]
        public void Register_ReturnsRedirectToActionResult_WhenRegistrationSucceeds()
        {
            // Arrange
            var registerViewModel = new RegisterViewModel();
            var mockAuthService = new Mock<IAuthService>();
            mockAuthService.Setup(s => s.RegisterUserService(registerViewModel)).Returns(string.Empty);
            var target = new AuthController(mockAuthService.Object);

            // Act
            var actual = target.Register(registerViewModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("RegisterSuccess", redirectToActionResult.ActionName);
            mockAuthService.Verify(s => s.RegisterUserService(registerViewModel), Times.Once);
        }

        [Fact]
        public void Login_ReturnsRedirectToActionResult_WhenLoginSucceeds()
        {
            // Arrange
            var login = new LoginViewModel { Username = "username", Password = "password" };
            var mockToken = "mockToken";
            var mockAuthService = new Mock<IAuthService>();
            mockAuthService.Setup(c => c.LoginUserService(login)).Returns(mockToken);
            var mockResponseCookie = new Mock<IResponseCookies>();

            mockResponseCookie.Setup(c => c.Append("jwtToken", mockToken, It.IsAny<CookieOptions>()));
            var mockHttpContext = new Mock<HttpContext>();
            var mockHttpResponse = new Mock<HttpResponse>();
            
            mockHttpContext.SetupGet(c => c.Response).Returns(mockHttpResponse.Object);
            mockHttpResponse.SetupGet(c => c.Cookies).Returns(mockResponseCookie.Object);
            var target = new AuthController(mockAuthService.Object)
            {
                ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
                {
                    HttpContext= mockHttpContext.Object,
                }
            };

            // Act
            var actual = target.Login(login) as RedirectToActionResult;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal("Index", actual.ActionName);
            Assert.Equal("Category", actual.ControllerName);
            mockAuthService.Verify(c => c.LoginUserService(login), Times.Once);
            mockResponseCookie.Verify(c => c.Append("jwtToken", mockToken, It.IsAny<CookieOptions>()), Times.Once);
            mockHttpContext.VerifyGet(c => c.Response, Times.Once);
            mockHttpResponse.VerifyGet(c => c.Cookies, Times.Once);
        }

        [Fact]
        public void Login_ReturnsViewResult_WhenModelStateIsNotValid()
        {
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            var controller = new AuthController(mockAuthService.Object);
            controller.ModelState.AddModelError("Email", "Email is required");

            // Act
            var actual = controller.Login(new LoginViewModel());

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.IsAssignableFrom<LoginViewModel>(viewResult.Model);
        }

        [Theory]
        [InlineData("Invalid username or password!")]
        [InlineData("Something went wrong, please try after sometime.")]
        public void Login_ReturnsViewResultWithError_WhenLoginFails(string errorMessage)
        {
            // Arrange
            var loginViewModel = new LoginViewModel();
            var mockAuthService = new Mock<IAuthService>();
            mockAuthService.Setup(s => s.LoginUserService(loginViewModel)).Returns(errorMessage);
            var tempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), tempDataProvider.Object);

            var target = new AuthController(mockAuthService.Object)
            {
                TempData = tempData
            };

            // Act
            var actual = target.Login(loginViewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.Equal(errorMessage, target.TempData["ErrorMessage"]);
            Assert.IsAssignableFrom<LoginViewModel>(viewResult.Model); // Verify the object is of given or dirived type.
        }

        [Fact]
        public void Logout_DeletesJwtTokenCookie_AndRedirectsToHomeIndex()
        {
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            var responseCookies = new Mock<IResponseCookies>();
            var response = new Mock<HttpResponse>();
            response.SetupGet(r => r.Cookies).Returns(responseCookies.Object);

            var httpContext = new Mock<HttpContext>();
            httpContext.SetupGet(c => c.Response).Returns(response.Object);

            var target = new AuthController(mockAuthService.Object);

            // Set controller's HttpContext to the mocked HttpContext
            target.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext.Object
            };

            // Act
            var actual = target.Logout() as RedirectToActionResult;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal("Home", actual.ControllerName);
            Assert.Equal("Index", actual.ActionName);

            // Verify that Response.Cookies.Delete method was called with correct arguments
            responseCookies.Verify(c => c.Delete("jwtToken"), Times.Once);
            response.VerifyGet(r => r.Cookies, Times.Once);
            httpContext.VerifyGet(c => c.Response, Times.Once);
        }
    }
}
