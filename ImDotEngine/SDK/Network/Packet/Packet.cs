using System.Net.Sockets;

abstract class Packet
{
    public PacketType Type { get; protected set; }
    
    // this is actually a function in memory
    public string Name
    {
        get => Type.ToString();
    }

    public Packet(PacketType type)
    {
        Type = type;
    }

    public virtual void Decode(string format) { }// nothing..
    public virtual string Encode() => $"{Name}:";

    public virtual void Initialize() { } // incase you need something forcefully set in packets

    // incase they try to use ToString
    public override string ToString() => Encode();
}