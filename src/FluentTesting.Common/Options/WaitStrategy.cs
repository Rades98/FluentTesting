namespace FluentTesting.Common.Options
{
    public class WaitStrategy
    {
        public ushort? RetryCount { get; set; }

        public int? IntervalSeconds { get; set; }

        public int? TimeoutSeconds { get; set; }
    }
}
