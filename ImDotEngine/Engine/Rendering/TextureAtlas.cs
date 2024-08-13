using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;

internal class TextureAtlas
{
    private RenderTexture m_renderTexture;
    private Texture m_Texture;
    private Dictionary<SolidActor, Vector2f> objectPositions;

    public TextureAtlas(uint width, uint height)
    {
        m_renderTexture = new RenderTexture(width, height);
        m_renderTexture.Clear(Color.Transparent);

        m_Texture = m_renderTexture.Texture;

        objectPositions = new Dictionary<SolidActor, Vector2f>();
    }

    private void Invalidate()
    {
        m_renderTexture.Clear(Color.Transparent);
    }

    public Texture Texture => m_Texture;

    public void AddObjects(IEnumerable<SolidActor> objects)
    {
        m_renderTexture.Clear(Color.Transparent);

        foreach (var obj in objects)
        {
            var position = obj.Position;
            var size = obj.GetSize();
            var drawable = obj as SolidObject;

            if (drawable != null)
            {
                m_renderTexture.Draw(drawable.Drawable);
                objectPositions[drawable] = position;
            }
        }
    }
}