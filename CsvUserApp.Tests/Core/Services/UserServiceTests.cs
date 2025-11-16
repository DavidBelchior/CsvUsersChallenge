using AutoFixture.AutoMoq;
using AutoFixture;
using CsvUserApp.Core.Entities;
using CsvUserApp.Core.Interfaces;
using CsvUserApp.Core.Services;
using Moq;

namespace CsvUserApp.Tests.Core.Services
{
    public class UserServiceTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IUserRepository> _repositoryMock;
        private readonly UserService _service;

        public UserServiceTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _repositoryMock = _fixture.Freeze<Mock<IUserRepository>>();
            _service = new UserService(_repositoryMock.Object);
        }


        [Fact]
        public async Task FindByEmailAsync_ShouldReturnUser_WhenEmailExists()
        {
            // Arrange
            var targetUser = new User(1, "Jane Doe", "jane@example.com", 25);

            _repositoryMock.Setup(r => r.FindByEmailAsync("jane@example.com"))
                           .ReturnsAsync(targetUser);

            // Act
            var result = await _service.FindByEmailAsync("jane@example.com");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("jane@example.com", result!.Email);
        }

        [Fact]
        public async Task GetUsersOlderThanAsync_ShouldReturnCorrectUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new User(1, "John", "john@example.com", 20),
                new User(2, "Jane", "jane@example.com", 35)
            };

            _repositoryMock.Setup(r => r.GetUsersOlderThanAsync(30))
                           .Returns(GetUsersOlderThanAsync(users, 30));

            // Act
            var result = await _service.GetUsersOlderThanAsync(30).ToListAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal("Jane", result[0].Name);
        }

        [Fact]
        public async Task GetAverageAgeAsync_ShouldReturnCorrectValue()
        {
            // Arrange
            var users = new List<User>
            {
                new User(1, "John", "john@example.com", 20),
                new User(2, "Jane", "jane@example.com", 40)
            };

            _repositoryMock.Setup(r => r.GetAllUsersAsync())
                           .Returns(GetUsersAsync(users));

            // Act
            var result = await _service.GetAverageAgeAsync();

            // Assert
            Assert.Equal(30, result);
        }


        private async IAsyncEnumerable<User> GetUsersAsync(List<User> users)
        {
            foreach (var user in users)
                yield return user;
        }

        private async IAsyncEnumerable<User> GetUsersOlderThanAsync(List<User> users, int minAge)
        {
            foreach (var user in users.Where(u => u.Age > minAge))
                yield return user;
        }
    }
}