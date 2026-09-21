using System;

namespace GSEvent.Email.Service.Interface;

public interface IPasswordResetEmailService
{
    Task SendPasswordResetEmailAsync(
        string email,
        string userName,
        string resetUrl,
        CancellationToken cancellationToken = default
    );
}
