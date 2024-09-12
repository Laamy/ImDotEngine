using System;
using System.Collections.Generic;
using System.Net.Sockets;

// test plugin
#if SERVER
class Anticheat : BasePlugin
{
    public Dictionary<TcpClient, Tuple<float, float>> prevPos = new Dictionary<TcpClient, Tuple<float, float>>();
    
    // some configs
    public float MaxVelocity = 240; // u/s

    public Anticheat()
    {
        DebugLogger.Log("Anticheat", "Started anticheat");
    }

    public override bool OnReceived(TcpClient client, Packet message)
    {
        // TODO: this is just lazy, fix it
        if (SpeedHack.OnReceived(this, client, message))// speed & tp check
            return true;

        return false; // dont cancel
    }

    public override bool OnConnect(TcpClient client)
    {
        prevPos.Add(client, new Tuple<float, float>(0, 0));

        return false; // dont cancel
    }

    public override bool OnDisconnect(TcpClient client)
    {
        prevPos.Remove(client);

        return false; // dont cancel
    }
}
#endif