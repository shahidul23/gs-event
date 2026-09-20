using System;
using RabbitMQ.Client;

namespace GSEvent.RabbitMQ.Service.Interface;

public interface IRabbitMqConnection
{
    Task<IConnection> CreateConnectionAsync(
        CancellationToken cancellationToken
    );
}
