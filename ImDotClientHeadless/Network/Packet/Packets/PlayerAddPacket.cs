class PlayerAddPacket : Packet
{
    public string UUID { get; set; }
    public float X { get; set; }
    public float Y { get; set; }

    public PlayerAddPacket() : base(PacketType.PLAYER_ADD) { }
    public PlayerAddPacket(string format) : base(PacketType.PLAYER_ADD) => Decode(format);

    public override void Decode(string format)
    {
        var parts = format.Split(':');

        if (parts[0] != Name)
            throw new System.Exception("Attempt to unpack invalid packet");

        {
            var variables = parts[1].Trim().Split(',');

            UUID = variables[0];

            X = float.Parse(variables[1]);
            Y = float.Parse(variables[2]);
        }
    }

    public override string Encode() => $"{Name}:{UUID},{X},{Y}";
}