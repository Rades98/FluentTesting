using Microsoft.AspNetCore.Authorization;
using Samples.AspApp;
using Samples.AspApp.Repos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddTransient<ISomeRepo, SomeRepo>();

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

await app.RunAsync(default);

public partial class Program() { }

namespace Samples.AspApp
{
	internal record Result(string Message);

	internal record SqlData(string StringVal, int IntVal);
}
