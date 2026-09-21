using System;

namespace GSEvent.Email.Service.Interface;

public interface IVerificationEmailService
{
    Task SendVerificationEmailAsync(
        string email,
        string userName,
        string verificationUrl,
        CancellationToken cancellationToken = default
    );
}
