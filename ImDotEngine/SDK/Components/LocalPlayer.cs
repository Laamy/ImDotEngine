using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Diagnostics;
using System.Linq;

class LocalPlayer : BaseComponent
{
    private SolidObject Player;
    public Vector2f Velocity = new Vector2f(0, 15);

    private Vector2f prevPos;
    private Vector2f curPos;

    const float Gravity = 1;

    public bool DebugLine = true;

    public override void OnFixedUpdate()
    {
        var Instance = ClientInstance.GetSingle();

        prevPos = curPos;

        Velocity.Y += Gravity;

        ResolveCollisions();

        curPos += Velocity;
        Player.Position = curPos;

    }

    private void ResolveCollisions()
    {
        var Instance = ClientInstance.GetSingle();

        //Console.WriteLine("Getting nearby tiles");
        //var nearbyBlocks = Instance.Level.GetLayer(LevelLayers.BackBlocks).GetNearbyObjects(Player.Position, 100);
        //
        //Console.WriteLine(nearbyBlocks.Count());

        //foreach (var block in nearbyBlocks)
        //{
        //    if (block != null)
        //    {
        //        Velocity = new Vector2f(0, 1); // reset temp
        //    }
        //}
    }

    public override void OnUpdate(RenderWindow ctx)
    {
        var Instance = ClientInstance.GetSingle();
        
        float timeSinceLastTick = (float)Instance.GuiData.StepTime.ElapsedMilliseconds / 1000;
        float tickInterval = 1.0f / Instance.Engine.TargetPhysicsRate;

        float lerpFactor = Mathf.Clamp(timeSinceLastTick / tickInterval, 0.0f, 1.0f);

        Player.Position = prevPos.Lerp(curPos, lerpFactor);

        ctx.Draw(Player.GetShape());

        if (DebugLine) // draw debug stuff
        {
            // Draw the velocity vector
            Vector2f velocityVec = new Vector2f(curPos.X - prevPos.X, curPos.Y - prevPos.Y);
            var velocityShape = new RectangleShape(new Vector2f(2, velocityVec.Y));

            velocityShape.FillColor = Color.Red;
            velocityShape.Position = curPos + new Vector2f(
                Player.Size.X / 2,
                Player.Size.Y
            );

            ctx.Draw(velocityShape);
        }
    }

    public LocalPlayer()
    {
        Player = new SolidObject();

        Player.Position = new Vector2f(100, -100);
        Player.Size = new Vector2f(50, 100);
        Player.Color = Color.White;

        prevPos = Player.Position;
        curPos = Player.Position;

        DebugLogger.Log("Components", $"Initialized : LocalPlayer");
    }

    public override void KeyPressed(KeyEventArgs e)
    {
        var Instance = ClientInstance.GetSingle();
    }
}