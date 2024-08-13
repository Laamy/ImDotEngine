#region Includes

using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;
using System.Security.Cryptography;

#endregion

internal class SolidObject : SolidActor
{
    public SolidObject()
    {
        shape = new RectangleShape();
    }

    // cache
    private RectangleShape shape;

    public RectangleShape Drawable { get => shape; }

    // base
    public Vector2f Size
    {
        get => shape.Size;
        set => shape.Size = value;
    }

    public Color? Color
    {
        get => shape.FillColor;
        set => shape.FillColor = (Color)value;
    }

    public Texture Texture
    {
        get => shape.Texture;
        set => shape.Texture = value;
    }

    public new Vector2f Position
    {
        get => shape.Position;
        set => shape.Position = value;
    }

    public override Vector2f GetPosition() => Position;
    public override Vector2f GetSize() => Size;

    public override void Draw(RenderWindow e) => e.Draw(shape);
}