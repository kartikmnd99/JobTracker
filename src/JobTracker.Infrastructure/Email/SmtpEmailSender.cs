using System.Net;
using System.Net.Mail;
using JobTracker.Application.Interfaces;
using Microsoft.Extensions.Options;

namespace JobTracker.Infrastructure.Email;

public class SmtpEmailSender : IEmailSender
{
    private readonly SmtpSettings _settings;

    public SmtpEmailSender(IOptions<SmtpSettings> settings) => _settings = settings.Value;

    public async Task SendAsync(string to, string subject, string body)
    {
        using var client = new SmtpClient(_settings.Host, _settings.Port)
        {
            Credentials = new NetworkCredential(_settings.Username, _settings.Password),
            EnableSsl = _settings.EnableSsl
        };

        using var message = new MailMessage(_settings.From, to, subject, body);
        await client.SendMailAsync(message);
    }
}