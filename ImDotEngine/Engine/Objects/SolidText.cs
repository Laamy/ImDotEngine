using SFML.Graphics;
using SFML.System;

internal class SolidText : SolidActor
{
    public SolidText()
    {
        shape = new Text();
    }

    // cached shape
    Text shape;

    // shape properties
    public Color Color
    {
        get => shape.FillColor;
        set => shape.FillColor = value;
    }

    public string Text
    {
        get => shape.DisplayedString;
        set => shape.DisplayedString = value;
    }

    public Font Font
    {
        get => shape.Font;
        set => shape.Font = value;
    }

    public uint Size
    {
        get => shape.CharacterSize;
        set => shape.CharacterSize = value;
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

    public override void Draw(RenderWindow e) => e.Draw(shape);
}