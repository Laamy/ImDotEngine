#region Includes

using SFML.Graphics;
using SFML.System;
using SFML.Window;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

#endregion

#if CLIENT
internal class GameEngine
{
    #region Internal Crap

    private int targetPPS = 30;

    public int CurrentFPS = 0;
    private int frameCount = 0;
    private long fpsLastTime = DateTime.Now.Ticks;

    public int CurrentPPS = 0;
    private int physicStepCount = 0;
    private long lastPhysicsStep = DateTime.Now.Ticks;

    private bool vsync = false;
    private uint framerate = 0;

    private Vector2u _Size;

    #endregion

    // sdl/sfml stuff
    public RenderWindow window;

    // Components that inherit game overrides
    public List<BaseService> Services = new List<BaseService>();

    // all the game things insida here !
    public ClientInstance Instance = ClientInstance.GetSingle();

    // some animation stuff
    public Stopwatch TimeSinceStart = Stopwatch.StartNew();

    public void Start()
    {
        // VERY FIRST THING YOU NEED TO DO IS LOAD THE BUNDLES!
        Instance.BundleRepository.Initialize();

        // initialize starter assets
        Assets.Initialize();

        Instance.FontRepository.Initialize();

        // sdl renderer
        Instance.VideoMode = new VideoMode(800, 600);
        window = new RenderWindow(Instance.VideoMode, "ImDotEngine", Styles.Default);

        Size = new Vector2u(800, 600);

        window.SetIcon(32, 32, Assets.Image_x32.Pixels);

        Instance.Engine = this;

        window.Closed += (s, e) =>
        {
            Closing();
            window.Close();
        };

        window.Resized += (s, e) =>
        {
            Resize(e);

            Size = new Vector2u(e.Width, e.Height);
        };
        
        {
            // camera view
            View view = new View();

            // background
            RectangleShape shape = new RectangleShape
            {
                Size = (Vector2f)Assets.Intro_x580.Size,
                Texture = new Texture(Assets.Intro_x580)
            };

            // info text
            Text infoText = new Text()
            {
                CharacterSize = 32,
                FillColor = Color.Black,
                Position = new Vector2f(10, 10),
                Font = Instance.FontRepository.GetFont("arial"),
                DisplayedString = "Caching assets.."
            };

            // render intro frame while we load crap
            {
                window.DispatchEvents();

                window.Clear(Color.White);

                view.Reset(new FloatRect(new Vector2f(0, 0), (Vector2f)Size));
                view.Zoom(1.5f);
                view.Center = new Vector2f(Assets.Intro_x580.Size.X / 2, Assets.Intro_x580.Size.Y / 2);
                window.SetView(view);

                window.Draw(shape);
                window.Draw(infoText);

                window.Display();
            }

            LoadAssets();
        }

        window.GainedFocus += (s, e) => Focus();
        window.LostFocus += (s, e) => LostFocus();

        window.JoystickButtonPressed += (s, e) => JoystickButtonPressed(e);
        window.JoystickButtonReleased += (s, e) => JoystickButtonReleased(e);
        window.JoystickDisconnected += (s, e) => JoystickDisconnected(e);
        window.JoystickMoved += (s, e) => JoystickMoved(e);

        window.KeyPressed += (s, e) => KeyPressed(e);
        window.KeyReleased += (s, e) => KeyReleased(e);

        window.MouseButtonPressed += (s, e) => MouseButtonPressed(e);
        window.MouseButtonReleased += (s, e) => MouseButtonReleased(e);
        window.MouseMoved += (s, e) => MouseMoved(e);
        window.MouseWheelScrolled += (s, e) => MouseWheelScrolled(e);
        window.MouseEntered += (s, e) => MouseEntered();
        window.MouseLeft += (s, e) => MouseLeft();

        window.SensorChanged += (s, e) => SensorChanged(e);
        window.TextEntered += (s, e) => TextEntered(e);

        window.TouchBegan += (s, e) => TouchBegan(e);
        window.TouchEnded += (s, e) => TouchEnded(e);
        window.TouchMoved += (s, e) => TouchMoved(e);

        window.SetActive();

        Initialized();

        // physics thread
        Task.Factory.StartNew(() =>
        {
            var targetTicksPerFrame = TimeSpan.TicksPerSecond / targetPPS;
            var prevTicks = DateTime.Now.Ticks;

            while (window.IsOpen)
            {
                var currTicks = DateTime.Now.Ticks;
                var elapsedTicks = currTicks - prevTicks;

                if (elapsedTicks >= targetTicksPerFrame)
                {
                    // update the camera
                    prevTicks = currTicks;

                    OnFixedUpdate();

                    physicStepCount++;

                    var curTime = DateTime.Now.Ticks;
                    if (curTime - lastPhysicsStep >= TimeSpan.TicksPerSecond)
                    {
                        CurrentPPS = physicStepCount;
                        physicStepCount = 0;
                        lastPhysicsStep = curTime;
                    }

                    Instance.GuiData.StepTime = Stopwatch.StartNew();
                }
            }
        });

        {

            var prevTicks = DateTime.Now.Ticks;

            while (window.IsOpen)
            {
                var currTicks = DateTime.Now.Ticks;
                var elapsedTicks = currTicks - prevTicks;

                prevTicks = currTicks;

                window.DispatchEvents();

                OnUpdate(window); // redraw window

                window.Display(); // swapbuffers

                frameCount++;

                if (currTicks - fpsLastTime >= TimeSpan.TicksPerSecond)
                {
                    CurrentFPS = frameCount;
                    frameCount = 0;
                    fpsLastTime = currTicks;
                }

                Instance.GuiData.FrameTime = Stopwatch.StartNew();
            }
        }
    }

    #region Easy Game Properties

    /// <summary>
    /// Vertical Sync, if true, the game will wait for the vertical blank to update the screen.
    /// </summary>
    public bool VSync
    {
        get => vsync;
        set
        {
            vsync = value;
            window.SetVerticalSyncEnabled(value);
        }
    }

    /// <summary>
    /// Target framerate, 0 for unlimited
    /// </summary>
    public uint TargetFramerate
    {
        get => framerate;
        set
        {
            framerate = value;
            window.SetFramerateLimit(value);
        }
    }

    /// <summary>
    /// If the window has focus or not
    /// </summary>
    public bool HasFocus
    {
        get => window.HasFocus();
        set => window.RequestFocus(); // TODO: add a check here
    }

    /// <summary>
    /// Camera Size
    /// </summary>
    public Vector2u Size
    {
        get => _Size;
        set
        {
            _Size = value;
            window.Size = value;
        }
    }

    /// <summary>
    /// Window Title
    /// </summary>
    public String Title
    { set => window.SetTitle(value); }

    /// <summary>
    /// Target physics update rate (fixed)
    /// </summary>
    public int TargetPhysicsRate
    {
        get => targetPPS;
        set => targetPPS = value;
    }

    #endregion

    #region Overrides

    protected virtual void OnUpdate(RenderWindow ctx)
    {
        // bad version of ECS
        foreach (BaseService component in Services)
            component.OnUpdate(ctx);
    }

    protected virtual void OnFixedUpdate()
    {
        // bad version of ECS
        foreach (BaseService component in Services)
            component.OnFixedUpdate();
    }

    public virtual void Initialized()
    {
        foreach (BaseService component in Services)
            if (!component.isInit)
            {
                component.isInit = true;
                component.Initialized();
            }
    }

    public virtual void Closing()
    {
        foreach (BaseService component in Services)
            component.Closing();
    }

    public virtual void Resize(SizeEventArgs e)
    {
        foreach (BaseService component in Services)
            component.Resize(e);
    }

    public virtual void Focus()
    {
        foreach (BaseService component in Services)
            component.Focus();
    }

    public virtual void LostFocus()
    {
        foreach (BaseService component in Services)
            component.LostFocus();
    }

    public virtual void JoystickButtonPressed(JoystickButtonEventArgs e)
    {
        foreach (BaseService component in Services)
            component.JoystickButtonPressed(e);
    }

    public virtual void JoystickButtonReleased(JoystickButtonEventArgs e)
    {
        foreach (BaseService component in Services)
            component.JoystickButtonReleased(e);
    }

    public virtual void JoystickConnected(JoystickConnectEventArgs e)
    {
        foreach (BaseService component in Services)
            component.JoystickConnected(e);
    }

    public virtual void JoystickDisconnected(JoystickConnectEventArgs e)
    {
        foreach (BaseService component in Services)
            component.JoystickDisconnected(e);
    }

    public virtual void JoystickMoved(JoystickMoveEventArgs e)
    {
        foreach (BaseService component in Services)
            component.JoystickMoved(e);
    }

    public virtual void KeyPressed(KeyEventArgs e)
    {
        foreach (BaseService component in Services)
            component.KeyPressed(e);
    }

    public virtual void KeyReleased(KeyEventArgs e)
    {
        foreach (BaseService component in Services)
            component.KeyReleased(e);
    }

    public virtual void MouseButtonPressed(MouseButtonEventArgs e)
    {
        foreach (BaseService component in Services)
            component.MouseButtonPressed(e);
    }

    public virtual void MouseButtonReleased(MouseButtonEventArgs e)
    {
        foreach (BaseService component in Services)
            component.MouseButtonReleased(e);
    }

    public virtual void MouseMoved(MouseMoveEventArgs e)
    {
        //update guidata
        Instance.GuiData.CursorPos = new Vector2f(e.X, e.Y);

        foreach (BaseService component in Services)
            component.MouseMoved(e);
    }

    public virtual void MouseWheelScrolled(MouseWheelScrollEventArgs e)
    {
        foreach (BaseService component in Services)
            component.MouseWheelScrolled(e);
    }

    public virtual void MouseEntered()
    {
        foreach (BaseService component in Services)
            component.MouseEntered();
    }

    public virtual void MouseLeft()
    {
        foreach (BaseService component in Services)
            component.MouseLeft();
    }

    public virtual void SensorChanged(SensorEventArgs e)
    {
        foreach (BaseService component in Services)
            component.SensorChanged(e);
    }

    public virtual void TextEntered(TextEventArgs e)
    {
        foreach (BaseService component in Services)
            component.TextEntered(e);
    }

    public virtual void TouchBegan(TouchEventArgs e)
    {
        foreach (BaseService component in Services)
            component.TouchBegan(e);
    }

    public virtual void TouchEnded(TouchEventArgs e)
    {
        foreach (BaseService component in Services)
            component.TouchEnded(e);
    }

    public virtual void TouchMoved(TouchEventArgs e)
    {
        foreach (BaseService component in Services)
            component.TouchMoved(e);
    }

    public virtual void LoadAssets() { }

    #endregion
}
#endif