using System;

internal class Program
{
    [STAThread]
    static void Main(string[] args)
    {
#if SERVER
        GameServer server = new GameServer();
#elif CLIENT
        new Game();
#endif
    }
}