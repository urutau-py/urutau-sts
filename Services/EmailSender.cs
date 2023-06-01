using SendGrid;
using SendGrid.Helpers.Mail;
using urutau.Services.Interfaces;

namespace urutau.Services;

public sealed class EmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;
    
    public EmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task<bool> SendAsync(string email, string subject, string message, CancellationToken cancellationToken = default)
    {
        var to = new EmailAddress(email);
        var from = new EmailAddress(_configuration["SendGrid:From"], _configuration["SendGrid:FromName"]);
        var msg = MailHelper.CreateSingleEmail(from, to, subject, message, message);

        var client = new SendGridClient(_configuration["SendGrid:ApiKey"]);

        var result = await client.SendEmailAsync(msg, cancellationToken);

        return result.IsSuccessStatusCode;
    }
}