using System;
using System.Text;

class GameClient
{
    private ClientSocket socket;
    private string myUuid;
    private bool connected = false; // temp

    public void SendPosition(float x, float y) => socket.Send($"UPDATE_POSITION:{myUuid}.{x},{y}");

    public GameClient(string ip, int port)
    {
        socket = new ClientSocket(ip, port);
        connected = true;

        socket.OnReceived += OnMessage;

        //Task.Factory.StartNew(RecieveMessages);
        Console.ReadKey();
    }

    // clean message then pass to ProcessMessage
    private void OnMessage(byte[] obj) => ProcessMessage(Encoding.ASCII.GetString(obj).Trim());

    public void ProcessMessage(string message)
    {
        DebugLogger.Log("DEBUG", $"Raw; {message}");

        string[] parts = message.Split(':');

        var type = parts[0].ToString();
        
        // TODO: move packets to actual files & packet files with events/overrides
        if (type == "SEED")
        {
            int seed = int.Parse(parts[1]);
            DebugLogger.Log("ClientPackets", $"Recieved world seed {seed}");
        }
        else if (type == "REMOVE_PLAYER")
        {
            string uuid = parts[1];
            DebugLogger.Log("ClientPackets", $"Lost player {uuid}");
        }
        else if (type == "ADD_PLAYER")
        {
            string uuid = parts[1];
            DebugLogger.Log("ClientPackets", $"Recieved player {uuid}");
        }
        else if (type == "UPDATE_POSITION")
        {
            string uuid = parts[1];
            string[] pos = parts[2].Split(',');

            float x = float.Parse(pos[0]);
            float y = float.Parse(pos[1]);

            DebugLogger.Log("ClientPackets", $"Player {uuid} moved to {x},{y}");
        }
        else
        {
            DebugLogger.Log("ClientPackets", $"Invalid packet type {type}");
        }
    }
}