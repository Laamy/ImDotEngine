#region Includes

using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;

#endregion

internal class SolidCircle : SolidActor
{
    public SolidCircle(float radius)
    {
        shape = new CircleShape(radius);
    }

    // cached object
    CircleShape shape;

    // base
    public float Radius
    {
        get => shape.Radius;
        set
        {
            var C2R = (shape.Radius * 2) + 1;
            WorldSize = new Vector2f(C2R, C2R);

            shape.Radius = value;
        }
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