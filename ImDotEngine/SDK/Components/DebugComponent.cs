using SFML.System;
using SFML.Window;
using System.Linq;

class DebugComponent : BaseComponent
{
    public DebugComponent()
    {
        DebugLogger.Log("Components", $"Initialized : DebugComponent");
    }

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

        if (e.Code == Keyboard.Key.S)
        {
            DebugConfig.ShowPhysicsDetails = !DebugConfig.ShowPhysicsDetails;
        }
    }
}