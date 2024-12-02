using System;
using System.Net.Sockets;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using Datadog.Trace;
using Communication;

public class Client
{
    private readonly ILogger<Client> _logger;
    private const string ServerAddress = "server";
    private const int Port = 5000;

    public Client(ILogger<Client> logger)
    {
        _logger = logger;
    }

    public void Run()
    {
        try
        {
            // Start a Datadog trace
            using (var scope = Tracer.Instance.StartActive("client.send_message"))
            {
                scope.Span.SetTag("client", "protobuf-client");

                // Simulate a CPU-intensive task
                _logger.LogInformation("Starting CPU-intensive task...");
                var result = PerformCpuIntensiveTask(45); // 40th Fibonacci number
                _logger.LogInformation("CPU-intensive task completed. Result: {Result}", result);

                // Prepare the message
                var message = new MyMessage
                {
                    Id = 1,
                    Content = "Hello, Server!",
                };

                // Inject the trace context into the headers
                var spanContextInjector = new SpanContextInjector();
                spanContextInjector.Inject(message.Headers, SetHeaderValues, scope.Span.Context);

                // Establish connection to the server
                using var client = new TcpClient();
                _logger.LogInformation("Connecting to server...");
                client.Connect(ServerAddress, Port);
                _logger.LogInformation("Connected to server.");

                // Send the Protobuf message
                using var networkStream = client.GetStream();
                _logger.LogInformation("Sending message: ID={Id}, Content={Content}", message.Id, message.Content);
                message.WriteDelimitedTo(networkStream);

                // Await server response
                var response = MyMessage.Parser.ParseDelimitedFrom(networkStream);
                _logger.LogInformation("Received response: ID={Id}, Content={Content}", response.Id, response.Content);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while communicating with the server.");
        }
    }

    private long PerformCpuIntensiveTask(int n)
    {
        using (var scope = Tracer.Instance.StartActive("client.cpu_intensive_task"))
        {
            scope.Span.SetTag("algorithm", "naive_fibonacci");
            scope.Span.SetTag("input.n", n.ToString());

            return NaiveFibonacci(n);
        }
    }

    private long NaiveFibonacci(int n)
    {
        if (n <= 1)
            return n;

        return NaiveFibonacci(n - 1) + NaiveFibonacci(n - 2); // Exponential complexity: O(2^n)
    }

    private void SetHeaderValues(IDictionary<string, string> headers, string name, string value)
    {
        headers[name] = value;
    }
}
