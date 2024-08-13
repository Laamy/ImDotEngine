using SFML.Graphics;
using SFML.System;
using System;
using System.Linq;

class CameraCursor : BaseComponent
{
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
        CircleShape shape = new CircleShape();

        shape.FillColor = new Color(255, 255, 255, 128); // temp colour
        shape.Radius = 1;
        shape.Position = Camera.CursorToWorld(ctx, ClientInstance.GetSingle().GuiData.CursorPos) - new Vector2f(shape.Radius, shape.Radius);

        ctx.Draw(shape);
    }
}