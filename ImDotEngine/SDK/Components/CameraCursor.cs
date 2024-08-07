using SFML.Graphics;
using SFML.System;
using System;

class CameraCursor : BaseComponent
{
    private Camera2D _camera;

    public Camera2D Camera
    {
        get => _camera;
        set => _camera = value;
    }

    public override void OnUpdate(RenderWindow ctx)
    {
        // visualize cursor
        CircleShape shape = new CircleShape();

        shape.FillColor = new Color(255, 255, 255, 128); // temp colour
        shape.Radius = 1;
        shape.Position = Camera.CursorToWorld(ctx, ClientInstance.GetSingle().GuiData.CursorPos) - new Vector2f(shape.Radius, shape.Radius);

        ctx.Draw(shape);
    }
}