using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;

namespace Samples.Worker.RabbitMq
{
    public class RabbitPublishingWorker(IOptions<RabbitConnectionOptions> opts) : BackgroundService
    {
        private readonly RabbitConnectionOptions rabbitOpts = opts.Value;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = rabbitOpts.HostName,
                UserName = rabbitOpts.UserName,
                Password = rabbitOpts.Password,
            };

            using var connection = await factory.CreateConnectionAsync(stoppingToken);
            using var channel = await connection.CreateChannelAsync(null, stoppingToken);

            byte[] messageBodyBytes = Encoding.UTF8.GetBytes("Hello, world!");

            await channel.BasicPublishAsync("test", "testRoutingKey", true, new BasicProperties(), messageBodyBytes, stoppingToken);

            await channel.CloseAsync(cancellationToken: stoppingToken);
            await connection.CloseAsync(cancellationToken: stoppingToken);
        }
    }
}
