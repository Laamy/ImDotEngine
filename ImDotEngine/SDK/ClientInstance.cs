using SFML.Window;
using System.Collections.Generic;
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
    public FontRepository FontRepository = new FontRepository();

    public bool AllowPhysics { get; internal set; }
#endif

    // all the servers clientinstance shit
#if SERVER
    public Dictionary<TcpClient, Player> Clients = new Dictionary<TcpClient, Player>();
    public ServerWorld World = new ServerWorld();

    public ushort ServerPort { get; set; }
#endif

    // shared between both
    public Level Level = new Level();
}