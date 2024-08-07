using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

// funky debug camera I threw together
internal class Camera2D : BaseComponent
{
    public ClientInstance Instance = ClientInstance.GetSingle();

    private View view = new View(new FloatRect(0, 0, 0, 0)); // main view
    
    // info for view
    private Vector2f position = new Vector2f(0, 0);
    private Vector2f size = new Vector2f(700, 700);
    private float zoom = 1;

    // for camera zooming
    private bool moving = false;
    private Vector2f initMousePos;

    // basic settings for the camera
    public bool AutoResize = true;

    /// <summary>
    /// Camera Size
    /// </summary>
    public Vector2u Size
    {
        get => new Vector2u((uint)size.X, (uint)size.Y);
        set
        {
            size = new Vector2f(value.X, value.Y);
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

            return new FloatRect(topLeft, bottomRight);
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
        if (moving)
        {
            Vector2f curMousePos = new Vector2f(e.X, e.Y);
            Vector2f offset = curMousePos - initMousePos;

            Position -= offset * Zoom;

            initMousePos = curMousePos;
        }
    }

    public override void MouseButtonPressed(MouseButtonEventArgs e)
    {
        if (e.Button == Mouse.Button.Right)
        {
            moving = true;
            initMousePos = new Vector2f(e.X, e.Y);
        }
    }

    public override void MouseButtonReleased(MouseButtonEventArgs e)
    {
        if (e.Button == Mouse.Button.Right)
        {
            moving = false;
        }
    }

    public override void MouseWheelScrolled(MouseWheelScrollEventArgs e)
    {
        float zoomAmount = 0.1f;

        if (e.Delta > 0)
            Zoom /= (1 + zoomAmount);
        else if (e.Delta < 0)
            Zoom *= (1 + zoomAmount);
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