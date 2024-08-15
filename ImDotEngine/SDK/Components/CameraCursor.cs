using SFML.Graphics;
using SFML.System;
using System.Linq;

class CameraCursor : BaseComponent
{
    public const int Radius = 1;

    SolidGroup shape;

    public CameraCursor()
    {
        DebugLogger.Log("Components", $"Initialized : CameraCursor");
    }

    public override void Initialized()
    {
        shape = new SolidGroup(new TextureAtlas(100, 100));

        {
            SolidCircle circle = new SolidCircle(Radius * 50);

            circle.Color = new Color(255, 255, 255, 128);

            shape.AddObject(circle);
            shape.Invalidate();
        }
    }

    public override void OnUpdate(RenderWindow ctx)
    {
        // get the client instance
        var Instance = ClientInstance.GetSingle();

        // get the engine from the client instance
        var Engine = Instance.Engine;

        // get the components list from the engine
        var Components = Engine.Components;

        // get the camera component
        Camera2D Camera = Components.OfType<Camera2D>().FirstOrDefault();

        // visualize cursor
        shape.Position = Camera.CursorToWorld(ctx, ClientInstance.GetSingle().GuiData.CursorPos) - new Vector2f(Radius, Radius);

        //shape.Draw(ctx);
    }
}