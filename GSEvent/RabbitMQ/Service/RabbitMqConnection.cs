using System;
using GSEvent.RabbitMQ.Service.Interface;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace GSEvent.RabbitMQ.Service;

public class RabbitMqConnection : IRabbitMqConnection
{
    private readonly RabbitMqSettings _settings;
    public RabbitMqConnection(
        IOptions<RabbitMqSettings> options
    )
    {
        _settings = options.Value;
    }
    public async Task<IConnection> CreateConnectionAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _settings.Host,
            Port = _settings.Port,
            UserName = _settings.Username,
            Password = _settings.Password
        };
        return await factory.CreateConnectionAsync(cancellationToken);
    }
}
