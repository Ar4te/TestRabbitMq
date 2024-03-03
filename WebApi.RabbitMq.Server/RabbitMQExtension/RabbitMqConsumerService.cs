
namespace WebApi.RabbitMq.Server.RabbitMQExtension
{
    public class RabbitMqConsumerService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public RabbitMqConsumerService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            RabbitMqConsumerRegister.Register(_serviceProvider);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
