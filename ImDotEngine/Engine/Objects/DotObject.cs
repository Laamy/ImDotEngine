#region Includes

using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;

#endregion

internal class DotObject : Object
{
    public DotObject(float radius)
    {
        shape = new CircleShape(radius);
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
}