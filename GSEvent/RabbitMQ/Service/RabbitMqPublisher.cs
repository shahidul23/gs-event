using System;
using System.Text;
using System.Text.Json;
using GSEvent.Enums;
using GSEvent.RabbitMQ.Service.Interface;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace GSEvent.RabbitMQ.Service;

public class RabbitMqPublisher : IRabbitMqPublisher
{
    private readonly RabbitMqSettings _mqSettings;
    private readonly IRabbitMqConnection _rabbitMqConnection;
    public RabbitMqPublisher(IOptions<RabbitMqSettings> options, IRabbitMqConnection rabbitMqConnection)
    {
        _mqSettings = options.Value;
        _rabbitMqConnection = rabbitMqConnection;
    }
    public async Task PublishAsync<T>(RabbitMqQueue queue, T message)
    {
        var queueName = _mqSettings.Queues[queue];
       
        await using var connection = await _rabbitMqConnection.CreateConnectionAsync(CancellationToken.None);
        await using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false
        );
        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);
        var properties = new BasicProperties
        {
            Persistent = true
        };
        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: queueName,
            mandatory: false,
            basicProperties: properties,
            body: body
        );
        
    }
}
