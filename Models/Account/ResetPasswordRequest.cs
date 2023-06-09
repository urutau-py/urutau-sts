namespace urutau.Models.Account;

public sealed class ResetPasswordRequest
{
    public long UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}