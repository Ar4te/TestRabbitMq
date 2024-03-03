using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;
using WebApi.RabbitMq.Server.RabbitMQExtension;

namespace WebApi.RabbitMq.Server.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] _summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];
    private readonly IRabbitMqMessagePublisher _rbPublisher;

    //private readonly IConnection _rabbitConnection;
    //private readonly IRabbitMqConnectionFactory _rbmqcf;
    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(/*IConnection rabbitConnection, IRabbitMqConnectionFactory rbmqcf,*/IRabbitMqMessagePublisher rbPublisher, ILogger<WeatherForecastController> logger)
    {
        _rbPublisher = rbPublisher;
        //_rabbitConnection = rabbitConnection;
        //_rbmqcf = rbmqcf;
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = _summaries[Random.Shared.Next(_summaries.Length)]
        })
        .ToArray();
    }


    [HttpGet]
    public IActionResult TestRbMQ(string message)
    {
        //using var channel = _rabbitConnection.CreateModel();
        //channel.QueueDeclare(
        //    queue: "my_queue",
        //    durable: false,
        //    exclusive: false,
        //    autoDelete: false,
        //    arguments: null);

        //var body = Encoding.UTF8.GetBytes(message);

        //channel.BasicPublish(
        //    exchange: "",
        //    routingKey: "my_queue",
        //    basicProperties: null,
        //    body: body
        //    );

        _rbPublisher.Publish<Test>(new Test()
        {
            test1 = "1",
            test2 = "2",
        });

        return Ok();
    }


}

[CustomRabbitMq(queue: "my_queue", exchange: "my_exchange", routingKey: "my_routingkey")]
public class Test
{
    public Test()
    {

    }
    public string test1 { get; set; }
    public string test2 { get; set; }
}