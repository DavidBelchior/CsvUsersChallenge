using CsvUserApp.Core.Entities;

namespace CsvUserApp.Core.Interfaces
{
    public interface IUserRepository
    {
        IAsyncEnumerable<User> GetAllUsersAsync();
        Task<User?> FindByEmailAsync(string email);
        IAsyncEnumerable<User> GetUsersOlderThanAsync(int age);
    }
}
