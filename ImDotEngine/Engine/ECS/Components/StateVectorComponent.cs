using SFML.System;

class StateVectorComponent : IEntityComponent
{
    public Vector2f Velocity = new Vector2f(0, 15); // TODO: add a second velocity vector for speed for bodies that can move

    public Vector2f PrevPosition;
    public Vector2f CurPosition;
}