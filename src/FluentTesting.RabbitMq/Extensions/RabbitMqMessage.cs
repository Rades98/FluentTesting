namespace FluentTesting.RabbitMq.Extensions
{
    public class RabbitMqMessage
    {
        public string? RoutingKey { get; set; }

        public string? Exchange { get; set; }

        public int MessageCount { get; set; }

        public string? Payload { get; set; }

        public int PayloadBytes { get; set; }

        public string? PayloadEncoding { get; set; }

        public string? Properties { get; set; }

        public bool? Redelivered { get; set; }
    }
}
