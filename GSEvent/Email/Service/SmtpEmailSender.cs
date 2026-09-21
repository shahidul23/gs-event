using System;
using GSEvent.Email.Service.Interface;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
namespace GSEvent.Email.Service;

public class SmtpEmailSender : IEmailSender
{
    private readonly SmtpSettings _settings;
    public SmtpEmailSender(
        IOptions<SmtpSettings> options
    )
    {
        _settings = options.Value;
    }
    public async Task SendAsync(
        EmailMessage message, 
        CancellationToken cancellationToken = default
    )
    {
        var email = new MimeMessage();
        email.From.Add(
            new MailboxAddress(
                _settings.FromName,
                _settings.FromEmail
            )
        );
        email.To.Add(
            MailboxAddress.Parse(message.To)
        );
        email.Subject = message.Subject;
        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = message.HtmlBody
        };
        foreach (var attachment in message.Attachments)
        {
            bodyBuilder.Attachments.Add(
                attachment.FileName,
                attachment.Content,
                ContentType.Parse(attachment.ContentType)
            );
        }
        email.Body = bodyBuilder.ToMessageBody();
        using var smtp = new SmtpClient();
        var socketOption = bool.TryParse(_settings.UseSsl, out var useSsl) && useSsl 
            ? SecureSocketOptions.StartTls
            : SecureSocketOptions.None;
        await smtp.ConnectAsync(
            _settings.Host,
            _settings.Port,
            socketOption,
            cancellationToken
        );
        if (!string.IsNullOrWhiteSpace(_settings.Username))
        {
            await smtp.AuthenticateAsync(
                _settings.Username,
                _settings.Password,
                cancellationToken
            );
        }
        await smtp.SendAsync(email, cancellationToken);
        await smtp.DisconnectAsync(true, cancellationToken);
    }
}
