namespace WebApi.RabbitMq.Server.RabbitMQExtension;

public static class AddRabbitMqExtensions
{
    public static void AddRabbitMqExtension(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<RabbitMqConnectionOption>(builder.Configuration.GetSection("RabbitMQ"));
        builder.Services.AddSingleton<IRabbitMqConnectionFactory, RabbitMqConnectionFactory>();
        builder.Services.AddTransient<IRabbitMqMessagePublisher, RabbitMqMessagePublisher>();
        builder.Services.AddHostedService<RabbitMqConsumerService>();
    }
}
