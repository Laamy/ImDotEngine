using SFML.Window;

internal class ClientInstance
{
    public static ClientInstance instance;
    public static ClientInstance GetSingle()
    {
        if (instance == null)
            instance = new ClientInstance();

        return instance;
    }

    public GameEngine Engine;
    public VideoMode VideoMode;

    public Level Level = new Level();
    public GuiData GuiData = new GuiData();

    // assets n shit
    public BundleRepository BundleRepository = new BundleRepository();

    public MaterialRepository MaterialRepository = new MaterialRepository();
    public TextureRepository TextureRepository = new TextureRepository();
    public FontRepository FontRepository = new FontRepository();
}