using System.Net;
using System.Net.Sockets;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using Datadog.Trace;
using Communication;

public class Server
{
    private readonly ILogger<Server> _logger;
    private const int Port = 5000;

    public Server(ILogger<Server> logger)
    {
        _logger = logger;
    }

    public void Run()
    {
        try
        {
            _logger.LogInformation("Starting server on port {Port}...", Port);

            using var listener = new TcpListener(IPAddress.Any, Port);
            listener.Start();
            _logger.LogInformation("Server started and listening on port {Port}.", Port);

            while (true)
            {
                _logger.LogInformation("Waiting for a client connection...");
                using var client = listener.AcceptTcpClient();
                _logger.LogInformation("Client connected!");

                using var networkStream = client.GetStream();

                // Read and process the incoming Protobuf message
                var request = MyMessage.Parser.ParseDelimitedFrom(networkStream);

                // Extract the trace context from headers
                var spanContextExtractor = new SpanContextExtractor();
                var parentContext = spanContextExtractor.Extract(request.Headers, GetHeaderValues);

                // Start a span with the extracted context as the parent
                using var scope = Tracer.Instance.StartActive("server.handle_request", new SpanCreationSettings { Parent = parentContext });
                scope.Span.SetTag("server.port", Port);
                scope.Span.SetTag("request.id", request.Id);
                scope.Span.SetTag("request.content", request.Content);

                // Simulate a CPU-intensive task
                _logger.LogInformation("Starting CPU-intensive task...");
                var result = PerformCpuIntensiveTask(45); // 40th Fibonacci number
                _logger.LogInformation("CPU-intensive task completed. Result: {Result}", result);

                _logger.LogInformation("Received request: ID={Id}, Content={Content}", request.Id, request.Content);

                // Create a response
                var response = new MyMessage
                {
                    Id = request.Id,
                    Content = $"Acknowledged: {request.Content}"
                };

                // Send the response
                response.WriteDelimitedTo(networkStream);
                _logger.LogInformation("Sent response: ID={Id}, Content={Content}", response.Id, response.Content);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while running the server.");
        }
    }

    private long PerformCpuIntensiveTask(int n)
    {
        using (var scope = Tracer.Instance.StartActive("server.cpu_intensive_task"))
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

    private IEnumerable<string> GetHeaderValues(IDictionary<string, string> headers, string name)
    {
        if (headers.TryGetValue(name, out var value))
        {
            return new[] { value };
        }

        return Array.Empty<string>();
    }
}
