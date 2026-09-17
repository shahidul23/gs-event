using System;

namespace GSEvent.Email.Service.Interface;

public interface IEmailService
{
    Task SendVerificationEmailAsync(
        string email,
        string userName,
        string verificationUrl
    );
}
