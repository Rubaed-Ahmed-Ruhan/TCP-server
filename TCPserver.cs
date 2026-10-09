using System.Net;
using System.Net.Sockets;
using System.Text;

namespace WEB;

public class TcpServer
{
    private readonly int _port;

    public TcpServer(int port) => _port = port;

    public async Task Start()
    {
        var listener = new TcpListener(IPAddress.Any, _port);
        listener.Start();
        Console.WriteLine($"Server started: http://localhost:{_port}");

        while (true)
        {
            var client = await listener.AcceptTcpClientAsync();
            _ = Response(client);   // await নেই, যাতে একসাথে অনেক client সামলানো যায়
        }
    }

    public async Task Response(TcpClient client)
    {
        using (client)
        {
            using var ss = client.GetStream();

            var array = new byte[1024];
            var load = await ss.ReadAsync(array);
            var translate = Encoding.UTF8.GetString(array, 0, load);

            Console.WriteLine(translate);

            // নিচের অংশটাই এতক্ষণ তোমার কোডে ছিল না
            string body = "<h1>Hello from my C# server!</h1>";
            string response =
                "HTTP/1.1 200 OK\r\n" +
                "Content-Type: text/html; charset=utf-8\r\n" +
                $"Content-Length: {Encoding.UTF8.GetByteCount(body)}\r\n" +
                "Connection: close\r\n" +
                "\r\n" +
                body;

            await ss.WriteAsync(Encoding.UTF8.GetBytes(response));
        }
    }
} 