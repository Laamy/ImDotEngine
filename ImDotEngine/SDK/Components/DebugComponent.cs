using SFML.System;
using SFML.Window;
using System.Linq;

class DebugComponent : BaseComponent
{
    public override void Initialized()
    {
        var Instance = ClientInstance.GetSingle();

        //SolidConvex triangle = new SolidConvex();
        //
        //triangle.LoadShape(BasicShapes.Triangle, 10);
        //triangle.Position = new Vector2f(-100, -100);
        //
        //Instance.Level.GetLayer(LevelLayers.Foreground).Add(triangle);
    }

    public override void KeyPressed(KeyEventArgs e)
    {
        var Instance = ClientInstance.GetSingle();

        if (e.Code == Keyboard.Key.B)
        {
            // Instance -> Engine -> Components -> Camera2D
            var Camera = Instance.Engine.Components.OfType<Camera2D>().FirstOrDefault();

            Camera.Position = new Vector2f(0, 0);
            Camera.Zoom = 1;
        }

        if (e.Code == Keyboard.Key.C)
        {
            Instance.Engine.Size = new Vector2u(800, 600);
        }
    }
}