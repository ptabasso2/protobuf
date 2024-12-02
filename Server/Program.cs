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
        tracerSettings.ServiceName = "protobuf-server";
        Tracer.Configure(tracerSettings);
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });

        var logger = loggerFactory.CreateLogger<Server>();

        // Create and run the server
        var server = new Server(logger);
        server.Run();
    }
}
