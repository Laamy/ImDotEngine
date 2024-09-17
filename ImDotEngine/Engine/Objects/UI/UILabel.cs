using SFML.Graphics;
using SFML.System;

class UILabel : UIActor
{
    private Text textShape;
    
    public UILabel(string content, Font font)
    {
        textShape = new Text(content, font);
        Position = new Vector2f(0, 0);
    }

    // quick properties
    public string Content
    {
        get => textShape.DisplayedString;
        set => textShape.DisplayedString = value;
    }

    public Font Font
    {
        get => textShape.Font;
        set => textShape.Font = value;
    }

    public uint CharacterSize
    {
        get => textShape.CharacterSize;
        set => textShape.CharacterSize = value;
    }

    // overrides
    public override Vector2f GetPosition() => Position;
    public override Vector2f GetSize() => new Vector2f(textShape.GetGlobalBounds().Width, textShape.GetGlobalBounds().Height);

    public override void Draw(RenderWindow e)
    {
        textShape.Position = GetPosition();
        e.Draw(textShape);
    }
}