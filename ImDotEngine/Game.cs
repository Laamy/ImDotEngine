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
using ImDotEngine.SDK.UIScene;
internal class Game : GameEngine
{
    public Game() => Start();

    public SolidText debugOverlay;
    public Camera2DService Camera;

    public override void LoadAssets()
    {
        base.LoadAssets();

        Instance.TextureRepository.Initialize();
        Instance.MaterialRepository.Initialize();
        Instance.AudioRepository.Initialize();
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

            Services.Add(new DefaultWindowBinds());
            Services.Add(Camera = new Camera2DService());
            Services.Add(new SoundService());
            //Components.Add(new CameraCursor());

            Services.Add(new DebugService());

            Services.Add(new UISceneService());

            DebugLogger.Log("Components", $"Initialized Components");
        }

        {
            ClientInstance.GetService<UISceneService>().SetUIScene(new MainMenuScene());
        }

        base.Initialized();

        TargetFramerate = 0;
        VSync = false;

        TargetPhysicsRate = 20;

        //{
        //    debugOverlay = Instance.Level.CreateText(LevelLayers.UI, new Vector2f(-250, 10), Color.Red);
        //
        //    Instance.Level.CreateRectangle(LevelLayers.Background, new Vector2f(-260, -3), new Vector2f(250, 275), new Color(0x20, 0x20, 0x20));// in-world menu box
        //}
        Instance.Level.Warm();

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
        //debugOverlay.Text =
        //    $"Frames: {CurrentFPS}\n" +
        //    $"PhysicSteps: {CurrentPPS}\n" +
        //    $"\n" +
        //    $"Layers: {Instance.Level.Layers.Length}\n" +
        //    $"Block Count: {Instance.Level.GetLayer(LevelLayers.ForeBlocks).Count}\n";
    }

    protected override void OnUpdate(RenderWindow ctx)
    {
        //ctx.Clear(new Color(0, 72, 105)); // clear buffer ready for next frame
        var camera = Services.OfType<Camera2DService>().FirstOrDefault();

        //ctx.Clear(new Color(0, 72, 105));

        Instance.Level.ApplyShader("skybox.frag", (skybox) =>
        {
            skybox.SetUniform("u_basecolor", new Vec3(0.0f, 72.0f / 255.0f, 105.0f / 255.0f));
        });

        Instance.Level.Draw(ctx); // draw scene

        Instance.Level.ApplyShader("fog.frag", (fogFrag) =>
        {
            var fogNoise = Instance.TextureRepository.GetTexture("Assets\\Noise\\FogTexture_Tile.png");
            fogNoise.Repeated = true;
            
            fogFrag.SetUniform("u_fog_noise", fogNoise);
            fogFrag.SetUniform("u_fog_width", 100.0f);
            fogFrag.SetUniform("u_fog_strength", 0.1f);
            fogFrag.SetUniform("u_fog_color_factor", new Vec3(1, 1, 1));
        });

        base.OnUpdate(ctx); // call to allow components access to them

        foreach (BaseService component in Services)
            if (!component.isInit)
            {
                component.isInit = true;
                component.Initialized();
            }

        ClientInstance.DoDeffered();
    }
}
#endif