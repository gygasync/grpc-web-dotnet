using Grpc.Core;

namespace gRPCSampleServer.Services;

public class EchoService : Echo.EchoBase
{
    private readonly ILogger<EchoService> _logger;
    private static int s_counter = 0;

    public EchoService(ILogger<EchoService> logger)
    {
        _logger = logger;
    }

    public async override Task EchoStream(
        Empty _,
        IServerStreamWriter<EchoStreamResponse> writer,
        ServerCallContext context
    )
    {
        var streamNumber = Interlocked.Increment(ref s_counter);
        _logger.LogInformation($"Stream #{streamNumber} started");

        while (!context.CancellationToken.IsCancellationRequested)
        {
            try
            {
                await writer.WriteAsync(
                    new EchoStreamResponse { Message = DateTimeOffset.Now.ToString("u") }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed writing data to stream", ex.Message);
            }
            finally
            {
                await Task.Delay(1000);
            }
        }

        _logger.LogInformation($"Stream #{streamNumber} closed");
    }
}
