namespace Testing.Mongo;

/// <summary>
/// Mongo container settings
/// </summary>
public record MongoContainerSettings(IEnumerable<string> Hosts, int Port, string Username, string Password, string DatabaseName);