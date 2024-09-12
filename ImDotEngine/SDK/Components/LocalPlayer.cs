using SFML.Graphics;
using SFML.System;
using SFML.Window;

// NOTE: move all the properties into components and stack similar ones next to each other in memory (ECS/ENTT)
// it means I could make physics steps faster by a fuckton
// I also want to move the booleans into "FLAGS" aka just putting an empty component on an entity
// I want to do my own implementation of ENTT in a unique way that would make cheating harder but make modding easier if that makes sense(?)
// NOTE: port everything to C++
#if CLIENT
class LocalPlayer : RigidBodyComponent
{
    #region player properties

    public Vector2f MovementVector = new Vector2f(0, 0); // each float between -1 and 1, TODO: check on each physics step if the player is moving left or right

    public float Speed = 20; // cheats
    public float JumpHeight = 16;

    #endregion

    public override void OnCollisionResolve()
    {
        // apply our movement vector to the curPos
        curPos += MovementVector * Speed;
    }

    public override bool OnCollisionY(FloatRect Body, StaticTile Tile, Vector2f Overlap)
    {
        base.OnCollisionY(Body, Tile, Overlap);

        if (Body.Top < Tile.Bounds.Top)
        {
            var blockId = (BlockEnum)Tile.Block.Tags[0];

            if (blockId == BlockEnum.Grass || blockId == BlockEnum.Grass_Left || blockId == BlockEnum.Grass_Right)
            {
                // TODO: fix bug when ur between 3 blocks (standing on them)
                GroundBody(null, Tile.Bounds.Top - BodyRoot.Size.Y + 3); // force few pixels
            }
        }

        return false;
    }

    public override bool OnCollisionX(FloatRect Body, StaticTile Tile, Vector2f Overlap)
    {
        // allow body to handle its original collisions
        base.OnCollisionX(Body, Tile, Overlap);

        if (Body.Left < Tile.Bounds.Left)
        {
            if ((BlockEnum)Tile.Block.Tags[0] == BlockEnum.Grass_Left)
            {
                curPos.X += Overlap.X; // undo
                GroundBody(null, Tile.Bounds.Top - BodyRoot.Size.Y); // jump to top of block

                return true; // end collisions early TODO: dont do this
            }
        }
        else
        {
            if ((BlockEnum)Tile.Block.Tags[0] == BlockEnum.Grass_Right)
            {
                curPos.X -= Overlap.X; // undo
                GroundBody(null, Tile.Bounds.Top - BodyRoot.Size.Y); // jump to top of block

                return true;
            }
        }

        return false;
    }

    public override void OnFixedUpdate()
    {
        // gonna do this pre-fixed
        if (Instance.Engine.HasFocus)
        {
            MovementVector = new Vector2f(); // clear

            if (Keyboard.IsKeyPressed(Keyboard.Key.A) ||
                Keyboard.IsKeyPressed(Keyboard.Key.Left))
                MovementVector.X--;

            if (Keyboard.IsKeyPressed(Keyboard.Key.D) ||
                Keyboard.IsKeyPressed(Keyboard.Key.Right))
                MovementVector.X++;

            if (Keyboard.IsKeyPressed(Keyboard.Key.W) ||
                Keyboard.IsKeyPressed(Keyboard.Key.Space) ||
                Keyboard.IsKeyPressed(Keyboard.Key.Up))
            {
                if (OnGround) // make sure the player is grounded before applying velocity
                    Velocity.Y = -JumpHeight;
            }
        }

        // handle collisions
        base.OnFixedUpdate();
    }

    public override void KeyPressed(KeyEventArgs e)
    {
        if (e.Code == Keyboard.Key.LShift)
            Speed = 31;
    }

    public override void KeyReleased(KeyEventArgs e)
    {
        if (e.Code == Keyboard.Key.LShift)
            Speed = 20;
    }

    public LocalPlayer()
    {
        BodyRoot = new SolidObject();

        BodyRoot.Position = new Vector2f(0, 0);
        BodyRoot.Size = new Vector2f(38, 65);
        //BodyRoot.Color = Color.White;

        // zoom limit
        MaxZoom = 0.5f;
        Zoom = 0.3f;
        MinZoom = 0.2f;

        var playerAsset = Instance.TextureRepository.GetTexture("Assets\\Texture\\player\\female.png");

        BodyRoot.Texture = playerAsset;

        prevPos = BodyRoot.Position;
        curPos = BodyRoot.Position;

        DebugLogger.Log("Components", $"Initialized : LocalPlayer");
    }
}
#endif