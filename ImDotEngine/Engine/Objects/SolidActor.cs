#region Includes

using SFML.Graphics;
using SFML.System;

#endregion

abstract class SolidActor
{
    // base
    public Vector2f Position;

    public abstract Vector2f GetPosition();
    public abstract Vector2f GetSize();
    public abstract Shape GetShape();
    public abstract Drawable GetDrawable();

    public virtual void Draw(RenderWindow e) { }
}