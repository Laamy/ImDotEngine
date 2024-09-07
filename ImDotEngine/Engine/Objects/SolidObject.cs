#region Includes

using SFML.Graphics;
using SFML.System;
using System.Collections.Concurrent;
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

    // NOTE: find a better way without using to much more memory to add tags (ecs or smth)
    public List<object> Tags { get; internal set; } = new List<object>();

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
    public float Rotation
    {
        get => shape.Rotation;
        set => shape.Rotation = value;
    }


    public override Vector2f GetPosition() => Position;
    public override Vector2f GetSize() => Size;
    public override Shape GetShape() => shape;
    public override Drawable GetDrawable() => null;
    public override int ObjectCount() => 1;

    public override void Draw(RenderWindow e) => e.Draw(shape);
}