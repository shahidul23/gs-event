using System;
using System.Text;
using System.Text.Json;
using GSEvent.Enums;
using GSEvent.RabbitMQ.Service.Interface;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace GSEvent.RabbitMQ.Consumers;

public abstract class RabbitMqEmailConsumer<TMessage> : BackgroundService 
    where TMessage : class
{
    private readonly RabbitMqSettings _settings;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IRabbitMqConnection _rabbitMqConnection;
    private IConnection? _connection;
    private IChannel? _channel;
    protected RabbitMqEmailConsumer(
        IOptions<RabbitMqSettings> options,
        IServiceScopeFactory serviceScopeFactory,
        IRabbitMqConnection rabbitMqConnection
    )
    {
        _settings = options.Value;
        _serviceScopeFactory = serviceScopeFactory;
        _rabbitMqConnection = rabbitMqConnection;
    }
    protected abstract RabbitMqQueue Queue {get;}
    protected abstract Task HandleMessageAsync(
        TMessage message,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken
    );
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var queueName = _settings.Queues[Queue];
        _connection = await _rabbitMqConnection.CreateConnectionAsync(stoppingToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);
        await _channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete:false,
            cancellationToken: stoppingToken
        );
        await _channel.BasicQosAsync(
            prefetchSize:0,
            prefetchCount:1,
            global: false,
            cancellationToken: stoppingToken
        );
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async(_, eventArgs) =>
        {
            try
            {
                var json = Encoding.UTF8.GetString(
                    eventArgs.Body.ToArray()
                );
                var message = JsonSerializer.Deserialize<TMessage>(json);
                if (message == null)
                {
                    await _channel.BasicNackAsync(
                        eventArgs.DeliveryTag,
                        multiple:false,
                        requeue: false
                    );
                    return;
                }
                using var scope = _serviceScopeFactory.CreateAsyncScope();
                await HandleMessageAsync(
                    message,
                    scope.ServiceProvider,
                    stoppingToken
                );
                await _channel.BasicAckAsync(
                    eventArgs.DeliveryTag,
                    multiple:false
                );
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine(
                    $"{GetType().Name} stopped."
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"{GetType().Name} failed: {ex.Message}"
                );

                await _channel.BasicNackAsync(
                    eventArgs.DeliveryTag,
                    multiple: false,
                    requeue: false
                );
            }
        };
        await _channel.BasicConsumeAsync(
            queue: queueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken
        );

        await Task.Delay(
            Timeout.Infinite,
            stoppingToken
        );
    }
    public override async Task StopAsync(
        CancellationToken cancellationToken)
    {
        if (_channel != null)
        {
            await _channel.CloseAsync(
                cancellationToken
            );
        }

        if (_connection != null)
        {
            await _connection.CloseAsync(
                cancellationToken
            );
        }

        await base.StopAsync(cancellationToken);
    }
}

