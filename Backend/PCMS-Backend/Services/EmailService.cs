using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using PCMS_Backend.Interfaces.Services;
using PCMS_Backend.Shared;

public class MailKitEmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public MailKitEmailService(IOptions<EmailSettings> settings)
    {
        _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = false)
    {
        if (string.IsNullOrWhiteSpace(toEmail))
        {
            throw new ArgumentException("Recipient email cannot be null or empty.", nameof(toEmail));
        }

        // 1. Create the message structure using MimeKit
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = subject;

        // Construct body (supports plain text or complex HTML)
        var bodyBuilder = new BodyBuilder();
        if (isHtml)
        {
            bodyBuilder.HtmlBody = body;
        }
        else
        {
            bodyBuilder.TextBody = body;
        }
        message.Body = bodyBuilder.ToMessageBody();

        // 2. Transmit using MailKit SmtpClient
        using (var client = new SmtpClient())
        {
            try
            {
                // SecureSocketOptions.StartTls is standard for port 587
                await client.ConnectAsync(_settings.SmtpServer, _settings.Port, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_settings.AuthEmail, _settings.AppPassword);
                await client.SendAsync(message);
            }
            finally
            {
                // Cleanly disconnect from server even if an exception occurs
                await client.DisconnectAsync(true);
            }
        }
    }
}