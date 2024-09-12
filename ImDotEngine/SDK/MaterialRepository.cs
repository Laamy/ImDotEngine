using SFML.Graphics;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

#if CLIENT
internal class MaterialRepository
{
    private Dictionary<string, Shader> m_cache = new Dictionary<string, Shader>();

    private BundleInfo m_assetBundle;

    public void Initialize()
    {
        DebugLogger.Log("Materials", "Compiling shaders..");

        m_cache.Clear();

        ClientInstance Instance = ClientInstance.GetSingle();

        m_assetBundle = Instance.BundleRepository.GetBundle("Shaders");

        foreach (var file in m_assetBundle.Contents)
        {
            var fileName = file.Key;
            if (fileName.EndsWith(".frag"))
            {
                //initialize the cache the texture
                var texture = new Shader(null, null, new MemoryStream(file.Value));
                m_cache[fileName] = texture;

                DebugLogger.Log("Materials", $"Loaded : {fileName}");
            }
        }

        DebugLogger.Log("Materials", $"Compiled all shaders!");
    }

    public Shader GetShader(string name)
    {
        if (m_cache.ContainsKey(name))
            return m_cache[name];

        throw new Exception("Invalid shader");
    }
}
#endif