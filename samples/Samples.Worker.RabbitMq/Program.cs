using Samples.Worker.RabbitMq;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<RabbitPublishingWorker>();

builder.Services.Configure<RabbitConnectionOptions>(conf => builder.Configuration.GetSection("RabbitConnectionOptions").Bind(conf));

var host = builder.Build();
host.Run();


public partial class Program;