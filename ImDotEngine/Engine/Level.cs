#region Includes

using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#endregion

internal class Level
{
    private bool init = false;

    // TODO: make layers
    public List<SolidActor>[] Layers;

    public void Initialize()
    {
        Layers = new List<SolidActor>[(int)LevelLayers.Count];
        Layers.Initialize();

        for (int i = 0; i < Layers.Length; ++i)
            Layers[i] = new List<SolidActor>();

        init = true;
    }

    public List<SolidActor> GetLayer(LevelLayers layer)
    {
        if (!init) Initialize();

        return Layers[(int)layer];
    }

    public void Draw(RenderWindow e)
    {
        Camera2D camera = ClientInstance.GetSingle().Engine.Components.OfType<Camera2D>().FirstOrDefault();

        FloatRect bounds = camera.CameraBounds;

        foreach (var layer in Layers)
        {
            foreach (var child in layer)
            {
                var position = child.GetPosition();
                var size = child.GetSize();

                if (position.X + size.X > bounds.Left &&
                    position.X < bounds.Left + bounds.Width &&
                    position.Y + size.Y > bounds.Top &&
                    position.Y < bounds.Top + bounds.Height
                )
                {
                    child.Draw(e);
                }
                //child.Draw(e);
            }
        }
    }

    #region Temp region for layer stuff

    public SolidObject CreateRectangle(LevelLayers layer, Vector2f position, Vector2f size, Color colour)
    {
        // get pointers n references
        var Instance = ClientInstance.GetSingle();

        // temp
        SolidObject result;

        GetLayer(layer).Add(result = new SolidObject()
        {
            Position = position,
            Size = size,
            Color = colour,
        });

        return result;
    }

    public SolidCircle CreateCircle(LevelLayers layer, Vector2f position, float radius, Color colour)
    {
        // get pointers n references
        var Instance = ClientInstance.GetSingle();

        // temp
        SolidCircle result;

        GetLayer(layer).Add(result = new SolidCircle(radius)
        {
            Position = position,
            Color = colour
        });

        return result;
    }

    public SolidText CreateText(LevelLayers layer, Vector2f position, Color color, float size = 16, string text = "")
    {
        // get pointers n references
        var Instance = ClientInstance.GetSingle();

        // temp
        SolidText result;

        GetLayer(layer).Add(result = new SolidText()
        {
            Position = position,
            Size = (uint)size,
            Color = color,
            Font = Instance.FontRepository.GetFont("Arial"),
            Text = text
        });

        return result;
    }

    #endregion
}