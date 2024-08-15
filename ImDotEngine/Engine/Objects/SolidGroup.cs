﻿using SFML.Graphics;
using SFML.System;

using System;
using System.Collections.Generic;
using static SFML.Graphics.BlendMode;

internal class SolidGroup : SolidActor
{
    private HashSet<SolidActor> Children;
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
    public override Vector2f GetSize() => Size;
    public override Shape GetShape() => shape;
    public override Drawable GetDrawable() => null;

    public SolidGroup(TextureAtlas atlas)
    {
        Children = new HashSet<SolidActor>();

        this.textureAtlas = atlas;

        shape = new RectangleShape();

        UpdateTexture();
    }

    public void AddObject(SolidActor obj)
    {
        Children.Add(obj); // i must be high or smth
    }

    public void Invalidate()
    {
        textureAtlas.AddObjects(Children);
        UpdateTexture();
    }

    public void SetObjects(IEnumerable<SolidActor> actors)
    {
        textureAtlas.AddObjects(actors);

        UpdateTexture();
    }

    private void UpdateTexture()
    {
        shape.Texture = textureAtlas.Texture;
        shape.Size = (Vector2f)textureAtlas.Texture.Size;
    }

    public override void Draw(RenderWindow ctx)
    {
        if (textureAtlas.Texture != null)
        {
            ctx.Draw(shape);
        }
    }
}