#region Includes

using SFML.Graphics;
using SFML.System;

#endregion

abstract class SolidActor
{
    // base
    public Vector2f Position;

    // dimensions
    public abstract Vector2f GetPosition();
    public abstract Vector2f GetSize();

    // spatial hash counter
    public abstract int ObjectCount();

#if CLIENT
    // drawables & shapes
    public abstract Shape GetShape();
    public abstract Drawable GetDrawable();

    public virtual void Draw(RenderWindow e) { }
#endif
}