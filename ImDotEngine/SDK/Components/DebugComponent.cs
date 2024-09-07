using SFML.Window;

class DebugComponent : BaseComponent//RigidBodyComponent
{
    public DebugComponent()
    {
        //BodyRoot = new SolidObject();
        //
        //BodyRoot.Position = new Vector2f(200, -100);
        //BodyRoot.Size = new Vector2f(50, 100);
        //BodyRoot.Color = Color.Red;
        //
        //prevPos = BodyRoot.Position;
        //curPos = BodyRoot.Position;

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