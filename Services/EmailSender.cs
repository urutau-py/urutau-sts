using urutau.Services.Interfaces;

namespace urutau.Services;

public sealed class EmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string message)
    {
        throw new NotImplementedException();
    }
}