using System;
using GSEvent.Email.Service.Interface;

namespace GSEvent.Email.Service;

public class PasswordResetEmailService : IPasswordResetEmailService
{
    private IEmailSender _emailSender;
    public PasswordResetEmailService(
        IEmailSender emailSender
    )
    {
        _emailSender = emailSender;
    }
    public async Task SendPasswordResetEmailAsync(
        string email, 
        string userName, 
        string resetUrl, 
        CancellationToken cancellationToken = default
    )
    {
        var message = new EmailMessage
        {
            To = email,
            Subject =
                "Reset your Gulshan Society Event Management password",
            HtmlBody = BuildPasswordResetEmail(
                userName,
                resetUrl
            )
        };
        await _emailSender.SendAsync(
            message,
            cancellationToken
        );
    }

    private string BuildPasswordResetEmail(string userName, string resetUrl)
    {
        return $"""
        <html>
            <body>

                <h2>Password Reset</h2>

                <p>
                    Hello {userName},
                </p>

                <p>
                    We received a request to reset your
                    Gulshan Society Event Management account password.
                </p>

                <p>
                    Click the button below to reset your password:
                </p>

                <p>
                    <a href="{resetUrl}"
                       style="
                           display:inline-block;
                           padding:12px 20px;
                           background:#007bff;
                           color:white;
                           text-decoration:none;
                           border-radius:5px;
                       ">
                        Reset Password
                    </a>
                </p>

                <p>
                    If you did not request a password reset,
                    you can safely ignore this email.
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
