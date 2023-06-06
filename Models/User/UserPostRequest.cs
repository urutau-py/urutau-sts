namespace urutau.Models.User;

public sealed class UserPostRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public bool EmailConfirmed { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public bool PhoneNumberConfirmed { get; set; }
    public bool LockoutEnabled { get; set; }
    
    public string? Password { get; set; }
}