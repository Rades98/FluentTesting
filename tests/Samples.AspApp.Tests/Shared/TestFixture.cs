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
				opts.Port = 1200;
			})
			.UseAzurite(
				(configuration, settings) =>
				{
					configuration.AddConnectionString("BlobStorageConnection", settings.ConnectionString);
				},
				opts =>
				{
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
			SomeBool bit
		);

		INSERT INTO dbo.SomeTable(SomeInt, SomeString, SomeNullableString, SomeBool) VALUES (0, 'string', NULL, 0);
		INSERT INTO dbo.SomeTable(SomeInt, SomeString, SomeNullableString) VALUES (1, '0', NULL);
		INSERT INTO dbo.SomeTable(SomeInt, SomeString, SomeNullableString) VALUES (2, 'string', NULL);
		INSERT INTO dbo.SomeTable(SomeInt, SomeString, SomeNullableString) VALUES (3, '1', NULL);
	";
}