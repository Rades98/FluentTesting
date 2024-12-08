namespace Samples.Worker.RabbitMq.Contracts
{
    public class RabbitMessage
    {
        public required string Name { get; set; }

        public required string SurName { get; set; }

        public required DateTime BirthDate { get; set; }

        public required string Email { get; set; }
    }
}
