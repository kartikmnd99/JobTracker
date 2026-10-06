using JobTracker.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace JobTracker.Infrastructure.Email;

public class LogEmailSender : IEmailSender
{
    private readonly ILogger<LogEmailSender> _logger;

    public LogEmailSender(ILogger<LogEmailSender> logger) => _logger = logger;

    public Task SendAsync(string to, string subject, string body)
    {
        _logger.LogInformation("[FAKE EMAIL] To: {To} | Subject: {Subject}\n{Body}", to, subject, body);
        return Task.CompletedTask;
    }
}
