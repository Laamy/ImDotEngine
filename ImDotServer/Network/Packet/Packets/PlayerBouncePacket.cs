class PlayerBouncePacket : Packet
{
    public float X { get; set; }
    public float Y { get; set; }

    public float VX { get; set; }
    public float VY { get; set; }

    public PlayerBouncePacket() : base(PacketType.PLAYER_BOUNCE) { }
    public PlayerBouncePacket(string format) : base(PacketType.PLAYER_BOUNCE) => Decode(format);

    public override void Decode(string format)
    {
        var parts = format.Split(':');

        if (parts[0] != Name)
            throw new System.Exception("Attempt to unpack invalid packet");

        {
            var variables = parts[1].Trim().Split(',');

            X = float.Parse(variables[0]);
            Y = float.Parse(variables[1]);

            VX = float.Parse(variables[2]);
            VY = float.Parse(variables[3]);
        }
    }

    public override string Encode() => $"{Name}:{X},{Y},{VX},{VY}";
}