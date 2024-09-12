using SFML.Graphics;
using SFML.System;

using System.Collections.Generic;
using System.Linq;

class StaticTile
{
    public SolidGroup Chunk;
    public SolidObject Block;
    public FloatRect Bounds;
}

class RigidBodyComponent : BaseComponent
{
    public ClientInstance Instance = ClientInstance.GetSingle();

    #region body properties

    public SolidObject BodyRoot;

    public Vector2f Velocity = new Vector2f(0, 15); // TODO: add a second velocity vector for speed for bodies that can move

    public Vector2f prevPos;
    public Vector2f curPos;

    const float Gravity = 2;
    const int MaxGravity = 240; // in units

    public float Zoom = 1;
    public float MaxZoom = 1;
    public float MinZoom = 0.1f;

    public bool OnGround = false;
    public bool InAir = false;

    public bool IsAnchored = false;

#if CLIENT
    public bool ActiveCamera { get; set; } = true;
#endif

    #endregion

#if CLIENT
    public void RefreshCamera()
    {
        var camera = Instance.Engine.Components.OfType<Camera2D>().FirstOrDefault();

        camera.AllowMove = false;
        camera.AllowZoom = true;

        camera.Zoom = Zoom;
        camera.MaxZoom = MaxZoom;
        camera.MinZoom = MinZoom;
    }
#endif

    public override void OnFixedUpdate()
    {
        var Instance = ClientInstance.GetSingle();

        prevPos = curPos;

        if (IsAnchored)
            return;

        Velocity.Y += Gravity;

        if (Mathf.Abs(Velocity.Y) > MaxGravity)
            Velocity.Y = MaxGravity; // not the best thing to do

        curPos += Velocity; // apply velocity
        
        // collision events too for control
        OnCollisionResolve();
        ResolveCollisions();
        AfterCollisionResolve();

        BodyRoot.Position = curPos;
    }

    public virtual void OnCollisionResolve() { }
    public virtual void AfterCollisionResolve() { }

    public virtual bool OnCollisionX(FloatRect Body, StaticTile Tile, Vector2f Overlap)
    {
        // TODO: add vert & hor collision flags seperately from onground & inair so i can verify this before clearing these flags
        ResetGroundFlags();

        if (Body.Left < Tile.Bounds.Left)
        {
            curPos.X -= Overlap.X;
        }
        else
        {
            curPos.X += Overlap.X;
        }

        return false;
    }

    public virtual bool OnCollisionY(FloatRect Body, StaticTile Tile, Vector2f Overlap)
    {
        if (Body.Top < Tile.Bounds.Top)
        {
            GroundBody(null, Tile.Bounds.Top - BodyRoot.Size.Y);
        }
        else
        {
            ResetGroundFlags();

            curPos.Y += Overlap.Y;
        }

        return false;
    }

    public IEnumerable<SolidActor> GetNearby()
    {
        var bodyCenter = BodyRoot.Position + (BodyRoot.Size / 2);
        var radius = Mathf.Max(BodyRoot.Size.X, BodyRoot.Size.Y) / 2;

        radius += 10; // keep it safe

        var nearbyChunks = Instance.Level.GetLayer(LevelLayers.ForeBlocks).GetNearbyObjects(bodyCenter, radius);

        return nearbyChunks;
    }

    // TODO: avoid tunneling by stepping through all the steps ig
    private void ResolveCollisions()
    {
        var Instance = ClientInstance.GetSingle();

        InAir = true;

        var nearbyChunks = GetNearby();

        var playerPos = curPos;
        var playerSize = BodyRoot.GetSize();
        var playerRect = new FloatRect(curPos, BodyRoot.GetSize());

        foreach (var _chunk in nearbyChunks)
        {
            if (_chunk != null)
            {
                // its just accured to me that i scale the group
                var chunk = _chunk as SolidGroup;

                var blocks = chunk.GetObjects();

                foreach (var block in blocks)
                {
                    // convert blockRect to worldspace
                    var blockRect = new FloatRect(chunk.GetPosition() + block.GetPosition().Mul(chunk.Scale), block.GetSize().Mul(chunk.Scale));

                    // NOTE: I would like to check if blocks are nearby me before making an intersects check due to how large it is math wise
                    // I saw a function abit ago in the floatrect class for this
                    // (BLOAT ALERT!)
                    FloatRect overlapRect;
                    if (playerRect.Intersects(blockRect, out overlapRect))
                    {
                        Vector2f overlap = new Vector2f(overlapRect.Width, overlapRect.Height);

                        bool endEarly = false;

                        // TODO: i want to handle blocks with angles differently.. for example the angled grass blocks should make the player go up if its the left or right (depending on the type)
                        // for now im going to put this idea into the localplayer but in the future I want it to be in all of them ^
                        if (overlap.X < overlap.Y)
                        {
                            endEarly = OnCollisionX(playerRect, new StaticTile()
                            {
                                Chunk = chunk,
                                Block = block as SolidObject, // NOTE: change this later with the better chunks
                                Bounds = blockRect,
                            }, overlap);
                        }
                        else
                        {
                            endEarly = OnCollisionY(playerRect, new StaticTile()
                            {
                                Chunk = chunk,
                                Block = block as SolidObject, // NOTE: change this later with the better chunks
                                Bounds = blockRect,
                            }, overlap);
                        }

                        if (endEarly)
                            return;
                    }
                }
            }
        }

        // world bottom
        // NOTE: call death functions once below this coordinate level (I might not do this cuz i want an infinite world height
        // NOTE: I might switch the world to a signed 32bit integer instead of floats, it'll make debugging easier & stop floating point errors
        if (Velocity.Y > 0 && playerPos.Y > 4750)
            GroundBody(null, 4750);

        if (InAir)
            OnGround = false;
    }

    // hacky helper function to ground the player properly
    public void GroundBody(float? x = null, float? y = null)
    {
        OnGround = true; // NOTE: probably better to move this up to where i set onground to false

        Velocity.Y = 0;
        curPos.X = x == null ? curPos.X : x.Value;
        curPos.Y = y == null ? curPos.Y : y.Value;

        InAir = false;
    }

    public void ResetGroundFlags()
    {
        OnGround = false;
        InAir = true;
    }

#if CLIENT
    public override void OnUpdate(RenderWindow ctx)
    {
        // keep the player smoothing interpolating between the
        // prevPos and curPos so the physics ticks aren't as visible
        {
            var Instance = ClientInstance.GetSingle();

            float timeSinceLastTick = (float)Instance.GuiData.StepTime.ElapsedMilliseconds / 1000;
            float tickInterval = 1.0f / Instance.Engine.TargetPhysicsRate;

            float lerpFactor = Mathf.Clamp(timeSinceLastTick / tickInterval, 0.0f, 1.0f);

            BodyRoot.Position = prevPos.Lerp(curPos, lerpFactor);
        }

        // TODO: allow solidgroups to be rigid bodies and not just solid objects..
        ctx.Draw(BodyRoot.GetShape());

        if (ActiveCamera) // TODO: move this to localplayer cuz i forgot to earlier..
        {
            var camera = Instance.Engine.Components.OfType<Camera2D>().FirstOrDefault();

            if (camera.AllowMove == true)
                RefreshCamera();

            var bounds = camera.CameraBounds;
            camera.Position = new Vector2f(BodyRoot.Position.X - (bounds.Width / 2) + (BodyRoot.Size.X / 2), BodyRoot.Position.Y - (bounds.Height / 2) + (BodyRoot.Size.Y / 2));
        }

        if (!IsAnchored && DebugConfig.ShowPhysicsDetails) // draw debug stuff
        {
            DebugPhysicsDetails.Draw(this, ctx);
        }
    }
#endif
}