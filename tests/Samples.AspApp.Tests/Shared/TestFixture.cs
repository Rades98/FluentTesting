using FluentTesting.Asp;
using FluentTesting.Asp.Authentication;
using FluentTesting.Asp.Extensions;
using FluentTesting.Common.Extensions;
using FluentTesting.Common.Interfaces;
using FluentTesting.Sql;

namespace Samples.AspApp.Tests.Shared;

/// <summary>
/// Example of test fixture with custom options
/// </summary>
public class TestFixture : ITestFixture
{
    public IApplicationFactory ApplicationFactory { get; }

    public string SqlConnectionString { get; private set; } = string.Empty;

    public HttpClient Client { get; }

    public TestFixture()
    {
        ApplicationFactory = new AspApplicationFactoryBuilder<Program>()
            .SetAssertionRegex(@".*Samples\.AspApp\.Tests[\\\/]+(.*?)[\\\/](?:(?![\\\/]).)*$")
            .RegisterServices((services, configuration) =>
                {
                    services.RegisterAuth();
                })
            .UseSql(SqlSeed, (configuration, sqlSettings) =>
            {
                configuration.AddConnectionString("Web", sqlSettings.ConnectionString);

                SqlConnectionString = sqlSettings.ConnectionString;
            })
            .Build();

        Client = ApplicationFactory.GetClient();
    }

    private const string SqlSeed = @"
		CREATE TABLE dbo.SomeTable(
			Id INT PRIMARY KEY IDENTITY(1,1), 
			SomeInt INT NOT NULL, 
			SomeString VARCHAR(30) NOT NULL
		);

		INSERT INTO dbo.SomeTable(SomeInt, SomeString) VALUES (0, 'string');
	";
}