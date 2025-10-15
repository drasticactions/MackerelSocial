using SQLite;

namespace MackerelSocial.Models;

public class LoginUser
{
    [PrimaryKey]
    [AutoIncrement]
    public int Id { get; set; }

    public string Handle { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Did { get; set; } = string.Empty;

    public string SessionData { get; set; } = string.Empty;

    public LoginType LoginType { get; set; }

    public bool IsDefault { get; set; }
}