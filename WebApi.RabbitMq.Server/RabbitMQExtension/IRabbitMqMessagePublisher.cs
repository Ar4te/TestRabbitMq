using RabbitMQ.Client;
using System.Reflection;
using System.Text;

namespace WebApi.RabbitMq.Server.RabbitMQExtension;

public interface IRabbitMqMessagePublisher
{
    void Publish<T>(T messageObj) where T : new();
    void Publish<T>(T messageObj, bool durable = false) where T : new();
}

public class RabbitMqMessagePublisher : IRabbitMqMessagePublisher
{
    private readonly IRabbitMqConnectionFactory _rabbitMqConnectionFactory;

    public RabbitMqMessagePublisher(IRabbitMqConnectionFactory rabbitMqConnectionFactory)
    {
        _rabbitMqConnectionFactory = rabbitMqConnectionFactory;
    }
    public void Publish<T>(T messageObj) where T : new()
    {
        using var channel = _rabbitMqConnectionFactory.connection.CreateModel();
        var attr = messageObj!.GetType().GetCustomAttribute<CustomRabbitMqAttribute>(inherit: false);
        if (attr!.Exchange != "")
        {
            channel.ExchangeDeclare(
                exchange: attr.Exchange,
                type: attr.ExchangeType,
                durable: false,
                autoDelete: false,
                arguments: null);
        }

        if (string.IsNullOrEmpty(attr!.Queue))
        {
            throw new ArgumentNullException(nameof(attr.Queue));
        }

        channel.QueueDeclare(
            queue: attr!.Queue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var message = System.Text.Json.JsonSerializer.Serialize(messageObj);
        var body = Encoding.UTF8.GetBytes(message);

        if (string.IsNullOrEmpty(attr!.RoutingKey))
        {
            throw new ArgumentNullException(nameof(attr.RoutingKey));
        }

        channel.BasicPublish(
            exchange: attr!.Exchange,
            routingKey: attr.RoutingKey,
            basicProperties: null,
            body: body);
    }

    public void Publish<T>(T messageObj, bool durable = false) where T : new()
    {
        using var channel = _rabbitMqConnectionFactory.connection.CreateModel();
        var attr = messageObj!.GetType().GetCustomAttribute<CustomRabbitMqAttribute>(inherit: false);
        if (attr!.Exchange != "")
        {
            channel.ExchangeDeclare(
                exchange: attr.Exchange,
                type: attr.ExchangeType,
                durable: false,
                autoDelete: false,
                arguments: null);
        }

        if (string.IsNullOrEmpty(attr!.Queue))
        {
            throw new ArgumentNullException(nameof(attr.Queue));
        }

        channel.QueueDeclare(
            queue: attr!.Queue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var message = System.Text.Json.JsonSerializer.Serialize(messageObj);
        var body = Encoding.UTF8.GetBytes(message);
        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;
        if (string.IsNullOrEmpty(attr!.RoutingKey))
        {
            throw new ArgumentNullException(nameof(attr.RoutingKey));
        }

        channel.BasicPublish(
            exchange: attr!.Exchange,
            routingKey: attr.RoutingKey,
            basicProperties: properties,
            body: body);
    }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class CustomRabbitMqAttribute : Attribute
{
    public CustomRabbitMqAttribute(string queue, string routingKey, string exchange = "", string exchangeType = "direct")
    {
        Exchange = exchange;
        ExchangeType = exchangeType;
        Queue = queue;
        RoutingKey = routingKey;
    }
    public string Exchange { get; set; } = "";
    public string Queue { get; set; }
    public string RoutingKey { get; set; }
    public string ExchangeType { get; set; } = "direct";
}