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

    public Texture Texture => m_Texture;

    public void AddObjects(IEnumerable<SolidActor> objects)
    {
        m_renderTexture.Clear(Color.Transparent);

        foreach (var obj in objects)
        {
            var position = obj.GetPosition();
            var size = obj.GetSize();

            if (obj != null)
            {
                if (obj.GetShape() != null)
                {
                    m_renderTexture.Draw(obj.GetShape());
                }
                else
                {
                    m_renderTexture.Draw(obj.GetDrawable());
                }

                objectPositions[obj] = position;
            }
        }

        m_renderTexture.Display();
    }
}