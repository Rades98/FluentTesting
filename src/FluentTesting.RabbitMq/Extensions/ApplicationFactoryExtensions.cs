using DotNet.Testcontainers.Containers;
using FluentTesting.Common.Extensions;
using FluentTesting.Common.Interfaces;
using FluentTesting.RabbitMq.Options;
using System.Text.Json;

namespace FluentTesting.RabbitMq.Extensions
{
    /// <summary>
    /// Application factory extensions for rabbit MQ
    /// </summary>
    public static class ApplicationFactoryExtensions
    {
        /// <summary>
        /// Consume rabbit mq message
        /// </summary>
        /// <param name="factory">factory <seealso cref="IApplicationFactory"/></param>
        /// <param name="queue">queue name</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>Rabbit message representing object <see cref="RabbitMqMessage"/></returns>
        public static Task<RabbitMqMessage?> ConsumeRabbitMqMessage(this IApplicationFactory factory, string queue, CancellationToken cancellationToken)
            => factory.ConsumeMessage(queue, cancellationToken);

        /// <summary>
        /// Consume rabbit mq JSON message
        /// </summary>
        /// <typeparam name="T">type of message</typeparam>
        /// <param name="factory">factory <seealso cref="IApplicationFactory"/></param>
        /// <param name="queue">queue name</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>Predefined object</returns>
        public static async Task<T?> ConsumeJsonRabbitMqMessage<T>(this IApplicationFactory factory, string queue, CancellationToken cancellationToken)
        {
            var res = await factory.ConsumeMessage(queue, cancellationToken);

            var payload = res?.Payload;

            return payload is null ? default : JsonSerializer.Deserialize<T>(payload);
        }

        /// <summary>
        /// Publish string to rabbit and wait for consumption using cancellation token source provided
        /// </summary>
        /// <param name="factory">factory <seealso cref="IApplicationFactory"/></param>
        /// <param name="exchangeName">exchange name</param>
        /// <param name="routingKey">routing key</param>
        /// <param name="payload">payload</param>
        /// <param name="cts">cancellation token source used to stop waiting mechanism</param>
        public static Task PublishToRabbitAndWaitForConsumption(this IApplicationFactory factory, string exchangeName, string routingKey, string payload, CancellationTokenSource cts)
            => factory.ExecuteAndWait(factory.PublishToRabbit(exchangeName, routingKey, payload, cts.Token), cts);

        /// <summary>
        /// Publish JSON object to rabbit and wait for consumption using cancellation token source provided
        /// </summary>
        /// <param name="factory">factory <seealso cref="IApplicationFactory"/></param>
        /// <param name="exchangeName">exchange name</param>
        /// <param name="routingKey">routing key</param>
        /// <param name="payload">payload</param>
        /// <param name="cts">cancellation token source used to stop waiting mechanism</param>
        public static Task PublishJsonToRabbitAndWaitForConsumption<T>(this IApplicationFactory factory, string exchangeName, string routingKey, T payload, CancellationTokenSource cts)
            => factory.ExecuteAndWait(factory.PublishToRabbit(exchangeName, routingKey, JsonSerializer.Serialize(payload), cts.Token), cts);

        private async static Task<RabbitMqMessage?> ConsumeMessage(this IApplicationFactory factory, string queue, CancellationToken cancellationToken)
        {
            ExecResult? rabbitRes = null;

            var rabbitContainer = factory.Containers.First(x => x.Key == RabbitMqOptions.ContainerName);

            while (rabbitRes?.ExitCode != 0 && !cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(1000, cancellationToken);

                rabbitRes = (await rabbitContainer.Value.ExecAsync(["/bin/bash", "-c", $"rabbitmqadmin -u {RabbitMqOptions.UserName} -p {RabbitMqOptions.Password} get queue={queue} count=1"], cancellationToken));

                if (rabbitRes?.ExitCode == 0)
                {
                    return rabbitRes?.Stdout.ExtractMessageFromOutput();
                }
            }

            return null;
        }

        private static RabbitMqMessage? ExtractMessageFromOutput(this string output)
        {
            var message = new RabbitMqMessage();

            var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length < 5)
            {
                return null;
            }

            var messageDataLine = lines[3];

            var columns = messageDataLine.Split('|').Select(col => col.Trim()).ToArray();

            message.RoutingKey = columns[1];
            message.Exchange = columns[2];
            message.MessageCount = int.TryParse(columns[3], out var messageCount) ? messageCount : 0;
            message.Payload = columns[4];
            message.PayloadBytes = int.TryParse(columns[5], out var payloadBytes) ? payloadBytes : 0;
            message.PayloadEncoding = columns[6];
            message.Properties = columns[7];
            message.Redelivered = bool.TryParse(columns[8], out var redelivered) && redelivered;

            return message;
        }

        private static async Task PublishToRabbit(this IApplicationFactory factory, string exchangeName, string routingKey, string payload, CancellationToken cancellationToken)
        {
            var rabbitContainer = factory.Containers.First(x => x.Key == RabbitMqOptions.ContainerName);
            var rabbitRes = await rabbitContainer.Value.ExecAsync(["/bin/bash", "-c",
                $"rabbitmqadmin -u {RabbitMqOptions.UserName} -p {RabbitMqOptions.Password} publish exchange={exchangeName} routing_key={routingKey} payload={payload}"], cancellationToken);
        }
    }
}
