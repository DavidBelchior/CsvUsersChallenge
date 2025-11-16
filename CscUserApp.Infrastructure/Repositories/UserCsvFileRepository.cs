using CsvUserApp.Core;
using CsvUserApp.Core.Entities;
using CsvUserApp.Core.Exceptions;
using CsvUserApp.Core.Interfaces;
using CsvUserApp.Core.Validators;
using Microsoft.Extensions.Options;
using static System.Net.Mime.MediaTypeNames;

namespace CsvUserApp
{
    public class UserCsvFileRepository : IUserRepository
    {
        private readonly string _filePath;
        private readonly UserValidator _validator = new UserValidator();


        public UserCsvFileRepository(IOptions<CsvSettings> options)
        {
            _filePath = options.Value.FilePath;
        }

        public IAsyncEnumerable<User> GetAllUsersAsync() => ReadAllUsersAsync();


        public async IAsyncEnumerable<User> GetUsersOlderThanAsync(int minAge)
        {
            await foreach (var user in ReadAllUsersAsync())
            {
                if (user.Age > minAge)
                    yield return user;
            }
        }


        public async Task<User?> FindByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            await foreach (var user in ReadAllUsersAsync())
            {
                if (user.Email.Equals(email, StringComparison.OrdinalIgnoreCase))
                    return user;
            }

            return null;
        }

        private async IAsyncEnumerable<User> ReadAllUsersAsync()
        {
            using var reader = new StreamReader(_filePath);

            if (reader.EndOfStream)
                throw new UserDataException("CSV file is empty.");

            bool headerSkipped = false;
            bool hasValidUser = false;

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                if (!headerSkipped)
                {
                    headerSkipped = true;
                    continue;
                }

                var user = ParseAndValidateLine(line);

                hasValidUser = true;
                yield return user;
            }

            if (!hasValidUser)
                throw new UserDataException("No valid users found in the CSV file.");
        }

        private User ParseAndValidateLine(string line)
        {
            var parts = line.Split(',').Select(p => p.Trim(' ', '"')).ToArray();

            if (parts.Length != 4)
                throw new UserDataException($"Line format invalid: {line}");

            if (!int.TryParse(parts[0], out int id))
                throw new UserDataException($"Invalid Id in line: {line}");

            if (!int.TryParse(parts[3], out int age))
                throw new UserDataException($"Invalid Age in line: {line}");

            var user = new User(id, parts[1], parts[2], age);

            var result = _validator.Validate(user);
            if (!result.IsValid)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.ErrorMessage));
                throw new UserDataException($"Validation failed for line: {line}. Errors: {errors}");
            }

            return user;
        }


    }
}
