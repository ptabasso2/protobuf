using Microsoft.Extensions.Logging;
using Communication;
using Datadog.Trace;
using Datadog.Trace.Configuration;

class Program
{
    static void Main(string[] args)
    {
        // Configure Datadog tracer
        var tracerSettings = TracerSettings.FromDefaultSources();
        tracerSettings.ServiceName = "protobuf-client";
        Tracer.Configure(tracerSettings);

        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });

        var logger = loggerFactory.CreateLogger<Client>();

        // Create and run the client
        var client = new Client(logger);
        client.Run();
    }
}
