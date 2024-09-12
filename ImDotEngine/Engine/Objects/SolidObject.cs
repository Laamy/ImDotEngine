#region Includes

using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;

#endregion

internal class SolidObject : SolidActor
{
    public SolidObject(Shader shader = null)
    {
        shape = new RectangleShape();
        Shader = shader;
    }

    // cache
    private RectangleShape shape;

    // NOTE: find a better way without using to much more memory to add tags (ecs or smth)
    public List<object> Tags { get; internal set; } = new List<object>();

    // shader for the object
    public Shader Shader { get; set; }

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
    public override int ObjectCount() => 1;

#if CLIENT
    public override Shape GetShape() => shape;
    public override Drawable GetDrawable() => null;
    public override void Draw(RenderWindow e)
    {
        if (Shader == null)
            e.Draw(shape);
        else
        {
            DebugLogger.Warn("drawing");
            var Instance = ClientInstance.GetSingle();

            Shader.SetUniform("u_res", (Vector2f)Instance.Engine.Size);
            Shader.SetUniform("u_pos", Position);
            Shader.SetUniform("u_time", Convert.ToSingle(Instance.Engine.TimeSinceStart.Elapsed.TotalSeconds));

            e.Draw(shape, new RenderStates(Shader));
        }
    }
#endif
}