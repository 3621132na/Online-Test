using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using TaskManagement.Controllers;
using TaskManagement.Data;
using TaskManagement.DTOs;
using TaskManagement.Models;

namespace Tests;

public class AuthControllerTests
{
    [Fact]
        public async Task Register_ShouldReturnOk_WhenUserIsRegisteredSuccessfully()
        {
            var dto = new UserRegisterDto { Username = "newuser", Email = "newuser@example.com", Password = "Password123" };
            var options = new DbContextOptionsBuilder<TaskManagementDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;
            var context = new TaskManagementDbContext(options);
            var mockConfiguration = new Mock<IConfiguration>();
            var controller = new AuthController(context, mockConfiguration.Object);
            var result = await controller.Register(dto);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("User registered successfully.", okResult.Value);
        }

        [Fact]
        public async Task Register_ShouldReturnBadRequest_WhenUsernameOrEmailExists()
        {
            var dto = new UserRegisterDto { Username = "existinguser", Email = "existinguser@example.com", Password = "Password123" };
            var options = new DbContextOptionsBuilder<TaskManagementDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;
            var context = new TaskManagementDbContext(options);
            context.users.Add(new user
            {
                username = "existinguser",
                email = "existinguser@example.com",
                passwordhash = BCrypt.Net.BCrypt.HashPassword("AnyPassword"),
                createdat = DateTime.Now,
                role = "User"
            });
            await context.SaveChangesAsync();
            var mockConfiguration = new Mock<IConfiguration>();
            var controller = new AuthController(context, mockConfiguration.Object);
            var result = await controller.Register(dto);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Username or email already exists.", badRequestResult.Value);
        }

        [Fact]
        public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
        {
            var options = new DbContextOptionsBuilder<TaskManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            using var context = new TaskManagementDbContext(options);
            var inMemorySettings = new Dictionary<string, string>
            {
                { "Jwt:Key", "rMMaL8hnJ2dUEKiki5WDBsxhgGGA0o8I" },
                { "Jwt:Issuer", "TestIssuer" },
                { "Jwt:Audience", "TestAudience" }
            };
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
            var password = "ValidPassword123";
            var user = new user
            {
                username = "validuser",
                email = "validuser@example.com",
                passwordhash = BCrypt.Net.BCrypt.HashPassword(password),
                role = "User",
                createdat = DateTime.Now
            };
            context.users.Add(user);
            await context.SaveChangesAsync();

            var controller = new AuthController(context, configuration);

            var loginDto = new UserLoginDto
            {
                Username = "validuser",
                Password = password
            };
            var result = await controller.Login(loginDto);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var tokenObject = Assert.IsType<Dictionary<string, object>>(okResult.Value!
                .GetType()
                .GetProperties()
                .ToDictionary(p => p.Name, p => p.GetValue(okResult.Value)!));
            Assert.True(tokenObject.ContainsKey("Token"));
            Assert.IsType<string>(tokenObject["Token"]);
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenUserDoesNotExist()
        {
            var dto = new UserLoginDto { Username = "nonexistentuser", Password = "Password123" };
            var options = new DbContextOptionsBuilder<TaskManagementDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;
            var context = new TaskManagementDbContext(options);
            var mockConfiguration = new Mock<IConfiguration>();
            var controller = new AuthController(context, mockConfiguration.Object);
            var result = await controller.Login(dto);
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid credentials.", unauthorizedResult.Value);
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenPasswordIsIncorrect()
        {
            var dto = new UserLoginDto { Username = "existinguser", Password = "WrongPassword" };
            var user = new user
            {
                username = "existinguser",
                passwordhash = BCrypt.Net.BCrypt.HashPassword("Password123"),
                email = "existing@example.com",
                createdat = DateTime.Now,
                role = "Regular"
            };
            var options = new DbContextOptionsBuilder<TaskManagementDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;
            var context = new TaskManagementDbContext(options);
            context.users.Add(user);
            await context.SaveChangesAsync();
            var mockConfiguration = new Mock<IConfiguration>();
            var controller = new AuthController(context, mockConfiguration.Object);
            var result = await controller.Login(dto);
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid credentials.", unauthorizedResult.Value);
        }
}