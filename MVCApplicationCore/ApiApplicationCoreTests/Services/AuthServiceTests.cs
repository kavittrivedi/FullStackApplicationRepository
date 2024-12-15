using ApiApplicationCore.Data.Contract;
using ApiApplicationCore.Dtos;
using ApiApplicationCore.Models;
using ApiApplicationCore.Services.Implementation;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiApplicationCoreTests.Services
{
    public class AuthServiceTests
    {
        [Fact]
        public void RegisterUserService_ReturnsSuccess_WhenValidRegistration()
        {
            // Arrange
            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockAuthRepository.Setup(repo => repo.UserExists(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
            mockAuthRepository.Setup(repo => repo.RegisterUser(It.IsAny<User>())).Returns(true);

            
            var target = new AuthService(mockAuthRepository.Object, mockConfiguration.Object);

            var registerDto = new RegisterDto
            {
                FirstName = "firstname",
                LastName = "lastname",
                Email = "email@example.com",
                LoginId = "loginid",
                ContactNumber = "1234567890",
                Password = "Password@123"
            };

            // Act
            var result = target.RegisterUserService(registerDto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(string.Empty, result.Message);
            mockAuthRepository.Verify(c => c.UserExists(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            mockAuthRepository.Verify(c => c.RegisterUser(It.IsAny<User>()), Times.Once);
        }
        [Fact]
        public void RegisterUserService_ReturnsFailure_WhenWeakPassword()
        {
            // Arrange
            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockConfiguration = new Mock<IConfiguration>();
            var authService = new AuthService(mockAuthRepository.Object, mockConfiguration.Object);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Mininum password length should be 8" + Environment.NewLine);
            stringBuilder.Append("Password should be apphanumeric" + Environment.NewLine);
            stringBuilder.Append("Password should contain special characters" + Environment.NewLine);
            var registerDto = new RegisterDto
            {
                Password = "weak"
            };

            // Act
            var result = authService.RegisterUserService(registerDto);

            // Assert
            Assert.False(result.Success);
            Assert.Contains(stringBuilder.ToString(), result.Message);
        }

        [Fact]
        public void RegisterUserService_ReturnsFailure_WhenUserAlreadyExists()
        {
            // Arrange
            var mockAuthRepository = new Mock<IAuthRepository>();
            mockAuthRepository.Setup(repo => repo.UserExists(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            var mockConfiguration = new Mock<IConfiguration>();
            var authService = new AuthService(mockAuthRepository.Object, mockConfiguration.Object);

            var registerDto = new RegisterDto
            {
                FirstName = "firstname",
                LastName = "lastname",
                Email = "email@example.com",
                LoginId = "existingUser",
                ContactNumber = "1234567890",
                Password = "Password@123"
            };

            // Act
            var result = authService.RegisterUserService(registerDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("User already exists.", result.Message);
            mockAuthRepository.Verify(c => c.UserExists(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact(Skip ="Not able to set password hash and password salt.")]
        public void LoginUserService_ReturnsToken_WhenValidLogin()
        {
            // Arrange
            var mockAuthRepository = new Mock<IAuthRepository>();
            // Orignal Password Salt 0x8F2F4CA880CB0B28A5CB5124D47A81B4D4A4F3899B1CF268212F87982DBF0C4983518EDB158D0817539BF835FB8A3BBE955C3369D34F8DB1D5B4C107BA3C17438E92CEB4D67080DAA0EF92C633E72E5101CDE831C33AA359B7C03B52FB8B88CA50102243CBBA21A43254EBCC43F13D34C7C3895A3DE52C055F782762546EBA3F
            string hexString = "8F2F4CA880CB0B28A5CB5124D47A81B4D4A4F3899B1CF268212F87982DBF0C4983518EDB158D0817539BF835FB8A3BBE955C3369D34F8DB1D5B4C107BA3C17438E92CEB4D67080DAA0EF92C633E72E5101CDE831C33AA359B7C03B52FB8B88CA50102243CBBA21A43254EBCC43F13D34C7C3895A3DE52C055F782762546EBA3F";
            hexString = hexString.Substring(2); // Remove the "0x" prefix

            byte[] passwordSalt = new byte[12];
            for (int i = 0; i < 12; i++)
            {
                passwordSalt[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            mockAuthRepository.Setup(repo => repo.ValidateUser(It.IsAny<string>())).Returns(new User
            {
                userId = 1,
                LoginId = "validUser",
                PasswordHash = Encoding.UTF8.GetBytes("Password@123"),
                PasswordSalt = new byte[] {80,97,115,115,119,111,114,100,64,49,50,51 }
            });

            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(config => config.GetSection("AppSettings:Token").Value).Returns("YourSecretKey");

            var authService = new AuthService(mockAuthRepository.Object, mockConfiguration.Object);

            var loginDto = new LoginDto
            {
                Username = "validUser",
                Password = "Password@123"
            };

            // Act
            var result = authService.LoginUserService(loginDto);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
        }
    }
}
