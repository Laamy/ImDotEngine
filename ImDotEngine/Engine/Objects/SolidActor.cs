#region Includes

using SFML.Graphics;
using SFML.System;

#endregion

internal class SolidActor
{
    // base
    public Vector2f Position;
    public Vector2f WorldSize = new Vector2f(0, 0); // for points

    public virtual void Draw(RenderWindow e) { }
}