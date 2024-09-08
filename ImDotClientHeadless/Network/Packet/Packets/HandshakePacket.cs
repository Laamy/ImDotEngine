class HandshakePacket : Packet
{
    // 65536
    public ushort WorldSeed {  get; set; }

    public HandshakePacket() : base(PacketType.HANDSHAKE) { } // blank/empty packet
    public HandshakePacket(string format) : base(PacketType.HANDSHAKE) => Decode(format); // redirect to decode

    public override void Decode(string format)
    {
        var parts = format.Split(':');

        if (parts[0] != Name)
            throw new System.Exception("Attempt to unpack invalid packet");

        {
            var variables = parts[1].Trim().Split(',');

            WorldSeed = ushort.Parse(variables[0]);
        }
    }

    public override string Encode() => $"{Name}:{WorldSeed}";
}