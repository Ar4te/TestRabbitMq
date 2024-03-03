using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Reflection;
using System.Text;

namespace WebApi.RabbitMq.Server.RabbitMQExtension;

public static class RabbitMqConsumerRegister
{
    public static void Register(IServiceProvider serviceProvider)
    {
        var types = typeof(RabbitMqConsumerRegister)
            .Assembly
            .GetTypes()
            .AsParallel()
            .Where(t => !t.IsInterface &&
                         t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRabbitMqMessageConsumer<>)))
            .Select(t =>
            {
                var msgObj = t.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRabbitMqMessageConsumer<>)).GetGenericArguments().First();
                var attr = msgObj.GetCustomAttribute<CustomRabbitMqAttribute>(inherit: false);
                return (t, msgObj, attr);
            })
            .Where(t => t.attr is not null)
            .AsSequential();

        var rabbitMqConnectionFactory = serviceProvider.GetService<IRabbitMqConnectionFactory>();

        foreach (var (t, msgObj, attr) in types)
        {
            var channel = rabbitMqConnectionFactory!.connection.CreateModel();
            if (attr!.Exchange != "")
            {
                channel.ExchangeDeclare(
                    exchange: attr!.Exchange,
                    type: attr!.ExchangeType,
                    durable: false,
                    autoDelete: false,
                    arguments: null);
            }

            channel.QueueDeclare(
                queue: attr!.Queue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            channel.QueueBind(
                queue: attr.Queue,
                exchange: attr!.Exchange ?? "",
                routingKey: attr.RoutingKey,
                arguments: null);
            channel.BasicQos(
                prefetchSize: 0,
                prefetchCount: 1,
                global: false);
            var consumer = new EventingBasicConsumer(channel);
            var rabbitMqConsumer = Activator.CreateInstance(t);
            var consumerMethod = t.GetMethod("Consume");
            consumer.Received += (sender, e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                var obj = System.Text.Json.JsonSerializer.Deserialize(message, msgObj);
                try
                {
                    consumerMethod!.Invoke(rabbitMqConsumer, new object[] { obj });
                }
                catch (Exception)
                {
                    consumer.Model.BasicReject(e.DeliveryTag, requeue: true);
                }
                consumer.Model.BasicAck(e.DeliveryTag, multiple: false);
            };

            channel.BasicConsume(queue: attr.Queue, autoAck: false, consumer);
        }
    }
}
