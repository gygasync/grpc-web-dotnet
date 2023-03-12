using gRPCSampleServer.Services;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

builder.Services.AddGrpc();
builder.Services.AddCors(
    o =>
        o.AddPolicy(
            "AllowAll",
            builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithExposedHeaders(
                        "Grpc-Status",
                        "Grpc-Message",
                        "Grpc-Encoding",
                        "Grpc-Accept-Encoding"
                    );
            }
        )
);
builder.Services.AddGrpcReflection();

var app = builder.Build();

app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });

app.UseCors();
app.MapGrpcReflectionService();
app.MapGrpcService<EchoService>().RequireCors("AllowAll");
app.MapGet(
    "/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909"
);

app.Run();
