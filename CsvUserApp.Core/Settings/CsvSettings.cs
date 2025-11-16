using System.ComponentModel.DataAnnotations;

namespace CsvUserApp.Core
{
    public class CsvSettings
    {
        [Required(ErrorMessage = "FilePath must be provided in appsettings.json")]
        public string FilePath { get; set; } = string.Empty;
    }
}
