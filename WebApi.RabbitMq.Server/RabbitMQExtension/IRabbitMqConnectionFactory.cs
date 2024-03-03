using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace WebApi.RabbitMq.Server.RabbitMQExtension;

public interface IRabbitMqConnectionFactory
{
    IConnection connection { get; }
}


public class RabbitMqConnectionFactory : IRabbitMqConnectionFactory
{
    public IConnection connection { get; }
    public RabbitMqConnectionFactory(IOptions<RabbitMqConnectionOption> option)
    {
        ConnectionFactory connectionFactory = new()
        {
            HostName = option.Value.HostName,
            UserName = option.Value.UserName,
            Password = option.Value.Password
        };

        connection = connectionFactory.CreateConnection();
    }
}

public class RabbitMqConnectionOption
{
    public string HostName { get; set; }
    public string Password { get; set; }
    public string UserName { get; set; }
}