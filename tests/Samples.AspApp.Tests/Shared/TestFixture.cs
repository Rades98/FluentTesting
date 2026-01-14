using FluentTesting.Asp;
using FluentTesting.Asp.Authentication;
using FluentTesting.Asp.Extensions;
using FluentTesting.Azurite;
using FluentTesting.Common.Extensions;
using FluentTesting.Common.Interfaces;
using FluentTesting.Redis;
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
            .RegisterServices((services, configuration) =>
            {
                services.RegisterAuth();
            })
            .UseSql(SqlSeed, (configuration, sqlSettings) =>
            {
                configuration.AddConnectionString("Web", sqlSettings.ConnectionString);

                SqlConnectionString = sqlSettings.ConnectionString;
            }, opts =>
            {
                opts.Database = "TestDb";
                opts.WaitStrategy = new()
                {
                    IntervalSeconds = 20,
                    RetryCount = 3,
                    TimeoutSeconds = 120,
                };

                opts.ContainerConfig = new()
                {
                    CPUs = 1,
                    MemoryMb = 1024
                };
            })
            .UseAzurite(
            (configuration, settings) =>
            {
                configuration.AddConnectionString("BlobStorageConnection", settings.ConnectionString);
            },
            opts =>
            {
                opts.BlobPort = 1200;
                opts.QueuePort = 1201;
                opts.TablePort = 1202;

                opts.Version = "3.34.0";

                opts.BlobSeed =
            [
                new()
                {
                    Name = "photos",
                    Files =
                    [
                    new()
                    {
                        Path = Path.Combine(Directory.GetCurrentDirectory(), "Shared", "asd.png"),
                        Name = "Some name"
                    }
                    ]
                }
            ];
            }
            )
            .UseRedis(
            (configuration, settings) =>
            {
                configuration.AddConnectionString("RedisConnectionString", $"{settings.Url}:{settings.Port}");
            },
            opts =>
            {
                opts.Seed = new()
                {
            { "someKey", "some value :)" },
            { "someKey2", "some value :)" },
            { "someKey3", "some value :)" },
            { "someKey4", "some value :)" },
            { "someKey5", "some value :)" },
                };
            })
            .Build();

        Client = ApplicationFactory.GetClient();
    }

    private const string SqlSeed = @"
		CREATE TABLE dbo.SomeTable(
			Id INT PRIMARY KEY IDENTITY(1,1), 
			SomeInt INT NOT NULL, 
			SomeString VARCHAR(30) NOT NULL,
			SomeNullableString VARCHAR(30),
			SomeBool bit,
      SomeDecimal [decimal](18, 2) NOT NULL
		);

    CREATE TABLE dbo.SomeTableBase(
			Id INT PRIMARY KEY IDENTITY(1,1), 
			SomeBaseInt INT NOT NULL, 
         SomeBaseFloat REAL NOT NULL,
         SomeGuid uniqueidentifier NOT NULL,
         SomeDateOnly Date NOT NULL,
         SomeTimeOnly Time NOT NULL,
         SomeDateTime datetime2 Not NULL,
		);

		INSERT INTO dbo.SomeTable(SomeInt, SomeString, SomeNullableString, SomeBool, SomeDecimal) VALUES (0, 'string', NULL, 0, 1000);
		INSERT INTO dbo.SomeTable(SomeInt, SomeString, SomeNullableString,SomeDecimal) VALUES (1, '0', NULL,0);
		INSERT INTO dbo.SomeTable(SomeInt, SomeString, SomeNullableString,SomeDecimal) VALUES (2, 'string', NULL,0);
		INSERT INTO dbo.SomeTable(SomeInt, SomeString, SomeNullableString,SomeDecimal) VALUES (3, '1', NULL,0);

		INSERT INTO dbo.SomeTableBase(SomeBaseInt, SomeBaseFloat, SomeGuid, SomeDateOnly, SomeTimeOnly, SomeDateTime) VALUES (3, 0.8, 'F0B85BE2-3F18-4E58-9976-0C2D562C7458', '2024-01-09', '10:29:00.0000000', '2024-11-03 14:30:00.0000000');
	";
}