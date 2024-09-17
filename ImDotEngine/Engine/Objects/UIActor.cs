using SFML.Graphics;
using SFML.System;

abstract class UIActor
{
    // actor variables
    public Vector2f Position { get; set; }

    // required overrides
    public abstract Vector2f GetPosition();
    public abstract Vector2f GetSize();

    // events
    public virtual void Draw(RenderWindow e) { }
}