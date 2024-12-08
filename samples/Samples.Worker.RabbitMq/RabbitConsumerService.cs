

using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Samples.Worker.RabbitMq.ConsumptionHandlingServices;
using Samples.Worker.RabbitMq.Contracts;
using System.Text;
using System.Text.Json;

namespace Samples.Worker.RabbitMq
{
    public class RabbitConsumerService(IOptions<RabbitConnectionOptions> opts, IConsumptionHandler handler) : BackgroundService
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

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();

                var message = Encoding.UTF8.GetString(body);

                await handler.HandleMessageAsync(JsonSerializer.Deserialize<RabbitMessage>(message)!, stoppingToken);

                await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            while (!stoppingToken.IsCancellationRequested)
            {
                await channel.BasicConsumeAsync(queue: "ConsumptionTestRabbitMessageQueue", autoAck: false, consumer: consumer, stoppingToken);
            }
        }
    }
}
