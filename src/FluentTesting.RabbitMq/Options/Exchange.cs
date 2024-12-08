namespace FluentTesting.RabbitMq.Options
{
    public class Exchange
    {
        public string ExchangeName { get; set; } = "";

        public string ExchangeType { get; set; } = "topic";

        public IList<string> RoutingKeys { get; set; } = [];
    }
}
