using urutau.Services.Interfaces;

namespace urutau.Services;

public sealed class SmsSender : ISmsSender
{
    public Task SendAsync(string phoneNumber, string message, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}