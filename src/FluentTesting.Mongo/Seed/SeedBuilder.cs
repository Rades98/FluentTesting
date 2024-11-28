namespace FluentTesting.Mongo.Seed;

/// <summary>
/// Builder for Mongo seed
/// </summary>
public class SeedBuilder
{
    /// <summary>
    /// Builder for Mongo seed
    /// </summary>
    public SeedBuilder(string databaseName)
    {
        DatabaseName = databaseName;
        Commands.Add($"""
						db = db.getSiblingDB("{databaseName}");
					""");
    }

    public string DatabaseName { get; }

    internal List<string> Commands { get; } = [];
}