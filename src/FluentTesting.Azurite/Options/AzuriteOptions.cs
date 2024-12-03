namespace FluentTesting.Azurite.Options
{
    public class AzuriteOptions
    {
        public int? BlobPort { get; set; } = 10_000;

        public int? QueuePort { get; set; } = 10_001;

        public int? TablePort { get; set; } = 10_002;

        public int GuiPort { get; set; } = 8100;

        public string DefaultUserName { get; set; } = "devstoreaccount1";

        public string DefaultPassword { get; set; } = "devstoreaccount1";

        public bool RunAdminTool { get; set; } = true;
    }
}
