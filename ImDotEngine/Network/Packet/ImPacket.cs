using System;
using System.Collections.Generic;

class ImPacket
{
    private static readonly Dictionary<string, Func<Packet>> factory = new Dictionary<string, Func<Packet>>()
    {
        { nameof(PacketType.HANDSHAKE), () => new HandshakePacket() },
        { nameof(PacketType.PLAYER_UPDATE), () => new PlayerUpdatePacket() },
        { nameof(PacketType.PLAYER_ADD), () => new PlayerAddPacket() },
        { nameof(PacketType.PLAYER_REMOVE), () => new PlayerRemovePacket() },
        { nameof(PacketType.PLAYER_BOUNCE), () => new PlayerBouncePacket() },
        { nameof(PacketType.WORLD_STATE), () => new WorldStatePacket() },
    };

    public static T Create<T>() where T : Packet, new()
    {
        T packet = new T();

        packet.Initialize();

        return packet;
    }

    public static T Create<T>(string format) where T : Packet, new()
    {
        T packet = new T();

        packet.Initialize();
        packet.Decode(format);

        return packet;
    }

    public static Packet Decode(string format)
    {
        var packetName = format.Split(':')[0];

        if (factory.TryGetValue(packetName, out var Creator))
        {
            var packet = Creator();

            packet.Initialize();
            packet.Decode(format);

            return packet;
        }

        throw new Exception($"Unexpected packet ID {packetName}");
    }
}