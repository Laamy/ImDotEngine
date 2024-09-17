#region Includes

using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

#endregion

internal class Level
{
    private bool init = false;

    public SpatialHash[] Layers;
    public List<UIActor>[] UILayers;

    public void Initialize()
    {
        // actual scene stuff
        {
            Layers = new SpatialHash[(int)LevelLayers.Count];
            Layers.Initialize();

            for (int i = 0; i < Layers.Length; ++i)
                Layers[i] = new SpatialHash(50);
        }

        // ui scene
        {
            UILayers = new List<UIActor>[Layers.Length];
            UILayers.Initialize();

            for (int i = 0; i < Layers.Length; ++i)
                UILayers[i] = new List<UIActor>();
        }

        init = true;
    }

    public int HashPosition(Vector2f position)
    {
        int x = (int)Math.Floor(position.X / 50); // TODO: stop hardcoding shit
        int y = (int)Math.Floor(position.Y / 50);
        return x + y * 73856093;
    }

    public SpatialHash GetLayer(LevelLayers layer)
    {
        if (!init) Initialize();

        return Layers[(int)layer];
    }

#if CLIENT
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

        var shaderFrag = Instance.MaterialRepository.GetShader($"Shaders\\{shader}");
        Init(shaderFrag);

        shaderFrag.SetUniform("u_res", (Vector2f)Size);
        shaderFrag.SetUniform("u_pos", (Vector2f)topLeft);
        shaderFrag.SetUniform("u_time", Convert.ToSingle(Instance.Engine.TimeSinceStart.Elapsed.TotalSeconds));

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
#endif
}