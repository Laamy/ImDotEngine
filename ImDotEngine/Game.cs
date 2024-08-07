#region Includes

using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;

#endregion

// TODO: gridify the world so I can process less things!!!!
internal class Game : GameEngine
{
    public Game() => Start(); // we've finished so start the app

    // ui stuff
    public SolidText debugOverlay;

    // components
    public Camera2D Camera;

    public override void Initialized()
    {
        // some start components
        Components.Add(new DefaultWindowBinds()); // Escape for quit, F11 for fullscreen (inbuilt to the engine)
        Components.Add(Camera = new Camera2D()); // Example of 2D camera
        Components.Add(new CameraCursor() { Camera = Camera }); // cursor visualization

        base.Initialized(); // allow components to initialize

        TargetFramerate = 1000; // unlimited FPS
        TargetPhysicsRate = 20; // physics rate at 20

        // other bits and bobs
        {
            // debug stuff
            debugOverlay = Instance.Level.CreateText(LevelLayers.UI, new Vector2f(-250, 10), Color.White);

            Instance.Level.CreateRectangle(LevelLayers.Background, new Vector2f(-3, -3), new Vector2f(500, 500), new Color(0x20, 0x20, 0x20));// bounds
            Instance.Level.CreateRectangle(LevelLayers.Background, new Vector2f(-260, -3), new Vector2f(250, 275), new Color(0x20, 0x20, 0x20));// in-world menu box
        }

        // 7 FPS
        // 34 FPS
        // 58 FPS
        {
            for (int x = 0; x < 200; x++)
            {
                for (int z = 0; z < 200; z++)
                {
                    Instance.Level.CreateCircle(LevelLayers.Foreground, new Vector2f(12 * x, 12 * z), 4, Color.Red);
                }
            }
        }

        // TODO: Implement scene
    }

    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate(); // call to allow components access to them

        // update the debug crap
        debugOverlay.Text =
            $"Frames: {CurrentFPS}\n" +
            $"PhysicSteps: {CurrentPPS}\n" +
            $"\n" +
            $"Layers: {Instance.Level.Layers.Length}\n" +
            $"Foreground Count: {Instance.Level.GetLayer(LevelLayers.Foreground).Count}\n";

        // TODO: Implement game physics
    }

    protected override void OnUpdate(RenderWindow ctx)
    {
        ctx.Clear(new Color(0x10, 0x10, 0x10)); // clear buffer ready for next frame

        Instance.Level.Draw(ctx); // draw scene

        // TODO: Implement game rendering

        base.OnUpdate(ctx); // call to allow components access to them
    }
}