using Samples.Worker.RabbitMq.Contracts;

namespace Samples.Worker.RabbitMq.ConsumptionHandlingServices
{

    public static class StaticRabbitHandlerState
    {
        public static RabbitMessage RabbitMessage { get; set; } = null!;
    }
}
