using SFML.Graphics;
using SFML.System;

using System.Collections.Generic;

internal class TextureAtlas
{
#if CLIENT
    private RenderTexture m_renderTexture;
    private Texture m_Texture;
#endif
    private Dictionary<SolidActor, Vector2f> objectPositions;

    public TextureAtlas(uint width, uint height)
    {
#if CLIENT
        m_renderTexture = new RenderTexture(width, height);
        m_renderTexture.Clear(Color.Transparent);

        m_Texture = m_renderTexture.Texture;
#endif

        objectPositions = new Dictionary<SolidActor, Vector2f>();
    }

#if CLIENT
    public Texture Texture => m_Texture;
#endif

    public void AddObjects(IEnumerable<SolidActor> objects)
    {
#if CLIENT
        m_renderTexture.Clear(Color.Transparent);
#endif

        foreach (var obj in objects)
        {
            var position = obj.GetPosition();
            var size = obj.GetSize();

            if (obj != null)
            {
#if CLIENT
                if (obj.GetShape() != null)
                {
                    m_renderTexture.Draw(obj.GetShape());
                }
                else
                {
                    m_renderTexture.Draw(obj.GetDrawable());
                }
#endif

                objectPositions[obj] = position;
            }
        }

#if CLIENT
        m_renderTexture.Display();
#endif
    }
}