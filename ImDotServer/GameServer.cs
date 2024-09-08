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
    private ushort worldSeed;

    public GameServer()
    {
        worldSeed = (ushort)random.Next(0, 65536);

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
        {
            var handshake = ImPacket.Create<HandshakePacket>();
            handshake.WorldSeed = worldSeed;

            _socket.Send(client, handshake.Encode());
        }

        DebugLogger.Log("GameServer", $"Connected client, assigned UUID: {player.UUID}");

        // tell other clients to add a new player
        {
            var playeradd = ImPacket.Create<PlayerAddPacket>();
            playeradd.UUID = player.UUID;
            playeradd.X = player.X;
            playeradd.Y = player.Y;

            Broadcast(playeradd.Encode(), client);
        }
    }

    private void OnDisconnect(TcpClient client)
    {
        string UUID = clients[client].UUID;

        DebugLogger.Log("GameServer", $"Disconnected client, {UUID}");

        // remove from dictionary tell clients to remove player & close connection
        clients.Remove(client);
        {
            var playerremove = ImPacket.Create<PlayerRemovePacket>();
            playerremove.UUID = UUID;

            Broadcast(playerremove.Encode(), client);
        }
        client?.Close();
    }

    /// <summary>
    /// MAKE SURE TO HEAVILY VERIFY EVERYTHING SENT FROM CLIENT TO CLIENT!
    /// </summary>
    private void OnReceived(TcpClient client, byte[] arg2)
    {
        string message = Encoding.UTF8.GetString(arg2);

        // verify the packet should be broadcasted
        {
            var packet = ImPacket.Decode(message);

            // only these specific packets are allowed to be transferred between clients..
            if (packet is PlayerUpdatePacket playerupdate)
            {
                // malformed uuid check
                if (playerupdate.UUID != "0")
                {
                    OnDisconnect(client);
                    return;
                }

                // malformed coords check
                if (Math.Abs(playerupdate.X) > 300000 ||
                    Math.Abs(playerupdate.Y) > 300000)
                {
                    OnDisconnect(client);
                    return;
                }

                // update any info
                playerupdate.UUID = clients[client].UUID;

                // broadcast to clients
                Broadcast(playerupdate.Encode(), client);
            }
        }
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