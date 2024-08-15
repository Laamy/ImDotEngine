#region Includes

using SFML.Graphics;
using SFML.System;

using System.Linq;

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
        base.LoadAssets();

        DebugLogger.Log("Assets", "Caching assets..");
        Instance.TextureRepository.Initialize(); // load all assets
        DebugLogger.Log("Assets", $"Cached all assets");

        DebugLogger.Log("Materials", "Compiling shaders..");
        Instance.Materials.Initialize(); // load all assets
        DebugLogger.Log("Materials", $"Compiled all shaders!");
    }

    public override void Initialized()
    {
        {
            DebugLogger.Log("Components", $"Initializing Components..");

            // some start components
            Components.Add(new DefaultWindowBinds()); // Escape for quit, F11 for fullscreen (inbuilt to the engine)
            Components.Add(Camera = new Camera2D()); // Example of 2D camera
            Components.Add(new CameraCursor()); // cursor visualization

            Components.Add(new DebugComponent());

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
                for (int cX = 0; cX < 10; ++cX)
                {
                    for (int cY = 0; cY < 10; ++cY)
                    {
                        // texture atlas/object group (not scaled up or down cuz its a fucking square)
                        SolidGroup group = new SolidGroup(new TextureAtlas((uint)(24 * cellScale), (uint)(24 * cellScale)));

                        group.Position = new Vector2f(cX * (cellScale * (cellScale / 19)), cY * (cellScale * (cellScale / 19)));

                        var chunk = TerrainGenerator.GenerateChunk(cX * 24, cY * 24);

                        for (int y = 0; y < 24; ++y)
                        {
                            for (int x = 0; x < 24; ++x)
                            {
                                var block = chunk[y][x];

                                if (block == BlockEnum.Air)
                                    continue;

                                SolidObject chunkBlock = new SolidObject();

                                chunkBlock.Position = new Vector2f(x * cellScale, y * cellScale);
                                chunkBlock.Size = new Vector2f(cellScale, cellScale);

                                // TODO: loop over enum and find matching textures instead
                                if (block == BlockEnum.Dirt)
                                    chunkBlock.Texture = Instance.TextureRepository.GetTexture("Texture\\dirt.png");

                                if (block == BlockEnum.Grass)
                                    chunkBlock.Texture = Instance.TextureRepository.GetTexture("Texture\\grass.png");

                                if (block == BlockEnum.Stone)
                                    chunkBlock.Texture = Instance.TextureRepository.GetTexture("Texture\\stone.png");

                                if (block == BlockEnum.Grassy_Stone)
                                    chunkBlock.Texture = Instance.TextureRepository.GetTexture("Texture\\grassy_stone.png");

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

        DebugLogger.Log("Components", $"Game started with settings:" +
            $"\r\n\tTargetFramerate: {TargetFramerate}" +
            $"\r\n\tTargetPhysicsRate: {TargetPhysicsRate}" +
            $"\r\n\tVSync: {VSync}");

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
            $"Block Count: {Instance.Level.GetLayer(LevelLayers.ForeBlocks).Count}\n";

        // TODO: Implement game physics
    }

    protected override void OnUpdate(RenderWindow ctx)
    {
        //ctx.Clear(new Color(0, 72, 105)); // clear buffer ready for next frame
        {
            var camera = Components.OfType<Camera2D>().FirstOrDefault();

            var skybox = Instance.Materials.GetTexture("skybox.frag");

            skybox.SetUniform("u_res_y", Size.Y);

            RectangleShape skyboxScreen = new RectangleShape();

            View temp = new View(new FloatRect(camera.Position, (Vector2f)camera.Size));
            temp.Zoom(camera.Zoom);

            skyboxScreen.Position = temp.Center - new Vector2f(
                (Size.X / 2) * camera.Zoom,
                (Size.Y / 2) * camera.Zoom
            );
            skyboxScreen.Size = (Vector2f)Size;
            skyboxScreen.Scale = new Vector2f(camera.Zoom, camera.Zoom);

            skyboxScreen.Draw(ctx, new RenderStates(skybox));
        }

        Instance.Level.Draw(ctx); // draw scene


        // TODO: Implement game rendering

        base.OnUpdate(ctx); // call to allow components access to them
    }
}