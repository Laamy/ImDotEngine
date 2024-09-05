#region Includes

using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

#endregion

internal class TextureRepository
{   
    private Dictionary<string, Texture> m_cache = new Dictionary<string, Texture>();

    private BundleInfo m_assetBundle;

    public void Initialize()
    {
        DebugLogger.Log("Assets", "Caching assets..");

        m_cache.Clear();
        
        ClientInstance Instance = ClientInstance.GetSingle();

        m_assetBundle = Instance.BundleRepository.GetBundle("Assets");

        foreach (var file in m_assetBundle.Contents)
        {
            var fileName = file.Key;
            if (fileName.EndsWith(".png"))
            {
                // initialize the cache the texture
                var texture = new Texture(file.Value);
                m_cache[fileName] = texture;

                DebugLogger.Log("Assets", $"Loaded : {fileName}");
            }
        }

        DebugLogger.Log("Assets", $"Cached all assets");
    }

    public Texture GetTexture(string name)
    {
        if (m_cache.ContainsKey(name))
            return m_cache[name];

        throw new Exception("Invalid texture");
    }
}