using SFML.Graphics;
using SFML.System;

using System.Collections.Generic;

internal class SolidGroup : SolidActor
{
    private ConcurrentHashset<SolidActor> Children;
    private TextureAtlas textureAtlas;
    private RectangleShape shape;

    public new Vector2f Position
    {
        get => shape.Position;
        set => shape.Position = value;
    }

    public Vector2f Size
    {
        get => shape.Size;
        set => shape.Size = value;
    }

    public Vector2f Scale
    {
        get => shape.Scale;
        set => shape.Scale = value;
    }

    public override Vector2f GetPosition() => Position;
    public override Vector2f GetSize() => Size.Mul(Scale);
    public override int ObjectCount() => Children.Count;
    public ConcurrentHashset<SolidActor> GetObjects() => Children;

    public SolidGroup(TextureAtlas atlas)
    {
        Children = new ConcurrentHashset<SolidActor>();

        this.textureAtlas = atlas;

        shape = new RectangleShape();

#if CLIENT
        UpdateTexture();
#endif
    }

    public void AddObject(SolidActor obj)
    {
        Children.Add(obj); // i must be high or smth
    }
    public void RemoveObject(SolidActor obj)
    {
        Children.Remove(obj);
    }

    public void Invalidate()
    {
        textureAtlas.AddObjects(Children);
#if CLIENT
        UpdateTexture();
#endif
    }

    public void SetObjects(IEnumerable<SolidActor> actors)
    {
        textureAtlas.AddObjects(actors);

#if CLIENT
        UpdateTexture();
#endif
    }

#if CLIENT
    private void UpdateTexture()
    {
        shape.Texture = textureAtlas.Texture;
        shape.Size = (Vector2f)textureAtlas.Texture.Size;
    }

    public override Shape GetShape() => shape;
    public override Drawable GetDrawable() => null;

    public override void Draw(RenderWindow ctx)
    {
        //foreach (var child in Children)
        //{
        //    ctx.Draw(child.GetShape());
        //}

        {
            if (textureAtlas.Texture != null)
            {
                ctx.Draw(shape);
            }
        }
    }
#endif
}