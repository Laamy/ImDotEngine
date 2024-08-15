#region Includes

using SFML.Graphics;
using SFML.System;
using System;

#endregion

internal class SolidConvex : SolidActor
{
    public SolidConvex()
    {
        shape = new ConvexShape();
    }

    // cached object
    ConvexShape shape;

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

    public float Rotation
    {
        get => shape.Rotation;
        set => shape.Rotation = value;
    }

    public new Vector2f Position
    {
        get => shape.Position;
        set => shape.Position = value;
    }

    /// <summary>
    /// Shape has to be aligned 0-10
    /// </summary>
    public void LoadShape(Vector2f[] vertices, float size = 10)
    {
        shape.SetPointCount((uint)vertices.Length + 1);

        for (uint i = 0; i < vertices.Length; ++i)
            shape.SetPoint(i, vertices[i] * (size / 10));
    }

    public override void Draw(RenderWindow e) => e.Draw(shape);

    public override Vector2f GetPosition() => Position;
    public override Vector2f GetSize() => new Vector2f(10, 10); // TODO: check largest/farthest polygon and use as size for both X & Y
    public override Shape GetShape() => shape;
    public override Drawable GetDrawable() => null;
}