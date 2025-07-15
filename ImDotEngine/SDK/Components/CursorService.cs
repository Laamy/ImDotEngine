using SFML.Graphics;
using SFML.System;
using System.Linq;

#if CLIENT
class CursorService : BaseService
{
    public const int Radius = 1;

    SolidGroup shape;

    public CursorService()
    {
        DebugLogger.Log("Components", $"Initialized : CameraCursor");
    }

    public override void Initialized()
    {
        shape = new SolidGroup(new TextureAtlas(100, 100));
        
        
    }

    public override void OnUpdate(RenderWindow ctx)
    {
        // get the client instance
        var Instance = ClientInstance.GetSingle();

        // get the engine from the client instance
        var Engine = Instance.Engine;

        // get the components list from the engine
        var Components = Engine.Services;

        // get the camera component
        Camera2DService Camera = Components.OfType<Camera2DService>().FirstOrDefault();

        // visualize cursor
        shape.Position = Camera.ScreenToWorld(ctx, ClientInstance.GetSingle().GuiData.CursorPos) - new Vector2f(Radius, Radius);

        //shape.Draw(ctx);
    }
}
#endif