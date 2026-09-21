using System;

namespace GSEvent.Email.Service.Interface;

public interface IEmailSender
{
    Task SendAsync(
        EmailMessage message,
        CancellationToken cancellationToken = default
    );
}
