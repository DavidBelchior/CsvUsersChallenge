using CsvUserApp.Core.Entities;

namespace CsvUserApp.Core.Interfaces
{
    public interface IUserService
    {
        Task<User?> FindByEmailAsync(string email);
        IAsyncEnumerable<User> GetUsersOlderThanAsync(int age);
        Task<double?> GetAverageAgeAsync();
    }
}
-