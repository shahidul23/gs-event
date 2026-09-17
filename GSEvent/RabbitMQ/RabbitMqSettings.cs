using System;
using GSEvent.Enums;

namespace GSEvent.RabbitMQ;

public class RabbitMqSettings
{
    public string Host {get; set;} = string.Empty;
    public int Port {get; set; }
    public string Username {get; set;} = string.Empty;
    public string Password {get; set;} = string.Empty;
    public Dictionary<RabbitMqQueue, string> Queues {get; set;} = new();
}
