using DotNet.Testcontainers.Containers;
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
        public static async Task<RabbitMqMessage?> ConsumeRabbitMqMessage(this IApplicationFactory factory, string queue, CancellationToken cancellationToken)
            => (await factory.ConsumeMessage(queue, cancellationToken))?.Stdout.ExtractMessageFromOutput();

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

            var payload = res?.Stdout.ExtractMessageFromOutput()?.Payload;

            return payload is null ? default : JsonSerializer.Deserialize<T>(payload);
        }


        private async static Task<ExecResult?> ConsumeMessage(this IApplicationFactory factory, string queue, CancellationToken cancellationToken)
        {
            ExecResult? res = null;

            await Task.Delay(100, cancellationToken);

            var rabbitContainer = factory.Containers.First(x => x.Key == RabbitMqOptions.ContainerName);

            while (res is null && !cancellationToken.IsCancellationRequested)
            {
                res = await rabbitContainer.Value.ExecAsync(["/bin/bash", "-c", $"rabbitmqadmin -u {RabbitMqOptions.UserName} -p {RabbitMqOptions.Password} get queue={queue} count=1"], cancellationToken);
            }

            return res;
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
    }
}
