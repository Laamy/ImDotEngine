﻿#region Includes

using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

#endregion

internal class TextureRepository
{
    const string DataPath = "Data\\Assets";
    private Dictionary<string, Texture> m_cache = new Dictionary<string, Texture>();

    public void Initialize()
    {
        m_cache.Clear();

        foreach (var file in Directory.GetFiles(DataPath, "*.*", SearchOption.AllDirectories))
        {
            if (Path.GetExtension(file).Equals(".png"))
            {
                // initialize the cache the texture
                var texture = new Texture(file);
                m_cache[file.ToLower()] = texture;

                DebugLogger.Log("Assets", $"Loaded : {file.ToLower()}");
            }
        }
    }

    public Texture GetTexture(string name)
    {
        string path = Path.Combine(DataPath, name).ToLower();

        if (!File.Exists(path))
            return null; // doesnt exist

        if (m_cache.ContainsKey(path))
            return m_cache[path];

        return new Texture(Path.Combine(DataPath, name));
    }
}