using SFML.Graphics;

using System.Collections.Generic;
using System.IO;

internal class Materials
{
    const string DataPath = "Data\\Shaders";
    private Dictionary<string, Shader> m_cache = new Dictionary<string, Shader>();

    public void Initialize()
    {
        m_cache.Clear();

        foreach (var file in Directory.GetFiles(DataPath, "*.*", SearchOption.AllDirectories))
        {
            if (Path.GetExtension(file).Equals(".frag"))
            {
                // initialize the cache the texture
                var texture = new Shader(null, null, file);
                m_cache[file.ToLower()] = texture;

                DebugLogger.Log("Materials", $"Loaded : {file.ToLower()}");
            }
        }
    }

    public Shader GetShader(string name)
    {
        string path = Path.Combine(DataPath, name).ToLower();

        if (!File.Exists(path))
            return null; // doesnt exist

        if (m_cache.ContainsKey(path))
            return m_cache[path];

        return new Shader(null, null, Path.Combine(DataPath, name));
    }
}