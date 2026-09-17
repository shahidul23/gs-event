using System;
using GSEvent.Enums;

namespace GSEvent.RabbitMQ.Service.Interface;

public interface IRabbitMqPublisher
{
    Task PublishAsync<T>(RabbitMqQueue queue, T message);
}
