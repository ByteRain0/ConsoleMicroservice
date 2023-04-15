using Azure.Messaging.ServiceBus;
namespace Console.Microservice.ServiceBusConsumer;
public class ServiceBusConsumer : BackgroundService
{
    private readonly ServiceBusClient _client;
    private readonly ServiceBusProcessor _processor;
    public ServiceBusConsumer(ServiceBusClient client, ServiceBusProcessor processor)
    {
        _client = client;
        _processor = processor;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _processor.ProcessMessageAsync += MessageHandler;
        _processor.ProcessErrorAsync += ErrorHandler;
        await _processor.StartProcessingAsync(stoppingToken);
    }
    private async Task MessageHandler(ProcessMessageEventArgs args)
    {
        System.Console.WriteLine($"Received: {args.Message.Body}");
        await args.CompleteMessageAsync(args.Message);
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        System.Console.WriteLine(args.Exception.ToString());
        return Task.CompletedTask;
    }

    public override async void Dispose()
    {
        await _processor.StopProcessingAsync();
        await _processor.DisposeAsync();
        await _client.DisposeAsync();
        base.Dispose();
    }
}