namespace FluentTesting.Common.Interfaces
{
    /// <summary>
    /// Tests fixture
    /// </summary>
    public interface ITestFixture
    {
        public IApplicationFactory ApplicationFactory { get; }
    }
}
