using System;
using System.Text;

class GameClient
{
    private ClientSocket socket;
    private string myUuid;
    private bool connected = false; // temp

    public void SendPosition(float x, float y)
    {
        var packet = ImPacket.Create<PlayerUpdatePacket>();

        packet.X = x;
        packet.Y = y;

        socket.Send(packet.Encode());
    }

    public GameClient(string ip, int port)
    {
        socket = new ClientSocket(ip, port);
        connected = true;

        socket.OnReceived += OnMessage;

        //Task.Factory.StartNew(RecieveMessages);
        Console.ReadKey();
    }

    // clean message then pass to ProcessMessage
    private void OnMessage(byte[] obj) => ProcessMessage(ImPacket.Decode(Encoding.ASCII.GetString(obj).Trim()));

    public void ProcessMessage(Packet packet)
    {
        if (packet is HandshakePacket handshake)
        {
            DebugLogger.Log("ClientPackets", $"Recieved world seed {handshake.WorldSeed}");
        }
        else if (packet is PlayerRemovePacket playerremove)
        {
            DebugLogger.Log("ClientPackets", $"Lost player {playerremove.UUID}");
        }
        else if (packet is PlayerAddPacket playeradd)
        {
            DebugLogger.Log("ClientPackets", $"Recieved player {playeradd.UUID} at {playeradd.X},{playeradd.Y}");
        }
        else if (packet is PlayerUpdatePacket playerupdate)
        {
            DebugLogger.Log("ClientPackets", $"Player {playerupdate.Name} moved to {playerupdate.X},{playerupdate.Y}");
        }
        else
        {
            DebugLogger.Log("ClientPackets", $"Invalid packet type {packet.Name}");
            throw new Exception("Unexpected packet, check console for details");
        }
    }
}