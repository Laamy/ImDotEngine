using System.Net.Sockets;
using System;

#if SERVER
class SpeedHack
{
    static ClientInstance Instance = ClientInstance.GetSingle();

    public static bool OnReceived(Anticheat plugin, TcpClient client, Packet message)
    {
        if (message is PlayerUpdatePacket playerupdate)
        {
            var prev = plugin.prevPos[client];

            var curPos = new Tuple<float, float>(playerupdate.X, playerupdate.Y);
            var curVel = new Tuple<float, float>(playerupdate.VX, playerupdate.VY);

            // super basic position velocity check
            {
                var dx = curPos.Item1 - prev.Item1;
                var dy = curPos.Item2 - prev.Item2;
                float dt = Mathf.Sqrt(dx * dx + dy * dy);
                float dv = Mathf.Sqrt(playerupdate.VX * playerupdate.VX + playerupdate.VY * playerupdate.VY);
                
                dt = Mathf.Abs(dt);
                dv = Mathf.Abs(dv);

                if (dt > plugin.MaxVelocity || // position travel is higher then game threshold
                    dv > plugin.MaxVelocity || // velocity is higher then game threshold
                    (dv + 30) < dx) // velocity is to high compared to position (buffer for movement speed)
                {
                    DebugLogger.Log("Anticheat", $"Suspicious movement detected for {Instance.Clients[client].UUID}, Distance; P:{dt}, V:{dv}");

                    {
                        // cause lagback effect on the desync'd client (or cheater)
                        var playerbounce = ImPacket.Create<PlayerBouncePacket>();

                        // back to old pos
                        playerbounce.X = plugin.prevPos[client].Item1;
                        playerbounce.Y = plugin.prevPos[client].Item2;

                        // reset velocity just incase thats SOMEHOW causing it..
                        playerbounce.VX = 0;
                        playerbounce.VY = 0;

                        plugin.Server.FireClient(playerbounce, client);
                    }

                    //Server.OnDisconnect(client); // forcefully disconnect the client
                    return true; // cancel packet so other clients wont recieve it
                }
            }

            plugin.prevPos[client] = new Tuple<float, float>(playerupdate.X, playerupdate.Y);
        }

        return false; // dont cancel
    }
}
#endif