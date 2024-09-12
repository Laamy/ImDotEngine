class LoginPacket : Packet
{
    public string Username { get; set; }

    public LoginPacket() : base(PacketType.LOGIN) { } // blank/empty packet
    public LoginPacket(string format) : base(PacketType.LOGIN) => Decode(format); // redirect to decode

    public override void Decode(string format)
    {
        var parts = format.Split(':');

        if (parts[0] != Name)
            throw new System.Exception("Attempt to unpack invalid packet");

        {
            var variables = parts[1].Trim().Split(',');

            Username = variables[0];
        }
    }

    public override string Encode() => $"{Name}:{Username}";
}