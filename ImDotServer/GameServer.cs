using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class GameServer
{
    private ServerSocket _socket;
    private Random random = new Random();

    // for plugins to access
    public Dictionary<TcpClient, Player> clients = new Dictionary<TcpClient, Player>();
    public ushort WorldSeed;

    // https://stackoverflow.com/questions/1458468/youtube-like-guid
    // gives me more space in the console
    public string GenUUID()
    {
        var guid = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

        return guid.Substring(0, guid.Length - 2);
    }

    public GameServer()
    {
        WorldSeed = (ushort)random.Next(0, 65536);

        _socket = new ServerSocket(IPAddress.Any, 4746);
        _socket.OnConnect += OnConnect;
        _socket.OnDisconnect += OnDisconnect;
        _socket.OnReceived += OnReceived;

        // start plugins here
        PluginList.StartPlugins(this);

        DebugLogger.Log("GameServer", $"Server started on 127.0.0.1:{4746}");
        DebugLogger.Log("GameServer", $"World seed {WorldSeed}");

        Console.ReadKey();
    }

    /// <summary>
    /// MAKE SURE TO HEAVILY VERIFY EVERYTHING SENT FROM CLIENT TO CLIENT!
    /// </summary>
    private async Task OnReceived(TcpClient client, byte[] arg2)
    {
        string message = Encoding.UTF8.GetString(arg2);

        // verify the packet should be broadcasted
        {
            var packet = ImPacket.Decode(message);

            {
                bool cancel = false;

                // give every plugin a chance to access the event even if 1 has cancelled it
                foreach (var plugin in PluginList.plugins)
                    plugin.OnReceived(client, packet);

                if (cancel)
                    return;
            }

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

                // malformed coords check
                if (Math.Abs(playerupdate.VX) > 300000 ||
                    Math.Abs(playerupdate.VY) > 300000)
                {
                    OnDisconnect(client);
                    return;
                }

                // update any info
                playerupdate.UUID = clients[client].UUID;

                // broadcast to clients
                FireAllClientsEx(playerupdate, client);
            }
        }
    }

    private async Task OnConnect(TcpClient client)
    {
        // add clients player to dictionary
        Player player = new Player() { UUID = GenUUID() };
        clients[client] = player;

        {
            bool cancel = false;

            // give every plugin a chance to access the event even if 1 has cancelled it
            foreach (var plugin in PluginList.plugins)
                plugin.OnConnect(client);

            if (cancel)
            {
                clients.Remove(client);
                return;
            }
        }

        // tell client info & send client playerlist
        {
            var handshake = ImPacket.Create<HandshakePacket>();

            handshake.WorldSeed = WorldSeed;
            handshake.UUID = player.UUID;

            FireClient(handshake, client);

            foreach (var _client in clients)
            {
                if (_client.Key == client)
                    continue;

                var playeradd = ImPacket.Create<PlayerAddPacket>();

                playeradd.UUID = _client.Value.UUID;//OOPS
                playeradd.X = _client.Value.X;
                playeradd.Y = _client.Value.Y;

                FireClient(playeradd, client);
            }
        }

        DebugLogger.Log("GameServer", $"Connected client, assigned UUID: {player.UUID}");

        // tell other clients to add a new player
        {
            var playeradd = ImPacket.Create<PlayerAddPacket>();

            playeradd.UUID = player.UUID;
            playeradd.X = player.X;
            playeradd.Y = player.Y;

            FireAllClientsEx(playeradd, client);
        }
    }

    public async Task OnDisconnect(TcpClient client)
    {
        bool cancel = false;

        {
            // give every plugin a chance to access the event even if 1 has cancelled it
            foreach (var plugin in PluginList.plugins)
                plugin.OnDisconnect(client);
        }

        string UUID = clients[client].UUID;

        DebugLogger.Log("GameServer", $"Disconnected client, {UUID}");

        // remove from dictionary tell clients to remove player & close connection
        clients.Remove(client);
        {
            var playerremove = ImPacket.Create<PlayerRemovePacket>();
            playerremove.UUID = UUID;

            if (!cancel)
                FireAllClientsEx(playerremove, client);
        }
        client?.Close();
    }

    public void FireClient(Packet packet, TcpClient client)
    {
        if (client == null)
        {
            OnDisconnect(client);
            return;
        }

        _socket.Send(client, packet.Encode() + "\n");
    }

    public void FireAllClientsEx(Packet packet, TcpClient excludeClient)
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
                    FireClient(packet, client.Key);
                }
            }
            catch
            {
                OnDisconnect(client.Key);
            }
        }
    }
}