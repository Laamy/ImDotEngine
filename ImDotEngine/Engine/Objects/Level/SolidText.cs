using SFML.Graphics;
using SFML.System;

internal class SolidText : SolidActor
{
    public SolidText()
    {
        drawable = new Text();
    }

    // cached shape
    Text drawable;

    // shape properties
    public Color Color
    {
        get => drawable.FillColor;
        set => drawable.FillColor = value;
    }

    public string Text
    {
        get => drawable.DisplayedString;
        set => drawable.DisplayedString = value;
    }

    public Font Font
    {
        get => drawable.Font;
        set => drawable.Font = value;
    }

    public uint Size
    {
        get => drawable.CharacterSize;
        set => drawable.CharacterSize = value;
    }

    public float Rotation
    {
        get => drawable.Rotation;
        set => drawable.Rotation = value;
    }

    public new Vector2f Position
    {
        get => drawable.Position;
        set => drawable.Position = value;
    }

    public override Vector2f GetPosition() => Position;
    public override Vector2f GetSize() => new Vector2f(200, 200); // TODO: actual size for this
    public override int ObjectCount() => 1;

#if CLIENT
    public override void Draw(RenderWindow e) => e.Draw(drawable);
    public override Shape GetShape() => null;
    public override Drawable GetDrawable() => drawable;
#endif
}