using System;

using System.Net.Sockets;
using System.Net;

using System.Threading.Tasks;

using System.Text;

class ServerSocket
{
    private TcpListener _listener;

    public event Func<TcpClient, byte[], Task> OnReceived;
    public event Func<TcpClient, Task> OnDisconnect;
    public event Func<TcpClient, Task> OnConnect;

    public ServerSocket(IPAddress ip, int port)
    {
        _listener = new TcpListener(ip, port);
        _listener.Server.ReceiveTimeout = 5000; // 5 seconds
        _listener.Server.SendTimeout = 5000;
        _listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
        _listener.Server.NoDelay = true; // batching packets is gay as FUCK

        _listener.Start();

        StartAcceptingClients();
    }

    private async void StartAcceptingClients()
    {
        while (true)
        {
            TcpClient client = await _listener.AcceptTcpClientAsync();

            if (client == null)
                    continue;

            OnConnect?.Invoke(client);

            Task.Run(() => HandleClient(client));
        }
    }
    
    private async Task HandleClient(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[1024];

        while (true)
        {
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

            if (bytesRead == 0)
                break;

            byte[] data = new byte[bytesRead];
            Array.Copy(buffer, data, bytesRead); // clone data

            string message = Encoding.ASCII.GetString(data);

            // handle multiple packets per read being in the network stream
            if (message.Contains("\n"))
            {
                string[] packets = message.Trim().Split('\n');

                foreach(var packet in packets)
                {
                    byte[] bytes = new byte[packet.Length];
                    bytes = Encoding.ASCII.GetBytes(packet);

                    OnReceived?.Invoke(client, bytes);
                }

                continue;
            }

            OnReceived?.Invoke(client, data);
        }
    }

    public void Send(TcpClient client, string message)
    {
        byte[] buffer = Encoding.ASCII.GetBytes(message);
        NetworkStream stream = client.GetStream();
        stream.Write(buffer, 0, buffer.Length);
    }

    public void Stop() => _listener.Stop();
}