using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Data;

// funky debug camera I threw together
internal class Camera2D : BaseComponent
{
    public ClientInstance Instance = ClientInstance.GetSingle();

    private View view = new View(new FloatRect(0, 0, 0, 0)); // main view
    
    // info for view
    private Vector2f position = new Vector2f(0, 0);
    private Vector2f size = new Vector2f(800, 700);
    private float zoom = 1;
    private float maxZoom = 5;
    private float minZoom = 0;

    // for camera zooming
    private bool moving = false;
    private Vector2f initMousePos;

    // basic settings for the camera
    public bool AutoResize = true;
    public bool AllowZoom = true;
    public bool AllowMove = true;

    public Camera2D()
    {
        size = (Vector2f)Instance.Engine.Size;

        DebugLogger.Log("Components", $"Initialized : Camera2D");
    }

    public Vector2f Origin
    {
        get
        {
            View temp = new View(new FloatRect(position, size));
            temp.Zoom(zoom);

            var center = view.Center;

            return center;
        }
    }

    /// <summary>
    /// Camera Size
    /// </summary>
    public Vector2u Size
    {
        get => new Vector2u((uint)size.X, (uint)size.Y);
        set
        {
            View temp = new View(new FloatRect(position, size));
            temp.Zoom(zoom);

            var center = view.Center;

            size = new Vector2f(value.X, value.Y);

            temp.Reset(new FloatRect(position, size));
            temp.Zoom(zoom);
            temp.Center = center;

            position = new Vector2f(view.Center.X - size.X / 2, view.Center.Y - size.Y / 2);
        }
    }

    /// <summary>
    /// Camera Position
    /// </summary>
    public Vector2f Position
    {
        get => position;
        set
        {
            position = value;
        }
    }

    /// <summary>
    /// Camera Zoom
    /// </summary>
    public float Zoom
    {
        get => zoom;
        set
        {
            zoom = value;
        }
    }

    /// <summary>
    /// Max Camera Zoom
    /// </summary>
    public float MaxZoom
    {
        get => maxZoom;
        set
        {
            maxZoom = value;
        }
    }

    /// <summary>
    /// Max Camera Zoom
    /// </summary>
    public float MinZoom
    {
        get => minZoom;
        set
        {
            minZoom = value;
        }
    }

    public void Set(RenderWindow window)
    {
        view.Reset(new FloatRect(position, size));
        view.Zoom(zoom);

        window.SetView(view);
    }

    public FloatRect CameraBounds
    {
        get
        {
            Vector2f topLeft = Instance.Engine.window.MapPixelToCoords(new Vector2i(0, 0));
            Vector2f bottomRight = Instance.Engine.window.MapPixelToCoords(new Vector2i((int)size.X, (int)size.Y));

            Vector2f sizeInWorld = bottomRight - topLeft;

            sizeInWorld /= zoom;

            return new FloatRect(topLeft, sizeInWorld);
        }
    }

    public Vector2f CursorToWorld(RenderWindow window, Vector2f mousePixelPos)
    {
        Vector2f worldPos = window.MapPixelToCoords(new Vector2i((int)mousePixelPos.X, (int)mousePixelPos.Y), view);

        return worldPos;
    }

    #region Movement & Zooming

    public override void MouseMoved(MouseMoveEventArgs e)
    {
        if (AllowMove && moving)
        {
            Vector2f curMousePos = new Vector2f(e.X, e.Y);
            Vector2f offset = curMousePos - initMousePos;

            Position -= offset * Zoom;

            initMousePos = curMousePos;
        }
    }

    public override void MouseButtonPressed(MouseButtonEventArgs e)
    {
        if (AllowMove && e.Button == Mouse.Button.Right)
        {
            moving = true;
            initMousePos = new Vector2f(e.X, e.Y);
        }
    }

    public override void MouseButtonReleased(MouseButtonEventArgs e)
    {
        if (AllowMove && e.Button == Mouse.Button.Right)
        {
            moving = false;
        }
    }

    public override void MouseWheelScrolled(MouseWheelScrollEventArgs e)
    {
        if (AllowZoom)
        {
            float zoomAmount = 0.1f;

            if (e.Delta > 0)
                Zoom /= (1 + zoomAmount);
            else if (e.Delta < 0)
                Zoom *= (1 + zoomAmount);
            
            zoom = Mathf.Max(zoom, minZoom);
            zoom = Mathf.Min(zoom, maxZoom);
        }
    }

    #endregion

    public override void Resize(SizeEventArgs e)
    {
        if (AutoResize)
            Size = new Vector2u(e.Width, e.Height);
    }

    public override void OnUpdate(RenderWindow ctx)
    {
        Set(ctx); // lol
    }
}