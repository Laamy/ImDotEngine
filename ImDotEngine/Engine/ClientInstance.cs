using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Diagnostics.SymbolStore;

internal class ClientInstance
{
    public static ClientInstance instance;
    public static ClientInstance GetSingle()
    {
        if (instance == null)
            instance = new ClientInstance();

        return instance;
    }

    public RenderWindow RenderWindow;
    public VideoMode VideoMode;

    public Level Level = new Level();
    public FontRepository FontRepository = new FontRepository();
    public TextureRepository TextureRepository = new TextureRepository();
    public GuiData GuiData = new GuiData();
}