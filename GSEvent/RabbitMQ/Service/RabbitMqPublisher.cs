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
    public RabbitMqPublisher(IOptions<RabbitMqSettings> options)
    {
        _mqSettings = options.Value;
    }
    public async Task PublishAsync<T>(RabbitMqQueue queue, T message)
    {
        var queueName = _mqSettings.Queues[queue];
        var factory = new ConnectionFactory
        {
            HostName = _mqSettings.Host,
            Port = _mqSettings.Port,
            UserName = _mqSettings.Username,
            Password = _mqSettings.Password
        };
        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: queueName,
            durable: false,
            exclusive: false,
            autoDelete: false
        );
        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);
        var properties = new BasicProperties
        {
            Persistent = false
        };
        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: queueName,
            mandatory: false,
            basicProperties: properties,
            body: body
        );
        throw new NotImplementedException();
    }
}
