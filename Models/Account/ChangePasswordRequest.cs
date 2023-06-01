namespace urutau.Models.Account;

public sealed class ChangePasswordRequest
{
    public long UserId { get; set; }
    public string Password { get; set; } = string.Empty;

    public string NewPassword { get; set; } = string.Empty;
}