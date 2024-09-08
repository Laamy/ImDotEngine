using System.Net.Sockets;
using System.Text;
using System;

class ClientSocket
{
    private TcpClient     _client;
    private NetworkStream _stream;

    public event Action<byte[]> OnReceived;

    public ClientSocket(string ip, int port)
    {
        _client = new TcpClient(ip, port);
        _stream = _client.GetStream();

        StartReceiving();
    }

    private async void StartReceiving()
    {
        byte[] buffer = new byte[1024];

        while (true)
        {
            try
            {
                int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);

                if (bytesRead <= 0)
                    break;

                byte[] data = new byte[bytesRead];
                Array.Copy(buffer, data, bytesRead); // clone

                OnReceived.Invoke(data);
            }
            catch { };
        }
    }

    public void Send(string message)
    {
        byte[] buffer = Encoding.ASCII.GetBytes(message);
        _stream.Write(buffer, 0, buffer.Length);
    }

    public void Close()
    {
        _stream?.Close();
        _client?.Close();
    }
}