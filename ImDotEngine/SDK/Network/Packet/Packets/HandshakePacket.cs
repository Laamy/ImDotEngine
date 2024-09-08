class HandshakePacket : Packet
{
    public string UUID {  get; set; }
    public string Username {  get; set; }
    public ushort WorldSeed {  get; set; }//65536

    public HandshakePacket() : base(PacketType.HANDSHAKE) { } // blank/empty packet
    public HandshakePacket(string format) : base(PacketType.HANDSHAKE) => Decode(format); // redirect to decode

    public override void Decode(string format)
    {
        var parts = format.Split(':');

        if (parts[0] != Name)
            throw new System.Exception("Attempt to unpack invalid packet");

        {
            var variables = parts[1].Trim().Split(',');

            UUID = variables[0];
            WorldSeed = ushort.Parse(variables[1]);
        }
    }

    public override string Encode() => $"{Name}:{UUID},{WorldSeed}";
}