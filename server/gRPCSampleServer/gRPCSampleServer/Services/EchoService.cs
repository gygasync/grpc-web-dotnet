using Grpc.Core;
using System.Reflection.Metadata.Ecma335;

namespace gRPCSampleServer.Services;

public class EchoService : Echo.EchoBase
{
    private readonly ILogger<EchoService> _logger;
    private static int s_counter = 0;

    public EchoService(ILogger<EchoService> logger)
    {
        _logger = logger;
    }

    public override Task<EchoResponse> EchoRequest(Empty _, ServerCallContext context)
    {
        _logger.LogInformation("Echo Request");
        return Task.FromResult(new EchoResponse { Message = DateTimeOffset.Now.ToString("u") });
    }

    public async override Task EchoStream(
        Empty _,
        IServerStreamWriter<EchoResponse> writer,
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
                    new EchoResponse { Message = DateTimeOffset.Now.ToString("u") }
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
