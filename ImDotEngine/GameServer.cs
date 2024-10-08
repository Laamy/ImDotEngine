﻿#region Includes

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

#endregion

#if SERVER
class GameServer
{
    private ServerSocket _socket;
    private Random random = new Random();

    ClientInstance Instance = ClientInstance.GetSingle();

    // space in console by using small ID's
    public string GenUUID()
    {
        //var guid = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        //return guid.Substring(0, guid.Length - 2);

        return CID.New().ToString();
    }

    public GameServer(CommandUtility args)
    {
        // NOTE: clean this up later into its own set of classes maybe
        if (args.HasFlag("h") || args.HasFlag("help"))
        {
            Console.WriteLine("Usage: dot_server.exe [options]\r\n");

            Console.WriteLine("Arguments look similar to this");
            Console.WriteLine("# Description/info");
            Console.WriteLine("# Defaults or extras");
            Console.WriteLine("# argument requirements");
            Console.WriteLine("--key default\r\n");

            Console.WriteLine("# The maximum number of players before the server starts rejecting connections");
            Console.WriteLine("# set to 0 for no player-cap (not recommmended)");
            Console.WriteLine("# (Positiive Integers, 0 & above)");
            Console.WriteLine("--max-players 32\r\n");

            Console.WriteLine("# Which IPv4 port the server should listen to");
            Console.WriteLine("# (Positiive Integers, 1-65535)");
            Console.WriteLine("--port 4746\r\n");

            Console.WriteLine("# The seed the world will use to randomize");
            Console.WriteLine("# Dont set for random");
            Console.WriteLine("# (Positiive Integers, 0-65535)");
            Console.WriteLine("--seed 1\r\n");

            Console.WriteLine("# Tell the client to generate chunks unless the server specifically tells it otherwise");
            Console.WriteLine("# (true,false)");
            Console.WriteLine("--client-side-chunk-generation true");

            //Console.WriteLine("# The folder the world will be saved to");
            //Console.WriteLine("# (Any string)");
            //Console.WriteLine("--world-name world\r\n");

            // cancel server start
            Console.ReadKey();
            return;
        }

        Instance.World.Seed = (ushort)args.GetNumber("seed", (ushort)random.Next(0, 65536));
        Instance.ServerPort = (ushort)args.GetNumber("port", 4746);
        Instance.MaxPlayers = args.GetNumber("max-players", 32);
        Instance.ClientSideChunkGeneration = args.GetBool("client-side-chunk-generation", true);

        _socket = new ServerSocket(IPAddress.Any, Instance.ServerPort);
        _socket.OnConnect += OnConnect;
        _socket.OnDisconnect += OnDisconnect;
        _socket.OnReceived += OnReceived;

        // start plugins here
        PluginList.StartPlugins(this);

        DebugLogger.Log("GameServer", $"Server started on 127.0.0.1:{Instance.ServerPort}");
        DebugLogger.Log("GameServer", $"World seed {Instance.World.Seed}");

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

            if (packet is WorldStatePacket worldstate)
            {
                // TODO: unwrap this into the level
                DebugLogger.Warn($"world state packet recieved with {worldstate.states.Count} states");
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
                playerupdate.UUID = Instance.Clients[client].UUID;

                // broadcast to clients
                FireAllClientsEx(playerupdate, client);
            }
        }
    }

    private async Task OnConnect(TcpClient client)
    {
        if (Instance.Clients.Count >= Instance.MaxPlayers)
        {
            client?.Close();
            return;
        }

        // add clients player to dictionary
        Player player = new Player() { UUID = GenUUID() };
        Instance.Clients[client] = player;

        {
            bool cancel = false;

            // give every plugin a chance to access the event even if 1 has cancelled it
            foreach (var plugin in PluginList.plugins)
                plugin.OnConnect(client);

            if (cancel)
            {
                Instance.Clients.Remove(client);
                return;
            }
        }

        // tell client info & send client playerlist
        {
            var handshake = ImPacket.Create<HandshakePacket>();

            handshake.WorldSeed = Instance.World.Seed;
            handshake.UUID = player.UUID;
            
            // apply some settings
            handshake.AllowChunkGen = Instance.ClientSideChunkGeneration;

            FireClient(handshake, client);

            foreach (var _client in Instance.Clients)
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

        string UUID = Instance.Clients[client].UUID;

        DebugLogger.Log("GameServer", $"Disconnected client, {UUID}");

        // remove from dictionary tell clients to remove player & close connection
        Instance.Clients.Remove(client);
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

        foreach (var client in Instance.Clients)
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
#endif