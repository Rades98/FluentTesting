using Samples.Worker.RabbitMq;
using Samples.Worker.RabbitMq.ConsumptionHandlingServices;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<RabbitPublishingWorker>();
builder.Services.AddHostedService<RabbitConsumerService>();
builder.Services.AddTransient<IConsumptionHandler, ConsumptionHandler>();

builder.Services.Configure<RabbitConnectionOptions>(conf => builder.Configuration.GetSection("RabbitConnectionOptions").Bind(conf));

var host = builder.Build();
host.Run();


public partial class Program;