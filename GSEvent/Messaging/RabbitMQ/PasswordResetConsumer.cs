using System;
using GSEvent.Email.Service.Interface;
using GSEvent.Enums;
using GSEvent.RabbitMQ;
using GSEvent.RabbitMQ.Consumers;
using GSEvent.RabbitMQ.Service.Interface;
using Microsoft.Extensions.Options;

namespace GSEvent.Messaging.RabbitMQ;

public class PasswordResetConsumer : RabbitMqEmailConsumer<PasswordResetEmailMessage>
{
    public PasswordResetConsumer(
       IOptions<RabbitMqSettings> options,
       IServiceScopeFactory serviceScopeFactory,
       IRabbitMqConnection rabbitMqConnection,
       ILogger<RabbitMqEmailConsumer<PasswordResetEmailMessage>> logger
    ) : base(options, serviceScopeFactory, rabbitMqConnection, logger)
    {
    }
    protected override RabbitMqQueue Queue => RabbitMqQueue.PasswordReset;

    protected override async Task HandleMessageAsync(
        PasswordResetEmailMessage message, 
        IServiceProvider serviceProvider, 
        CancellationToken cancellationToken
    )
    {
        var emailService =
        serviceProvider.GetRequiredService<IPasswordResetEmailService>();
    var resetUrl =
        $"http://localhost:5173/api/reset-password" +
        $"?email={Uri.EscapeDataString(message.Email)}" +
        $"&token={Uri.EscapeDataString(message.Token)}";

        await emailService.SendPasswordResetEmailAsync(
            message.Email,
            message.FullName,
            resetUrl,
            cancellationToken
        );
    }
}
