namespace urutau.Models.Account;

public sealed class GenerateResetPasswordTokenRequest
{
    public string Email { get; set; } = string.Empty;
}