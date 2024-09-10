using System.Net.Sockets;

abstract class BasePlugin
{
    public GameServer Server { get; set; }

    public virtual bool OnReceived(TcpClient client, Packet message)
    {
        return false;
    }

    public virtual bool OnConnect(TcpClient client)
    {
        return false;
    }

    public virtual bool OnDisconnect(TcpClient client)
    {
        return false;
    }
}