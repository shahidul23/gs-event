using System;
using System.Text;
using System.Text.Json;
using GSEvent.Email.Service.Interface;
using GSEvent.Enums;
using GSEvent.Exceptions;
using GSEvent.RabbitMQ;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace GSEvent.Messaging.RabbitMQ;

public class EmailVerificationConsumer : BackgroundService
{
    private readonly RabbitMqSettings _settings;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private IConnection? _connection;
    private IChannel? _channel;
    public EmailVerificationConsumer(
       IOptions<RabbitMqSettings> options,
       IServiceScopeFactory serviceScopeFactory
    )
    {
        _settings = options.Value;
        _serviceScopeFactory = serviceScopeFactory; 
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var queueName = _settings.Queues[RabbitMqQueue.EmailVerification];
        var factory = new ConnectionFactory
        {
            HostName = _settings.Host,
            Port = _settings.Port,
            UserName = _settings.Username,
            Password = _settings.Password
        };
        _connection = await factory.CreateConnectionAsync(stoppingToken);
        _channel = await _connection.CreateChannelAsync(
            cancellationToken: stoppingToken
        );
        await _channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: stoppingToken
        );

        await _channel.BasicQosAsync(
            prefetchSize: 0,
            prefetchCount: 1,
            global: false,
            cancellationToken: stoppingToken
        );
        var customer = new AsyncEventingBasicConsumer(_channel);
        customer.ReceivedAsync += async (_, eventArgs) =>
        {
            try
            {
                var json = Encoding.UTF8.GetString(
                    eventArgs.Body.ToArray()
                );
                var message = JsonSerializer.Deserialize<EmailVerificationMessage>(json);
                if (message == null)
                {
                    await _channel.BasicNackAsync(
                        eventArgs.DeliveryTag,
                        multiple:false,
                        requeue: false
                    );
                    return;
                }
                using var scope = _serviceScopeFactory.CreateScope();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                var verificationUrl =
                    $"http://localhost:8080/api/auth/verify-email" +
                    $"?username={Uri.EscapeDataString(message.UserName)}" +
                    $"&token={Uri.EscapeDataString(message.VerificationToken)}";
                await emailService.SendVerificationEmailAsync(
                    message.Email,
                    message.UserName,
                    verificationUrl
                );
                await _channel.BasicAckAsync(
                    eventArgs.DeliveryTag,
                    multiple: false
                );

            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Email verification failed: {ex.Message}"
                );

                await _channel.BasicNackAsync(
                    eventArgs.DeliveryTag,
                    multiple: false,
                    requeue: true
                );
            }
        };
        await _channel.BasicConsumeAsync(
            queue: queueName,
            autoAck: false,
            consumer: customer,
            cancellationToken: stoppingToken
        );
        await Task.Delay(
            Timeout.Infinite,
            stoppingToken
        );
    }
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel != null)
        {
            await _channel.CloseAsync(cancellationToken);
        }
        if (_connection != null)
        {
            await _connection.CloseAsync(cancellationToken);
        }
        await base.StopAsync(cancellationToken);
    }
}
