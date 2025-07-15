using SFML.Graphics;
using SFML.Window;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

internal class ClientInstance
{
    public static ClientInstance instance;
    public static ClientInstance GetSingle()
    {
        if (instance == null)
            instance = new ClientInstance();

        return instance;
    }

    // all the clients clientinstance shit
#if CLIENT
    public GameEngine Engine;
    public VideoMode VideoMode;

    public GuiData GuiData = new GuiData();

    // assets n shit
    public BundleRepository BundleRepository = new BundleRepository();

    public MaterialRepository MaterialRepository = new MaterialRepository();
    public TextureRepository TextureRepository = new TextureRepository();
    public AudioRepository AudioRepository = new AudioRepository();
    public FontRepository FontRepository = new FontRepository();

    public bool AllowPhysics { get; internal set; }

    public static RenderWindow GetRenderWindow()
    {
        if (instance.Engine == null)
            throw new NullReferenceException("Engine is not initialized. Cannot get RenderWindow.");

        return instance.Engine.window;
    }

    public static T GetService<T>() where T : BaseService
    {
        var Instance = GetSingle();
        var Engine = Instance.Engine;
        var Components = Engine.Services;

        T serivce = Components.OfType<T>().FirstOrDefault();

        if (serivce == null)
            return null; // throw new KeyNotFoundException($"Service of type {typeof(T).Name} not found in the engine services.");

        return serivce;
    }

    private static readonly List<Action> actions = new();
    public static void DeferTask(Action value) => actions.Add(value);
    public static void DoDeffered()
    {
        foreach (var action in actions)
        {
            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                DebugLogger.Log("DeferredTask", $"Error executing deferred task: {ex.Message}");
            }
        }
        actions.Clear();
    }
#endif

    // all the servers clientinstance shit
#if SERVER
    public Dictionary<TcpClient, Player> Clients = new();
    public ServerWorld World = new();

    public ushort ServerPort { get; set; }
    public float MaxPlayers { get; set; }

    // some basic packet related settings
    public bool ClientSideChunkGeneration { get; set; }
#endif

    // shared between both
    public Level Level = new();

    // the ECS stuff & a context for the game/instance
    public SimpleRegistry EntityRegistry { get; set; }
    public EntityContext GameContext { get; set; }
}