using SQLite;

namespace MackerelSocial.Core.Models;

public class AppSettings
{
    [PrimaryKey]
    [AutoIncrement]
    public int Id { get; set; }

    public AppLanguage Language { get; set; }

    public AppTheme Theme { get; set; }
}