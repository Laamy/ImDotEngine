#if CLIENT
namespace ImDotEngine.SDK.UIScene;

using System;
using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;

class MainMenuScene : MenuScene
{
    MenuItem mainGroup;
    MenuRect backgroundRect;

    MenuItem singleplayerMenu;
    MenuItem multiplayerMenu;

    public override void Awake()
    {
        base.Awake();
        
        //    var Services = ClientInstance.instance.Engine.Services;
        //    Services.Add(new LocalPlayerService());
        //    Services.Add(new TerrainMorpherService());
        //    Services.Add(new NetworkService());
        //return;

        var Camera = ClientInstance.GetService<Camera2DService>();
        Camera.AllowZoom = false;
        Camera.AllowMove = false;

        InitComps();
    }

    void InitComps()
    {
        mainGroup?.Dispose();
        this.Add(mainGroup = new MenuItem() { Position = new() });

        var bgSize = new Vector2f(600, 500);
        backgroundRect = new MenuRect(new(), bgSize, new Color(25, 25, 25, 230));
        mainGroup.Add(backgroundRect);

        var buttonSize = new Vector2f(220, 50);
        var spacing = 20f;
        var bottomY = bgSize.Y - buttonSize.Y * 2 - spacing * 2;

        var spButton = new MenuButton(new Vector2f((bgSize.X - buttonSize.X) / 2, bottomY), buttonSize, "Singleplayer", new Color(100, 149, 237), ShowSingleplayerMenu);
        var mpButton = new MenuButton(new Vector2f((bgSize.X - buttonSize.X) / 2, bottomY + buttonSize.Y + spacing), buttonSize, "Multiplayer", new Color(60, 179, 113), ShowMultiplayerMenu);

        mainGroup.Add(spButton);
        mainGroup.Add(mpButton);
    }

    void ShowSingleplayerMenu()
    {
        multiplayerMenu?.Dispose();
        singleplayerMenu?.Dispose();

        singleplayerMenu = new MenuItem();
        mainGroup.Add(singleplayerMenu);

        var buttonSize = new Vector2f(200, 40);
        var spacing = 15f;
        var count = 4;
        var totalHeight = count * buttonSize.Y + (count - 1) * spacing;

        var start = new Vector2f(
            backgroundRect.Size.X - buttonSize.X - 40f,
            100f
        );

        var labels = new[] { "Create World", "Load World", "Delete World", "Open Folder" };

        for (var i = 0; i < labels.Length; ++i)
        {
            var btn = new MenuButton(
                new Vector2f(start.X, start.Y + i * (buttonSize.Y + spacing)),
                buttonSize,
                labels[i],
                new Color(105, 105, 105),
                () => { }
            );
            singleplayerMenu.Add(btn);
        }
    }

    void ShowMultiplayerMenu()
    {
        singleplayerMenu?.Dispose();
        multiplayerMenu?.Dispose();

        multiplayerMenu = new MenuItem();
        mainGroup.Add(multiplayerMenu);

        var buttonSize = new Vector2f(200, 40);
        var spacing = 15f;

        var start = new Vector2f(
            backgroundRect.Size.X - buttonSize.X - 40f,
            100f
        );

        var addServer = new MenuButton(
            start,
            buttonSize,
            "Add Server",
            new Color(72, 61, 139),
            () => { }
        );
        multiplayerMenu.Add(addServer);

        var server1 = new MenuButton(
            start + new Vector2f(0, buttonSize.Y + spacing),
            buttonSize,
            "127.0.0.1:19132",
            new Color(70, 130, 180),
            () => { }
        );
        multiplayerMenu.Add(server1);

        var server2 = new MenuButton(
            start + new Vector2f(0, 2 * (buttonSize.Y + spacing)),
            buttonSize,
            "localhost",
            new Color(100, 149, 237),
            () => {
                NetworkService.Connect("127.0.0.1", 4746);
            }
        );
        multiplayerMenu.Add(server2);
    }

    public override void OnUpdate(RenderWindow ctx)
    {
        base.OnUpdate(ctx);

        if (mainGroup != null)
        {
            var Camera = ClientInstance.GetService<Camera2DService>();
            if (Camera != null)
            {
                mainGroup.Position = Camera.Position + new Vector2f(
                    (Camera.Size.X / 2) - (backgroundRect.Size.X / 2),
                    (Camera.Size.Y / 2) - (backgroundRect.Size.Y / 2)
                );
            }
        }
    }
}
#endif