using Samples.Worker.RabbitMq.Contracts;

namespace Samples.Worker.RabbitMq.ConsumptionHandlingServices
{
    public interface IConsumptionHandler
    {
        public Task<bool> HandleMessageAsync(RabbitMessage message, CancellationToken cancellationToken);
    }
}
