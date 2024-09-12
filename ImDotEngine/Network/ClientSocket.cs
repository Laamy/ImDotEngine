using System.Net.Sockets;
using System.Text;
using System;
using System.Linq;
using System.Threading.Tasks;

class ClientSocket
{
    private TcpClient     _client;
    private NetworkStream _stream;

    public event Func<byte[], Task> OnReceived;

    public ClientSocket(string ip, int port)
    {
        _client = new TcpClient(ip, port);
        _client.ReceiveTimeout = 5000; // 5 seconds
        _client.SendTimeout = 5000;
        _client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
        _client.Client.NoDelay = true; // FUCL B ATCHING

        _stream = _client.GetStream();

        StartReceiving();
    }

    private async void StartReceiving()
    {
        byte[] buffer = new byte[1024];// large to allow multiple packets at once

        while (true)
        {
            int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);

            if (bytesRead <= 0)
                break;

            byte[] data = new byte[bytesRead];
            Array.Copy(buffer, data, bytesRead); // clone

            if (data.Contains((byte)'\n'))
            {
                // multiple packets in 1 message cuz sockets are fucking gay
                string[] messages = Encoding.ASCII.GetString(data).Trim().Split('\n');

                foreach (var packet in messages)
                {
                    // TODO: i might aswell jsut return the fuckiung string at this point
                    var freshData = Encoding.ASCII.GetBytes(packet);

                    OnReceived?.Invoke(freshData);
                }
            }
            else OnReceived?.Invoke(data);
        }
    }

    public void Send(string message)
    {
        if (!_client.Connected)
        {
            DebugLogger.Log("ClientSocket", "Client not connected");
            return;
        }

        try
        {
            byte[] buffer = Encoding.ASCII.GetBytes(message + "\n");
            _stream.Write(buffer, 0, buffer.Length);
        }
        catch
        {
            Close();
        }
    }

    public void Close()
    {
        _stream?.Close();
        _client?.Close();
    }
}