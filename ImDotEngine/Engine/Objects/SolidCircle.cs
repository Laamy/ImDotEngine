#region Includes

using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;

#endregion

internal class SolidCircle : SolidActor
{
    public SolidCircle(float radius, uint points = 20)
    {
        shape = new CircleShape(radius, points);
    }

    // cached object
    CircleShape shape;

    // base
    public float Radius
    {
        get => shape.Radius;
        set => shape.Radius = value;
    }

    public Color Color
    {
        get => shape.FillColor;
        set => shape.FillColor = value;
    }

    public new Vector2f Position
    {
        get => shape.Position;
        set => shape.Position = value;
    }

    public override void Draw(RenderWindow e) => e.Draw(shape);

    public override Vector2f GetPosition() => Position;
    public override Vector2f GetSize() => new Vector2f((Radius * 2) + 1, (Radius * 2) + 1);
    public override Shape GetShape() => shape;
    public override Drawable GetDrawable() => null;
}