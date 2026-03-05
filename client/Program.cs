using Grpc.Net.Client;
using System.Net.Sockets;
using Compute;
using DotNetEnv;

class Program
{
    static async Task Main()
    {
        Env.Load();

        var debug = Environment.GetEnvironmentVariable("DEBUG") == "True";

        GrpcChannel channel;

        if (debug)
        {
            var address = Environment.GetEnvironmentVariable("GRPC_SERVER_ADDRESS")
                          ?? "http://127.0.0.1:50051";

            channel = GrpcChannel.ForAddress(address);
        }
        else
        {
            var udsEndPoint = new UnixDomainSocketEndPoint("/tmp/compute.sock");

            var handler = new SocketsHttpHandler
            {
                ConnectCallback = async (context, cancellationToken) =>
                {
                    var socket = new Socket(
                        AddressFamily.Unix,
                        SocketType.Stream,
                        ProtocolType.Unspecified
                    );

                    await socket.ConnectAsync(udsEndPoint, cancellationToken);
                    return new NetworkStream(socket, ownsSocket: true);
                }
            };

            channel = GrpcChannel.ForAddress(
                "http://localhost",
                new GrpcChannelOptions
                {
                    HttpHandler = handler
                }
            );
        }

        var client = new ComputeService.ComputeServiceClient(channel);

        var request = new MatrixRequest
        {
            Rows = 2,
            Cols = 2
        };

        request.Data.AddRange(new double[] { 1, 2, 3, 4 });

        var reply = await client.SumMatrixAsync(request);
        Console.WriteLine(reply.Result);
    }
}