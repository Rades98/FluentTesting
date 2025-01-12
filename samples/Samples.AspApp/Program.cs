using Microsoft.AspNetCore.Authorization;
using Samples.AspApp;
using Samples.AspApp.Adapters;
using Samples.AspApp.Repos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddTransient<ISomeRepo, SomeRepo>();
builder.Services.AddSingleton<AppBlobServiceClient>();
builder.Services.AddTransient<IBlobAdapter, BlobAdapter>();

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
	var file = await adapter.GetFileAsync("photos", "asd.png", ct);

	return file is not null ? Results.File(file, "image/png") : Results.NotFound();
});


await app.RunAsync(default);

public partial class Program;

namespace Samples.AspApp
{
	internal record Result(string Message);

	internal record SqlData(string StringVal, int IntVal);
}
