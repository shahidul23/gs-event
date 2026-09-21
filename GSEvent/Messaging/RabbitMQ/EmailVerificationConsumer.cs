using System;
using GSEvent.Email.Service.Interface;
using GSEvent.Enums;
using GSEvent.RabbitMQ;
using GSEvent.RabbitMQ.Consumers;
using GSEvent.RabbitMQ.Service.Interface;
using Microsoft.Extensions.Options;

namespace GSEvent.Messaging.RabbitMQ;

public class EmailVerificationConsumer : RabbitMqEmailConsumer<EmailVerificationMessage>
{
    public EmailVerificationConsumer(
       IOptions<RabbitMqSettings> options,
       IServiceScopeFactory serviceScopeFactory,
       IRabbitMqConnection rabbitMqConnection
    ) : base(options, serviceScopeFactory, rabbitMqConnection)
    {
    }
    protected override RabbitMqQueue Queue => RabbitMqQueue.EmailVerification;
    protected override async Task HandleMessageAsync(
        EmailVerificationMessage message, 
        IServiceProvider serviceProvider, 
        CancellationToken cancellationToken
    )
    {
        var emailService = serviceProvider.GetRequiredService<IVerificationEmailService>();
        var verificationUrl =
            $"http://localhost:5071/api/verify-email" +
            $"?username={Uri.EscapeDataString(message.UserName)}" +
            $"&token={Uri.EscapeDataString(message.VerificationToken)}";
            
        await emailService.SendVerificationEmailAsync(
            message.Email,
            message.UserName,
            verificationUrl
        );
    }  
}
