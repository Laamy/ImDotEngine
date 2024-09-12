#region Includes

using SFML.Graphics;
using System;
using System.Collections.Generic;

#endregion

#if CLIENT
internal class FontRepository
{
    private static Dictionary<string, Font> _fonts = new Dictionary<string, Font>() {};

    public void Initialize()
    {
        ClientInstance Instance = ClientInstance.GetSingle();

        var Font = Instance.BundleRepository.GetBundle("Font");

        foreach (var font in Font.Contents)
        {
            var fontName = font.Key.Split('\\')[1].Split('.')[0];
            var fontBytes = font.Value;

            _fonts.Add(fontName.ToLower(), new Font(fontBytes));
        }

        //_fonts.Add("arial", new Font(Font["Font\\Arial.ttf"])); // left for a bit
    }

    public Font GetFont(string name) => _fonts[name.ToLower()];
}
#endif