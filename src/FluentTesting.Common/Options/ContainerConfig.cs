namespace FluentTesting.Common.Options
{
    public class ContainerConfig
    {
        public required int MemoryMb { get; set; }

        public required decimal CPUs { get; set; }

        public TimeSpan? DelayBeforeInit { get; set; }
    }
}
