using CsvUserApp.Core.Entities;
using CsvUserApp.Core.Interfaces;

namespace CsvUserApp.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> FindByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            return await _userRepository.FindByEmailAsync(email);
        }

        public async Task<double?> GetAverageAgeAsync()
        {
            var users = new List<User>();
            await foreach (var user in _userRepository.GetAllUsersAsync())
            {
                users.Add(user);
            }

            return users.Any() ? users.Average(u => u.Age) : null;
        }

        public IAsyncEnumerable<User> GetUsersOlderThanAsync(int age)
        {
            return _userRepository.GetUsersOlderThanAsync(age);
        }
    }
}
