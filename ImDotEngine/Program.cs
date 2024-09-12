using System;

internal class Program
{
    [STAThread]
    static void Main(string[] args)
    {
#if SERVER
        CommandUtility cmdLine = new CommandUtility(args);
        GameServer server = new GameServer(cmdLine);
#elif CLIENT
        new Game();
#endif
    }
}