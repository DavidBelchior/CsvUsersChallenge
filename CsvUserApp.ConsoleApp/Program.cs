using CsvUserApp;
using CsvUserApp.ConsoleApp;
using CsvUserApp.Core;
using CsvUserApp.Core.Interfaces;
using CsvUserApp.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

class Program
{
    static async Task Main(string[] args)
    {
        using var host = Host.CreateDefaultBuilder(args)
             .ConfigureAppConfiguration((context, config) =>
             {
                 config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
             })
             .ConfigureServices((context, services) =>
             {
                 services.AddOptions<CsvSettings>()
                .Bind(context.Configuration.GetSection("CsvSettings"))
                .ValidateDataAnnotations()
                .Validate(s => File.Exists(s.FilePath), "CSV file does not exist");


                 services.AddScoped<IUserRepository, UserCsvFileRepository>();
                 services.AddScoped<IUserService, UserService>();
                 services.AddTransient<App>();
             })
             .ConfigureLogging(logging =>
             {
                 logging.ClearProviders();
             })
            .Build();

        try
        {
            var app = host.Services.GetRequiredService<App>();
            await app.RunAsync();
        }
        catch (OptionsValidationException ex)
        {
            Console.WriteLine($"Configuration error: {string.Join(", ", ex.Failures)}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
        }

    }
}