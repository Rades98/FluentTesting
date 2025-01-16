using Microsoft.AspNetCore.Authorization;
using Samples.AspApp;
using Samples.AspApp.Adapters.Blob;
using Samples.AspApp.Adapters.Redis;
using Samples.AspApp.Repos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddTransient<ISomeRepo, SomeRepo>();
builder.Services.AddSingleton<AppBlobServiceClient>();
builder.Services.AddTransient<IBlobAdapter, BlobAdapter>();
builder.Services.AddTransient<IRedisAdapter, RedisAdapter>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.MapGet("anonym", () => Results.Ok(new Result("Ok")));
app.MapGet("auth", [Authorize] () => Results.Ok(new Result("Ok")));
app.MapGet("sql", async (ISomeRepo repo, CancellationToken ct) =>
{
	var (IntValue, StringValue) = await repo.GetDataAsync(ct);

	return Results.Ok(new SqlData(StringValue, IntValue));
});
app.MapPut("sql", async (ISomeRepo repo, CancellationToken ct) =>
{
	var res = await repo.UpdateDataAsync(ct);

	return Results.Ok(res);
});

app.MapGet("file", async (IBlobAdapter adapter, CancellationToken ct) =>
{
	var file = await adapter.GetFileAsync("photos", "Some name", ct);

	return file is not null ? Results.File(file, "image/png") : Results.NotFound();
});

app.MapGet("redis", async (IRedisAdapter adapter, CancellationToken ct) =>
{
	var entry = await adapter.GetValueAsync("someKey");

	return string.IsNullOrEmpty(entry) ? Results.NotFound() : Results.Ok(entry);

});

app.MapPut("redis", async (IRedisAdapter adapter, CancellationToken ct) =>
{
	await adapter.SetValueAsync("someKey", "some new value");

	return Results.Accepted();
});

await app.RunAsync(default);

public partial class Program;

namespace Samples.AspApp
{
	internal record Result(string Message);

	internal record SqlData(string StringVal, int IntVal);
}
