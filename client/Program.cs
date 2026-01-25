using Grpc.Net.Client;
using System.Net.Sockets;
using Compute;

class Program
{
    static async Task Main()
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

        var channel = GrpcChannel.ForAddress(
            "http://localhost", // required dummy
            new GrpcChannelOptions
            {
                HttpHandler = handler
            }
        );

        var client = new ComputeService.ComputeServiceClient(channel);

        var request = new MatrixRequest
        {
            Rows = 2,
            Cols = 2
        };
        request.Data.AddRange([1.0, 2.0, 3.0, 4.0]);

        var reply = await client.SumMatrixAsync(request);
        Console.WriteLine(reply.Result);
    }
}
