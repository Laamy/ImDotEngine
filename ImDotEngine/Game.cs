#region Includes

using SFML.Graphics;
using SFML.Graphics.Glsl;
using SFML.System;
using System;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Runtime.CompilerServices;

#endregion

#if CLIENT
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

        // NOTE: make these classes inherit a repository class
        Instance.TextureRepository.Initialize(); // load all assets
        Instance.MaterialRepository.Initialize(); // load all materials
        Instance.AudioRepository.Initialize(); // load all sound effects & music
    }

    public override void Initialized()
    {
        {
            DebugLogger.Log("EntityComponents", $"Initializing ECS (Entity Component System).");

            Instance.EntityRegistry = new SimpleRegistry();

            Instance.GameContext = new EntityContext(Instance.EntityRegistry);

            DebugLogger.Log("EntityComponents", $"Initialized ECS.");
        }

        {
            DebugLogger.Log("Components", $"Initializing Components..");

            // some start components
            Components.Add(new DefaultWindowBinds()); // default game keybinds you would expect to exist
            Components.Add(Camera = new Camera2D()); // movable camera for the scene
            //Components.Add(new CameraCursor()); // cursor visualization

            Components.Add(new DebugComponent()); // debug stuff
            Components.Add(new LocalPlayer()); // the actual player
            Components.Add(new TerrainMorpherComponent()); // ability to morph terrain
            Components.Add(new SoundComponent()); // sound effects & music

            // this accesses terrain morpher early on
            Components.Add(new NetworkComponent());

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

        DebugLogger.Log("ImDotEngine", $"Game started with settings:" +
            $"\r\n\tTargetFramerate: {TargetFramerate}" +
            $"\r\n\tTargetPhysicsRate: {TargetPhysicsRate}" +
            $"\r\n\tVSync: {VSync}\r\n");
    }
    
    protected override void OnFixedUpdate()
    {
        if (Instance.AllowPhysics)
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

        //ctx.Clear(new Color(0, 72, 105));

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
#endif