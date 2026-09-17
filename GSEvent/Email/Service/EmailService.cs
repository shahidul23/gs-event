using System;
using GSEvent.Email.Service.Interface;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace GSEvent.Email.Service;

public class EmailService : IEmailService
{
    private readonly SmtpSettings _settings;
    public EmailService( 
        IOptions<SmtpSettings> options
    )
    {
        _settings = options.Value;
    }
    public async Task SendVerificationEmailAsync(string email, string userName, string verificationUrl)
    {
        var message = new MimeMessage();
        message.From.Add(
            new MailboxAddress(
                _settings.FromName,
                _settings.FromEmail
            )
        );
        message.To.Add(
            MailboxAddress.Parse(email)
        );
        message.Subject = "Verify your Gulshan Society Even Management account";
        var body = $"""
            <html>
            <body>
                <h2>Welcome to GSEvent, {userName}!</h2>
                <p>
                    Thank you for registering.
                </p>
                <p>
                    Please verify your email address by clicking the button below:
                </p>
                <p>
                    <a href="{verificationUrl}"
                       style="
                           display:inline-block;
                           padding:12px 20px;
                           background:#007bff;
                           color:white;
                           text-decoration:none;
                           border-radius:5px;
                       ">
                        Verify Email
                    </a>
                </p>
                <p>
                    If you did not create this account, you can ignore this email.
                </p>
                <p>
                    Regards,<br/>
                    Gulshan Society Team
                </p>
            </body>
            </html>
            """;
        message.Body = new BodyBuilder
        {
            HtmlBody = body
        }.ToMessageBody();
        using var smtp = new SmtpClient();
        var socketOption = bool.Parse(_settings.UseSsl)
            ? SecureSocketOptions.StartTls
            : SecureSocketOptions.None;
        
        await smtp.ConnectAsync(
            _settings.Host,
            _settings.Port,
            socketOption
        );
        if (!string.IsNullOrWhiteSpace(_settings.Username))
        {
            await smtp.AuthenticateAsync(
                _settings.Username,
                _settings.Password
            );
        }
        await smtp.SendAsync(message);
        await smtp.DisconnectAsync(true);
    }
}
