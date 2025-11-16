using CsvUserApp.Core;
using CsvUserApp.Core.Entities;
using CsvUserApp.Core.Exceptions;
using Microsoft.Extensions.Options;

namespace CsvUserApp.Tests.Infrastrucure.Repositories
{
    public class UserRepositoryTests
    {
        private const string TestFile = "test_users.csv";

        private IOptions<CsvSettings> GetOptions(string filePath)
        {
            return Options.Create(new CsvSettings { FilePath = filePath });
        }

        public UserRepositoryTests()
        {
            File.WriteAllText(TestFile,
            @"Id,Name,Email,Age
            1,John Doe,john@example.com,31
            2,Jane Smith,jane@example.com,25");
        }


        [Fact]
        public async Task GetAllUsersAsync_ShouldLoadAllUsers()
        {
            var repo = new UserCsvFileRepository(GetOptions(TestFile));

            var users = new List<User>();
            await foreach (var u in repo.GetAllUsersAsync())
                users.Add(u);

            Assert.Equal(2, users.Count);
            Assert.Contains(users, u => u.Name == "John Doe");
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldThrow_WhenFileNotFound()
        {
            File.WriteAllText(TestFile, "");

            var repo = new UserCsvFileRepository(GetOptions(TestFile));

            var ex = await Assert.ThrowsAsync<UserDataException>(async () =>
            {
                await foreach (var u in repo.GetAllUsersAsync())
                    _ = u;
            });

            Assert.Equal("CSV file is empty.", ex.Message);
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldThrow_WhenFileIsEmpty()
        {
            File.WriteAllText(TestFile, "");

            var repo = new UserCsvFileRepository(GetOptions(TestFile));

            var ex = await Assert.ThrowsAsync<UserDataException>(async () =>
            {
                await foreach (var u in repo.GetAllUsersAsync())
                    _ = u;
            });

            Assert.Equal("CSV file is empty.", ex.Message);
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldThrow_WhenLineFormatIsInvalid()
        {
            File.WriteAllText(TestFile,
            @"Id,Name,Email,Age
            1,OnlyThree,Values");

            var repo = new UserCsvFileRepository(GetOptions(TestFile));

            var ex = await Assert.ThrowsAsync<UserDataException>(async () =>
            {
                await foreach (var u in repo.GetAllUsersAsync())
                    _ = u;
            });

            Assert.Contains("Line format invalid", ex.Message);
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldThrow_WhenInvalidAge()
        {
            File.WriteAllText(TestFile,
            @"Id,Name,Email,Age
            1,John Doe,john@example.com,NotANumber");

            var repo = new UserCsvFileRepository(GetOptions(TestFile));

            var ex = await Assert.ThrowsAsync<UserDataException>(async () =>
            {
                await foreach (var u in repo.GetAllUsersAsync())
                    _ = u;
            });

            Assert.Contains("Invalid Age", ex.Message);
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldThrow_WhenValidationFails()
        {
            File.WriteAllText(TestFile,
            @"Id,Name,Email,Age
            1,,invalid-email,200");

            var repo = new UserCsvFileRepository(GetOptions(TestFile));

            var ex = await Assert.ThrowsAsync<UserDataException>(async () =>
            {
                await foreach (var u in repo.GetAllUsersAsync())
                    _ = u;
            });

            Assert.Contains("Validation failed", ex.Message);
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldSkipHeaderAndBlankLines()
        {
            File.WriteAllText(TestFile,
            @"
            Id,Name,Email,Age

            1,John Doe,john@example.com,31
            ");

            var repo = new UserCsvFileRepository(GetOptions(TestFile));

            var users = new List<User>();
            await foreach (var u in repo.GetAllUsersAsync())
                users.Add(u);

            Assert.Single(users);
            Assert.Equal("John Doe", users[0].Name);
        }


        [Fact]
        public async Task FindByEmailAsync_ShouldReturnUser_WhenExists()
        {
            var repo = new UserCsvFileRepository(GetOptions(TestFile));

            var user = await repo.FindByEmailAsync("jane@example.com");

            Assert.NotNull(user);
            Assert.Equal("Jane Smith", user!.Name);
        }


        [Fact]
        public async Task FindByEmailAsync_ShouldReturnNull_WhenNotExists()
        {
            var repo = new UserCsvFileRepository(GetOptions(TestFile));

            var user = await repo.FindByEmailAsync("nonexistent@example.com");

            Assert.Null(user);
        }

        [Fact]
        public async Task FindByEmailAsync_ShouldBeCaseInsensitive()
        {
            var repo = new UserCsvFileRepository(GetOptions(TestFile));

            var user = await repo.FindByEmailAsync("JANE@EXAMPLE.COM");
            
            Assert.NotNull(user);
            Assert.Equal("Jane Smith", user!.Name);
        }

        [Fact]
        public async Task GetUsersOlderThanAsync_ShouldReturnMatchingUsers()
        {
            var repo = new UserCsvFileRepository(GetOptions(TestFile));

            var users = new List<User>();
            await foreach (var u in repo.GetUsersOlderThanAsync(30))
                users.Add(u);

            Assert.Single(users);
            Assert.Contains(users, u => u.Name == "John Doe");
        }

        [Fact]
        public async Task GetUsersOlderThanAsync_ShouldReturnEmpty_WhenNoneMatch()
        {
            var repo = new UserCsvFileRepository(GetOptions(TestFile));

            var users = new List<User>();
            await foreach (var u in repo.GetUsersOlderThanAsync(50))
                users.Add(u);

            Assert.Empty(users);
        }
    }
}