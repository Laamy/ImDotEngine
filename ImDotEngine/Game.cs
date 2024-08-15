#region Includes

using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Management.Instrumentation;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

#endregion

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
        Components.Add(new CameraCursor()); // cursor visualization

        Components.Add(new DebugComponent());

        base.Initialized(); // allow components to initialize

        TargetFramerate = 0; // unlimited FPS
        VSync = false; // fuck vsync, here for completeness

        TargetPhysicsRate = 20; // physics rate at 20

        // other bits and bobs
        {
            // debug stuff
            debugOverlay = Instance.Level.CreateText(LevelLayers.UI, new Vector2f(-250, 10), Color.Red);

            Instance.Level.CreateRectangle(LevelLayers.Background, new Vector2f(-3, -3), new Vector2f(500, 500), new Color(0x20, 0x20, 0x20));// bounds
            Instance.Level.CreateRectangle(LevelLayers.Background, new Vector2f(-260, -3), new Vector2f(250, 275), new Color(0x20, 0x20, 0x20));// in-world menu box
        }

        {
            uint cellSize = 100;

            // grid of shapes for performance debugging
            {
                // texture atlas/object group (not scaled up or down cuz its a fucking square)
                SolidGroup group = new SolidGroup(new TextureAtlas((uint)(cellSize * 11), (uint)(cellSize * 11)));

                group.Position = new Vector2f(0, 0);

                for (int x = 0; x < cellSize; ++x)
                {
                    for (int y = 0; y < cellSize; ++y)
                    {
                        SolidObject square = new SolidObject();

                        square.Position = new Vector2f(y * 11, x * 11);
                        square.Size = new Vector2f(10, 10);
                        //square.Texture = Instance.TextureRepository.GetTexture("Assets\\Texture\\dirt.png");

                        group.AddObject(square);
                    }
                }

                group.Invalidate(); // refresh texture atlas

                Instance.Level.GetLayer(LevelLayers.Foreground).AddObject(group);
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