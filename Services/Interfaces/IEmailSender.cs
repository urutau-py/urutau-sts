namespace urutau.Services.Interfaces;

public interface IEmailSender
{
    Task<bool> SendAsync(string email, string subject, string message, CancellationToken cancellationToken = default);
}