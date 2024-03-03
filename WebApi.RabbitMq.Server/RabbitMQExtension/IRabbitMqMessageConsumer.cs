using WebApi.RabbitMq.Server.Controllers;

namespace WebApi.RabbitMq.Server.RabbitMQExtension;

public interface IRabbitMqMessageConsumer<T>
{
    string Consume(T messageObj);
}

public class TestConsumer : IRabbitMqMessageConsumer<Test>
{
    public string Consume(Test messageObj)
    {
        Console.WriteLine($"TestConsumer:{System.Text.Json.JsonSerializer.Serialize(messageObj)}");
        return "";
    }
}