namespace urutau.Models.Account;

public sealed class ResetPasswordResponse
{
    public long UserId { get; set; }
    public string Token { get; set; } = string.Empty;
}