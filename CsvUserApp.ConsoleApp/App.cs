using CsvUserApp.Core.Exceptions;
using CsvUserApp.Core.Interfaces;
using System.Linq;

namespace CsvUserApp.ConsoleApp
{
    public class App
    {
        private readonly IUserService _userService;

        public App(IUserService userService)
        {
            _userService = userService;
        }

        public async Task RunAsync()
        {
            while (true)
            {
                try
                {
                    Console.WriteLine("\nChoose an option:");
                    Console.WriteLine("1 - Find user by email");
                    Console.WriteLine("2 - Average age");
                    Console.WriteLine("3 - Users older than X");
                    Console.WriteLine("0 - Exit");

                    Console.Write("Option: ");
                    var option = Console.ReadLine();

                    switch (option)
                    {
                        case "1":
                            Console.Write("Enter email: ");
                            string email = Console.ReadLine()!;
                            var user = await _userService.FindByEmailAsync(email);
                            Console.WriteLine(user != null
                                ? $" Found: {user.Name}, {user.Age} years old"
                                : " User not found.");
                            break;

                        case "2":
                            var average = await _userService.GetAverageAgeAsync();
                            Console.WriteLine(average.HasValue
                                ? $" Average age: {average.Value:F2}"
                                : " No users available.");
                            break;

                        case "3":
                            Console.Write("Enter minimum age: ");
                            if (int.TryParse(Console.ReadLine(), out int age))
                            {
                                if (age < 0)
                                {
                                    Console.WriteLine(" Age must be >= 0.");
                                    break;
                                }

                                var users = await _userService.GetUsersOlderThanAsync(age).ToListAsync();
                                if (!users.Any())
                                    Console.WriteLine($" No users older than {age}.");
                                else
                                {
                                    Console.WriteLine($" Users older than {age}:");
                                    foreach (var u in users)
                                        Console.WriteLine($"{u.Name} - {u.Age} years old");
                                }
                            }
                            else
                            {
                                Console.WriteLine(" Invalid age.");
                            }
                            break;

                        case "0":
                            Console.WriteLine(" Exiting...");
                            return;

                        default:
                            Console.WriteLine(" Invalid option.");
                            break;
                    }
                }
                catch (FileNotFoundException ex)
                {
                    Console.WriteLine($" File error: {ex.Message}");
                }
                catch (UserDataException ex)
                {
                    Console.WriteLine($" Data error: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($" Unexpected error: {ex.Message}");
                }
            }
        }
    }
}
