class PlayerRemovePacket : Packet
{
    public string UUID { get; set; }

    public PlayerRemovePacket() : base(PacketType.PLAYER_REMOVE) { }
    public PlayerRemovePacket(string format) : base(PacketType.PLAYER_REMOVE) => Decode(format);

    public override void Decode(string format)
    {
        var parts = format.Split(':');

        if (parts[0] != Name)
            throw new System.Exception("Attempt to unpack invalid packet");

        {
            var variables = parts[1].Trim().Split(',');

            UUID = variables[0];
        }
    }

    public override string Encode() => $"{Name}:{UUID}";
}