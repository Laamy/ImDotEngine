using SFML.Graphics;
using SFML.System;
using SFML.Window;

using System;
using System.Linq;

// TODO: move all the properties into components and stack similar ones next to each other in memory (ECS/ENTT)
// it means I could make physics steps faster by a fuckton
// I also want to move the booleans into "FLAGS" aka just putting an empty component on an entity
// I want to do my own implementation of ENTT in a unique way that would make cheating harder but make modding easier if that makes sense(?)
// TODO: port everything to C++
class LocalPlayer : BaseComponent
{
    public ClientInstance Instance = ClientInstance.GetSingle();

    #region player properties

    public SolidObject Player;

    public Vector2f Velocity = new Vector2f(0, 15);
    public Vector2f MovementVector = new Vector2f(0, 0); // each float between -1 and 1, TODO: check on each physics step if the player is moving left or right

    public Vector2f prevPos;
    public Vector2f curPos;

    const float Gravity = 1;
    const int MaxGravity = 240; // in units

    public bool OnGround = false;
    public bool PrevOnGround = false;
    public bool InAir = false;

    public float Speed = 20;

    #endregion

    public override void OnFixedUpdate()
    {
        var Instance = ClientInstance.GetSingle();

        // gonna do this pre-fixed
        {
            MovementVector = new Vector2f(); // clear

            if (Keyboard.IsKeyPressed(Keyboard.Key.A) || Keyboard.IsKeyPressed(Keyboard.Key.Left))
                MovementVector.X--;

            if (Keyboard.IsKeyPressed(Keyboard.Key.D) || Keyboard.IsKeyPressed(Keyboard.Key.Right))
                MovementVector.X++;
        }

        prevPos = curPos;

        Velocity.Y += Gravity;

        if (Mathf.Abs(Velocity.Y) > MaxGravity)
            Velocity.Y = MaxGravity; // not the best thing to do

        curPos += Velocity; // apply velocity

        // temp code cuz im genuinely not sure how you handle movement properly
        // normally I define multiple velocities but I think I'll just try this for now
        curPos += MovementVector * Speed;

        ResolveCollisions();

        Player.Position = curPos;
    }

    private void ResolveCollisions()
    {
        var Instance = ClientInstance.GetSingle();

        Console.WriteLine("Getting nearby tiles");
        var nearbyChunks = Instance.Level.GetLayer(LevelLayers.ForeBlocks).GetNearbyObjects(Player.Position, 75);

        var playerPos = curPos;
        var playerSize = Player.GetSize();
        var playerRect = new FloatRect(curPos, Player.GetSize());

        foreach (var _chunk in nearbyChunks)
        {
            if (_chunk != null)
            {
                var chunk = _chunk as SolidGroup;

                var blocks = chunk.GetObjects();

                foreach (var block in blocks)
                {
                    // convert blockRect to worldspace
                    var blockRect = new FloatRect(chunk.GetPosition() + block.GetPosition(), block.GetSize());

                    if (playerRect.Intersects(blockRect))
                    {
                        if (Velocity.Y > 0 && playerPos.Y <= blockRect.Top)
                        {
                            Velocity.Y = 0;
                            curPos.Y = blockRect.Top;
                        }
                    }
                }
            }
        }

        // world bottom
        // TODO: call death functions once below this coordinate level (I might not do this cuz i want an infinite world height
        // NOTE: I might switch the world to a signed 32bit integer instead of floats, it'll make debugging easier & stop floating point errors
        if (Velocity.Y > 0 && playerPos.Y > 4750)
        {
            Velocity.Y = 0;
            curPos.Y = 4750;
            //Player.Position = curPos;
        }
    }

    public override void OnUpdate(RenderWindow ctx)
    {
        // keep the player smoothing interpolating between the
        // prevPos and curPos so the physics ticks aren't as visible
        {
            var Instance = ClientInstance.GetSingle();

            float timeSinceLastTick = (float)Instance.GuiData.StepTime.ElapsedMilliseconds / 1000;
            float tickInterval = 1.0f / Instance.Engine.TargetPhysicsRate;

            float lerpFactor = Mathf.Clamp(timeSinceLastTick / tickInterval, 0.0f, 1.0f);

            Player.Position = prevPos.Lerp(curPos, lerpFactor);
        }

        ctx.Draw(Player.GetShape());

        {
            var camera = Instance.Engine.Components.OfType<Camera2D>().FirstOrDefault();

            //camera.AllowMove = false;
            camera.AllowZoom = false;

            camera.Zoom = 1;

            var bounds = camera.CameraBounds;
            camera.Position = new Vector2f(Player.Position.X - (bounds.Width / 2) + (Player.Size.X / 2), Player.Position.Y - (bounds.Height / 2) + (Player.Size.Y / 2));
        }

        if (DebugConfig.ShowPhysicsDetails) // draw debug stuff
        {
            DebugPhysicsDetails.Draw(this, ctx);
        }
    }

    public LocalPlayer()
    {
        Player = new SolidObject();

        Player.Position = new Vector2f(100, -100);
        Player.Size = new Vector2f(50, 100);
        Player.Color = Color.White;

        var Instance = ClientInstance.GetSingle();

        //Instance.Level.GetLayer(LevelLayers.Entities).AddObject(Player);

        prevPos = Player.Position;
        curPos = Player.Position;

        DebugLogger.Log("Components", $"Initialized : LocalPlayer");
    }
}