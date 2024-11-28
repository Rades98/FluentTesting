namespace FluentTesting.Mongo.Seed;

/// <summary>
/// Extensions for generating mongo seed in Javascript commands.
/// </summary>
public static class SeedBuilderExtensions
{
    /// <summary>
    /// Creates user with read/write rights on the database.
    /// </summary>
    public static SeedBuilder CreateUser(this SeedBuilder builder, string username, string password)
    {
        var script =
            $$"""
				db.createUser({
				    user: "{{username}}",
				    pwd: "{{password}}",
				    roles: [
				        {
				            role: "readWrite",
				            db: "{{builder.DatabaseName}}"
				        }
				    ]
				})
			""";

        builder.Commands.Add(script);
        return builder;
    }

    /// <summary>
    /// Creates user with read/write rights with credentials: guest // guest .
    /// </summary>
    public static SeedBuilder CreateGuestUser(this SeedBuilder builder) => builder.CreateUser("guest", "guest");

    /// <summary>
    /// Inserts specific json document into the collection.
    /// </summary>
    public static SeedBuilder InsertDocument(this SeedBuilder builder, string collectionName, string jsonDocument)
    {
        var script =
            $$"""
				db.{{collectionName}}.insertOne({{jsonDocument}});
			""";

        builder.Commands.Add(script);
        return builder;
    }

    public static string Build(this SeedBuilder builder) => string.Join(Environment.NewLine, builder.Commands);
}