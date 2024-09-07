using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

// temp and insecure!
class GameServer
{
    private ServerSocket _socket;
    private Dictionary<TcpClient, Player> clients = new Dictionary<TcpClient, Player>();
    private Random random = new Random();
    private int worldSeed;

    public GameServer()
    {
        worldSeed = random.Next(1, 100001);

        _socket = new ServerSocket(IPAddress.Any, 4746);
        _socket.OnConnect += OnConnect;
        _socket.OnDisconnect += OnDisconnect;
        _socket.OnReceived += OnReceived;

        DebugLogger.Log("GameServer", $"Server started on 127.0.0.1:{4746}");
        DebugLogger.Log("GameServer", $"World seed {worldSeed}");

        Console.ReadKey();
    }

    private void OnConnect(TcpClient client)
    {
        // add clients player to dictionary
        Player player = new Player() { UUID = Guid.NewGuid().ToString() };
        clients[client] = player;

        // tell client world seed
        _socket.Send(client, $"SEED:{worldSeed}");

        DebugLogger.Log("GameServer", $"Connected client, assigned UUID: {player.UUID}");

        // tell other clients to add a new player
        Broadcast($"ADD_PLAYER:{player.UUID}\n", client);
    }

    private void OnDisconnect(TcpClient client)
    {
        string UUID = clients[client].UUID;

        DebugLogger.Log("GameServer", $"Disconnected client, {UUID}");

        // remove from dictionary tell clients to remove player & close connection
        clients.Remove(client);
        Broadcast($"REMOVE_PLAYER:{UUID}\n", client);
        client?.Close();
    }

    private void OnReceived(TcpClient client, byte[] arg2)
    {
        string message = Encoding.UTF8.GetString(arg2);

        Broadcast(message, client);
    }

    public void Broadcast(string message, TcpClient excludeClient)
    {
        if (excludeClient == null)
        {
            OnDisconnect(excludeClient);
            return;
        }

        foreach (var client in clients)
        {
            try
            {
                if (client.Key != excludeClient)
                {
                    _socket.Send(client.Key, message);
                }
            }
            catch
            {
                OnDisconnect(client.Key);
            }
        }
    }
}