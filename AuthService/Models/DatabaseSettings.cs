namespace AuthService.Models;

public class DatabaseSettings
{
    public const string SectionName = "DatabaseSettings";
    
    public string ConnectionString { get; set; } = string.Empty;
} 