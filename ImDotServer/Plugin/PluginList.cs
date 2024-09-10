using System.Collections.Generic;

class PluginList
{
    public static List<BasePlugin> plugins = new List<BasePlugin>();

    public static void StartPlugins(GameServer server)
    {
        plugins.Add(new Anticheat());

        foreach (var plugin in plugins)
            plugin.Server = server;
    }
}