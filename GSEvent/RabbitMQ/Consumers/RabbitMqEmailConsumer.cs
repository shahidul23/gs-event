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
    private readonly ILogger<RabbitMqEmailConsumer<TMessage>> _logger;
    private IConnection? _connection;
    private IChannel? _channel;
    protected RabbitMqEmailConsumer(
        IOptions<RabbitMqSettings> options,
        IServiceScopeFactory serviceScopeFactory,
        IRabbitMqConnection rabbitMqConnection,
        ILogger<RabbitMqEmailConsumer<TMessage>> logger
    )
    {
        _settings = options.Value;
        _serviceScopeFactory = serviceScopeFactory;
        _rabbitMqConnection = rabbitMqConnection;
        _logger = logger;
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
        _logger.LogInformation(
            "Starting RabbitMQ consumer for queue: {QueueName}",
            queueName
        );
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
                    _logger.LogWarning(
                        "Invalid message received from queue: {QueueName}",
                        queueName
                    );
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
                 _logger.LogInformation(
                    "Message processed successfully from queue: {QueueName}",
                    queueName
                );
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine(
                    $"{GetType().Name} stopped."
                );
                 _logger.LogInformation(
                    "RabbitMQ consumer stopped: {QueueName}",
                    queueName
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"{GetType().Name} failed: {ex.Message}"
                );
                _logger.LogError(
                    ex,
                    "Error processing message from queue: {QueueName}",
                    queueName
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
        _logger.LogInformation(
            "RabbitMQ consumer started successfully: {QueueName}",
            queueName
        );

        await Task.Delay(
            Timeout.Infinite,
            stoppingToken
        );
    }
    public override async Task StopAsync(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Stopping RabbitMQ consumer: {Consumer}",
            GetType().Name
        );
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

