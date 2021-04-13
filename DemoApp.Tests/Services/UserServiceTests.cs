using DemoApp.Api.Models;
using DemoApp.Api.Services;
using DemoApp.Core.Entities;
using DemoApp.Core.Interfaces;
using DemoApp.Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace DemoApp.Tests.Services
{
    public class UserServiceTests
    {
        private ApiSettings ApiSettings = new ApiSettings
        {
            AuthKey = "G%hTPjt2^*dsETpg!6r-PXbcmk6GYu",
            HashIterations = 10000
        };

        private Mock<IOptions<ApiSettings>> _apiSettings = new Mock<IOptions<ApiSettings>>();
        private Mock<IUserRepository> _userRepo = new Mock<IUserRepository>();

        public UserServiceTests()
        {
            _apiSettings.Setup(x => x.Value).Returns(ApiSettings);
        }

        [Theory]
        [InlineData("test@test.com", "Testing12", true)]
        [InlineData("test@test.com", "T3st1ng12!", false)]
        public async void Authenticate(string username, string password, bool shouldAuthenicate)
        {
            var model = new AuthenticateRequest
            {
                Username = username,
                Password = password
            };
            var user = new User
            {
                Id = 111,
                PasswordHash = "10000.n+8OPkJOuLiMMyP5BomgwQ==.seB1WkF5VYMM+fsxolnAHojOpBCV6jRWE55w4UtwZmE=",
                Active = true
            };
            _userRepo.Setup(x => x.GetUserByUserName(model.Username)).ReturnsAsync(user);
            var service = new UserService(_apiSettings.Object, _userRepo.Object);

            var authModel = await service.Authenticate(model);

            if (shouldAuthenicate)
            {
                Assert.NotNull(authModel);
            }
            else
            {
                Assert.Null(authModel);
            }
        }

        [Fact]
        public async void CreateUser()
        {
            var user = new User
            {
                Id = 111,
                Password = "TestPw5678",
                Active = true
            };
            _userRepo.Setup(x => x.CreateUser(user)).ReturnsAsync(user.Id);
            var service = new UserService(_apiSettings.Object, _userRepo.Object);

            var userId = await service.CreateUser(user);

            Assert.Equal(111, userId.Value);
        }

        [Theory]
        [InlineData("test123")]
        [InlineData(null)]
        public async void UpdateUser(string password)
        {
            var user = new User
            {
                Password = password
            };
            _userRepo.Setup(x => x.UpdateUser(user)).ReturnsAsync(true);
            var service = new UserService(_apiSettings.Object, _userRepo.Object);

            var updateSucceeded = await service.UpdateUser(user);

            Assert.True(updateSucceeded);
        }
    }
}
