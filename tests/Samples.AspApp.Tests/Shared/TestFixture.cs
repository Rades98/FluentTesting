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
									Path = Path.Combine(Directory.GetCurrentDirectory(), "Shared", "asd.png")
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
			SomeString VARCHAR(30) NOT NULL
		);

		INSERT INTO dbo.SomeTable(SomeInt, SomeString) VALUES (0, 'string');
	";
}