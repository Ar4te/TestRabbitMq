
using RabbitMQ.Client;
using WebApi.RabbitMq.Server.RabbitMQExtension;

namespace WebApi.RabbitMq.Server;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        //builder.Services.AddSingleton(provider =>
        //{
        //    var factory = new ConnectionFactory()
        //    {
        //        HostName = "192.168.253.128",
        //        UserName = "guest",
        //        Password = "guest"
        //    };
        //    return factory.CreateConnection();
        //});
        builder.AddRabbitMqExtension();
        //builder.Services
        //    .Configure<RabbitMqConnectionOption>(builder.Configuration.GetSection("RabbitMQ"));
        //builder.Services.AddSingleton<IRabbitMqConnectionFactory, RabbitMqConnectionFactory>();
        //builder.Services.AddTransient<IRabbitMqMessagePublisher, RabbitMqMessagePublisher>();
        //builder.Services.AddHostedService<RabbitMqConsumerService>();
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
