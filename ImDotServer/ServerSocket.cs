using System;

using System.Net.Sockets;
using System.Net;

using System.Threading.Tasks;

using System.Text;

class ServerSocket
{
    private TcpListener _listener;

    public event Action<TcpClient, byte[]> OnReceived;
    public event Action<TcpClient> OnDisconnect;
    public event Action<TcpClient> OnConnect;

    public ServerSocket(IPAddress ip, int port)
    {
        _listener = new TcpListener(ip, port);
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

            OnConnect.Invoke(client);

            Task.Run(() => HandleClient(client));
        }
    }

    private async Task HandleClient(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[1024];

        while (true)
        {
            try
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                if (bytesRead == 0)
                    break;

                byte[] data = new byte[bytesRead];
                Array.Copy(buffer, data, bytesRead); // clone data

                OnReceived.Invoke(client, data);
            }
            catch
            {
                OnDisconnect.Invoke(client);
                client.Close();
            }
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