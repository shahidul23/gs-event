using System;
using GSEvent.Email.Service.Interface;

namespace GSEvent.Email.Service;

public class VerificationEmailService : IVerificationEmailService
{
    private readonly IEmailSender _emailSender;
    public VerificationEmailService(
        IEmailSender emailSender
    )
    {
        _emailSender = emailSender;
    }
    public async Task SendVerificationEmailAsync(
        string email, 
        string userName, 
        string verificationUrl, 
        CancellationToken cancellationToken = default
    )
    {
        var message = new EmailMessage
        {
            To = email,
            Subject =
                "Verify your Gulshan Society Event Management account",
            HtmlBody = BuildVerificationEmail(
                userName,
                verificationUrl
            )
        };
        await _emailSender.SendAsync(
            message,
            cancellationToken
        );
    }

    private string BuildVerificationEmail(string userName, string verificationUrl)
    {
        return $"""
            <html>
            <body>
                <h2>Welcome to Gulshan Society, {userName}!</h2>
                <p>
                    Thank you for registering.
                </p>
                <p>
                    Please verify your email address by
                    clicking the button below:
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
                    If you did not create this account,
                    you can ignore this email.
                </p>
                <p>
                    Regards,<br/>
                    Gulshan Society Team
                </p>
            </body>
        </html>
        """;
    }
}
