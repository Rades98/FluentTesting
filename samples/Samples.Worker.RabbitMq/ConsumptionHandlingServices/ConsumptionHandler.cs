using Samples.Worker.RabbitMq.Contracts;

namespace Samples.Worker.RabbitMq.ConsumptionHandlingServices
{
    internal class ConsumptionHandler : IConsumptionHandler
    {
        public Task<bool> HandleMessageAsync(RabbitMessage message, CancellationToken cancellationToken)
        {
            StaticRabbitHandlerState.RabbitMessage = message;

            return Task.FromResult(true);
        }
    }
}
