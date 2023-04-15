using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Console.Microservice.Infrastructure;
using Console.Microservice.ServiceBusConsumer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();

var client = new ServiceBusClient(
    builder.Configuration["ServiceBus:NameSpace"],
    new DefaultAzureCredential(),
    new ServiceBusClientOptions
    {
        TransportType = ServiceBusTransportType.AmqpWebSockets
    });

builder.Services.AddSingleton<ServiceBusClient>(_ => client);
builder.Services.AddSingleton<ServiceBusProcessor>(_
    => client.CreateProcessor(builder.Configuration["ServiceBus:QueueName"], new ServiceBusProcessorOptions()));
builder.Services.AddHostedService<ServiceBusConsumer>();

var app = builder.Build();
app.MapHealthChecks("/healthz", new HealthCheckOptions
{
    // Modify the response from plain text to json.
    ResponseWriter = HealthWriter.WriteResponse
});
app.Run();