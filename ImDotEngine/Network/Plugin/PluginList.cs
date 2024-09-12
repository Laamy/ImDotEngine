using System.Collections.Generic;

#if SERVER
class PluginList
{
    public static List<BasePlugin> plugins = new List<BasePlugin>();

    public static void StartPlugins(GameServer server)
    {
        DebugLogger.Log("PluinList", "Initializing plugins..");

        plugins.Add(new Anticheat());

        foreach (var plugin in plugins)
            plugin.Server = server;

        DebugLogger.Log("PluinList", "Initialized plugins");
    }
}
#endif