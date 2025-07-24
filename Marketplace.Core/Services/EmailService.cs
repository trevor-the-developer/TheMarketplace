using System;
using System.Threading.Tasks;
using Marketplace.Core.Constants;
using Marketplace.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace Marketplace.Core.Services;

public class EmailService : IEmailService
{
    private readonly string _fromEmail;
    private readonly string _fromName;
    private readonly ILogger<EmailService> _logger;
    private readonly string _smtpHost;
    private readonly int _smtpPort;
    private readonly bool _useSsl;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _smtpHost = configuration["EmailSettings:SmtpHost"] ?? ApiConstants.DefaultSmtpHost;
        _smtpPort = configuration.GetValue("EmailSettings:SmtpPort", ApiConstants.DefaultSmtpPort);
        _useSsl = configuration.GetValue("EmailSettings:SmtpUseSSL", ApiConstants.DefaultSmtpUseSsl);
        _fromEmail = configuration["EmailSettings:FromEmail"] ?? "admin@themarketplace.local";
        _fromName = configuration["EmailSettings:FromName"] ?? "The Marketplace Admin";
        _logger = logger;
    }

    public async Task SendConfirmationEmailAsync(string emailAddress, string confirmationLink)
    {
        ArgumentNullException.ThrowIfNull(emailAddress);
        ArgumentNullException.ThrowIfNull(confirmationLink);

        if (string.IsNullOrWhiteSpace(emailAddress))
            throw new ArgumentException("Email address cannot be empty", nameof(emailAddress));

        if (string.IsNullOrWhiteSpace(confirmationLink))
            throw new ArgumentException("Confirmation link cannot be empty", nameof(confirmationLink));

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_fromName, _fromEmail));
        message.To.Add(new MailboxAddress("", emailAddress));
        message.Subject = "Confirm your registration";
        message.Body = new TextPart("html")
        {
            Text = $"<p>Please click <a href='{confirmationLink}'>here</a> to confirm your registration.</p>"
        };

        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync(_smtpHost, _smtpPort, _useSsl);
            await client.SendAsync(message);
            _logger.LogInformation("Email sent to {EmailAddress}", emailAddress);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {EmailAddress}", emailAddress);
            throw;
        }
        finally
        {
            await client.DisconnectAsync(true);
        }
    }
}