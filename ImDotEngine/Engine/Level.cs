#region Includes

using SFML.Graphics;
using SFML.System;
using System;
using System.Linq;

#endregion

internal class Level
{
    private bool init = false;

    public SpatialHash[] Layers;

    public void Initialize()
    {
        Layers = new SpatialHash[(int)LevelLayers.Count];
        Layers.Initialize();

        for (int i = 0; i < Layers.Length; ++i)
            Layers[i] = new SpatialHash(50);

        init = true;
    }

    public SpatialHash GetLayer(LevelLayers layer)
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
            foreach (var child in layer.GetObjectsInBounds(bounds))
            {
                child.Draw(e);
            }
        }
    }

    public void ApplyShader(string shader, Action<Shader> Init)
    {
        var Instance = ClientInstance.GetSingle();
        var Components = Instance.Engine.Components;

        var Size = Instance.Engine.Size;

        var camera = Components.OfType<Camera2D>().FirstOrDefault();

        Vector2i topLeft = Instance.Engine.window.MapCoordsToPixel(new Vector2f(0, 0));

        var shaderFrag = Instance.Materials.GetShader(shader);
        Init(shaderFrag);

        shaderFrag.SetUniform("u_res", (Vector2f)Size);
        shaderFrag.SetUniform("u_pos", (Vector2f)topLeft);

        View temp = new View(new FloatRect(camera.Position, (Vector2f)camera.Size));
        temp.Zoom(camera.Zoom);

        {
            RectangleShape shaderScreen = new RectangleShape();

            shaderScreen.Position = temp.Center - new Vector2f(
                (Size.X / 2) * camera.Zoom,
                (Size.Y / 2) * camera.Zoom
            );
            shaderScreen.Size = (Vector2f)Size;
            shaderScreen.Scale = new Vector2f(camera.Zoom, camera.Zoom);

            shaderScreen.Draw(Instance.Engine.window, new RenderStates(shaderFrag));
        }
    }

    #region Temp region for layer stuff

    public SolidObject CreateRectangle(LevelLayers layer, Vector2f position, Vector2f size, Color colour)
    {
        // get pointers n references
        var Instance = ClientInstance.GetSingle();

        // temp
        SolidObject result;

        GetLayer(layer).AddObject(result = new SolidObject()
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

        GetLayer(layer).AddObject(result = new SolidCircle(radius)
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

        GetLayer(layer).AddObject(result = new SolidText()
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