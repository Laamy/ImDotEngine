#region Includes

using SFML.Graphics;
using SFML.Graphics.Glsl;
using SFML.System;
using System;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Runtime.CompilerServices;

#endregion

internal class Game : GameEngine
{
    public Game() => Start(); // we've finished so start the app

    // ui stuff
    public SolidText debugOverlay;

    // components
    public Camera2D Camera;

    public override void LoadAssets()
    {
        base.LoadAssets(); // send signal to components

        // TODO: make these classes inherit a repository class
        Instance.TextureRepository.Initialize(); // load all assets
        Instance.MaterialRepository.Initialize(); // load all materials
    }

    public override void Initialized()
    {
        {
            DebugLogger.Log("Components", $"Initializing Components..");

            // some start components
            Components.Add(new DefaultWindowBinds()); // Escape for quit, F11 for fullscreen (inbuilt to the engine)
            Components.Add(Camera = new Camera2D()); // Example of 2D camera
            //Components.Add(new CameraCursor()); // cursor visualization

            Components.Add(new DebugComponent());
            Components.Add(new LocalPlayer());

            DebugLogger.Log("Components", $"Initialized Components");
        }

        base.Initialized(); // allow components to initialize

        TargetFramerate = 0; // unlimited FPS
        VSync = false; // fuck vsync, here for completeness

        TargetPhysicsRate = 20; // physics rate at 20

        // other bits and bobs
        {
            // debug stuff
            debugOverlay = Instance.Level.CreateText(LevelLayers.UI, new Vector2f(-250, 10), Color.Red);

            Instance.Level.CreateRectangle(LevelLayers.Background, new Vector2f(-260, -3), new Vector2f(250, 275), new Color(0x20, 0x20, 0x20));// in-world menu box
        }

        //TerrainGenerator.Seed = 1;

        // basic sample world
        {
            uint cellScale = 128;
            
            // grid of shapes for performance debugging
            {
                for (int cX = 0; cX < 12; ++cX)
                {
                    for (int cY = 0; cY < 12; ++cY)
                    {
                        // texture atlas/object group (not scaled up or down cuz its a fucking square)
                        SolidGroup group = new SolidGroup(new TextureAtlas((uint)(12 * cellScale), (uint)(12 * cellScale)));

                        group.Position = new Vector2f(cX * (cellScale * (cellScale / 33)), cY * (cellScale * (cellScale / 33)));

                        var chunk = TerrainGenerator.GenerateChunk(cX * 12, cY * 12);

                        for (int y = 0; y < 12; ++y)
                        {
                            for (int x = 0; x < 12; ++x)
                            {
                                var block = chunk[y][x];

                                if (block == BlockEnum.Air)
                                    continue;

                                SolidObject chunkBlock = new SolidObject();

                                chunkBlock.Position = new Vector2f(x * cellScale, y * cellScale);
                                chunkBlock.Size = new Vector2f(cellScale, cellScale);

                                var blockResource = BlockRegistry.GetBlock(block);

                                if (blockResource != null) // valid block
                                    chunkBlock.Texture = BlockRegistry.GetBlock(block);

                                group.AddObject(chunkBlock);
                            }
                        }

                        group.Scale = new Vector2f(0.25f, 0.25f);
                        group.Invalidate(); // refresh texture atlas

                        Instance.Level.GetLayer(LevelLayers.ForeBlocks).AddObject(group);
                    }
                }
            }
        }

        DebugLogger.Log("ImDotEngine", $"Game started with settings:" +
            $"\r\n\tTargetFramerate: {TargetFramerate}" +
            $"\r\n\tTargetPhysicsRate: {TargetPhysicsRate}" +
            $"\r\n\tVSync: {VSync}\r\n");
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
            $"Block Count: {Instance.Level.GetLayer(LevelLayers.ForeBlocks).Count}\n";
    }

    protected override void OnUpdate(RenderWindow ctx)
    {
        //ctx.Clear(new Color(0, 72, 105)); // clear buffer ready for next frame
        var camera = Components.OfType<Camera2D>().FirstOrDefault();

        Instance.Level.ApplyShader("skybox.frag", (skybox) =>
        {
            skybox.SetUniform("u_basecolor", new Vec3(0.0f, 72.0f / 255.0f, 105.0f / 255.0f));
        });

        Instance.Level.Draw(ctx); // draw scene

        //Instance.Level.ApplyShader("fog.frag", (fogFrag) =>
        //{
        //    var fogNoise = Instance.TextureRepository.GetTexture("Assets\\Noise\\FogTexture_Tile.png");
        //    fogNoise.Repeated = true;
        //    
        //    fogFrag.SetUniform("u_fog_noise", fogNoise);
        //    fogFrag.SetUniform("u_fog_width", 150.0f);
        //    fogFrag.SetUniform("u_fog_strength", 0.2f);
        //    fogFrag.SetUniform("u_fog_color_factor", new Vec3(1, 1, 1));
        //});

        base.OnUpdate(ctx); // call to allow components access to them
    }
}