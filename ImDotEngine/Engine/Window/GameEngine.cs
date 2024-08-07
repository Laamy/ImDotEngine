#region Includes

using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Threading;
using System.Threading.Tasks;
using View = SFML.Graphics.View;

#endregion

internal class GameEngine
{
    private int targetFPS = 165;
    private int targetPPS = 30;

    public int CurrentFPS = 0;
    private int frameCount = 0;
    private long fpsLastTime = DateTime.Now.Ticks;

    public int CurrentPPS = 0;
    private int physicStepCount = 0;
    private long lastPhysicsStep = DateTime.Now.Ticks;

    private Vector2u _Size;

    // sdl/sfml stuff
    public RenderWindow window;

    // ECS stuff
    public List<BaseComponent> Components = new List<BaseComponent>();

    // all the game things insida here !
    public ClientInstance Instance = ClientInstance.GetSingle();

    public void Start()
    {
        // sdl renderer
        Instance.VideoMode = new VideoMode(800, 600);
        window = new RenderWindow(Instance.VideoMode, "Game Engine", Styles.Default);

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

        Winmm.TimeBeginPeriod(1);

        // physics thread
        Task.Factory.StartNew(() =>
        {
            long targetTicksPerFrame = TimeSpan.TicksPerSecond / targetPPS;
            long prevTicks = DateTime.Now.Ticks;

            while (window.IsOpen)
            {
                long currTicks = DateTime.Now.Ticks;
                long elapsedTicks = currTicks - prevTicks;

                if (elapsedTicks >= targetTicksPerFrame)
                {
                    // update the camera
                    prevTicks = currTicks;
                    OnFixedUpdate();

                    physicStepCount++;

                    long curTime = DateTime.Now.Ticks;
                    if (curTime - lastPhysicsStep >= TimeSpan.TicksPerSecond)
                    {
                        CurrentPPS = physicStepCount;
                        physicStepCount = 0;
                        lastPhysicsStep = curTime;
                    }
                }

                Thread.Sleep(1);
            }
        });

        {
            long targetTicksPerFrame = TimeSpan.TicksPerSecond / targetFPS;
            long prevTicks = DateTime.Now.Ticks;

            while (window.IsOpen)
            {
                long currTicks = DateTime.Now.Ticks;
                long elapsedTicks = currTicks - prevTicks;

                if (elapsedTicks >= targetTicksPerFrame)
                {
                    // update the camera
                    prevTicks = currTicks;

                    window.DispatchEvents();

                    OnUpdate(window); // redraw window

                    window.Display(); // swapbuffers

                    frameCount++;

                    long curTime = DateTime.Now.Ticks;
                    if (curTime - fpsLastTime >= TimeSpan.TicksPerSecond)
                    {
                        CurrentFPS = frameCount;
                        frameCount = 0;
                        fpsLastTime = curTime;
                    }
                }

                Thread.Sleep(1);
            }
        }

        Winmm.TimeEndPeriod(1);
    }

    protected virtual void OnUpdate(RenderWindow ctx)
    {
        // bad version of ECS
        foreach (BaseComponent component in Components)
            component.OnUpdate(ctx);
    }

    protected virtual void OnFixedUpdate()
    {
        // bad version of ECS
        foreach (BaseComponent component in Components)
            component.OnFixedUpdate();
    }

    #region Easy Game Properties

    /// <summary>
    /// Camera Size
    /// </summary>
    public Vector2u Size
    {
        get => _Size;
        set
        {
            _Size = value;
        }
    }

    /// <summary>
    /// Window Title
    /// </summary>
    public String Title
    { set => window.SetTitle(value); }

    public int TargetFramerate
    {
        get => targetFPS;
        set => targetFPS = value;
    }

    public int TargetPhysicsRate
    {
        get => targetPPS;
        set => targetPPS = value;
    }

    #endregion

    #region Overrides
    
    public virtual void Initialized()
    {
        // bad version of ECS
        foreach (BaseComponent component in Components)
            component.Initialized();
    }

    public virtual void Closing()
    {
        // bad version of ECS
        foreach (BaseComponent component in Components)
            component.Closing();
    }

    public virtual void Resize(SizeEventArgs e)
    {
        // bad version of ECS
        foreach (BaseComponent component in Components)
            component.Resize(e);
    }

    public virtual void Focus()
    {
        // bad version of ECS
        foreach (BaseComponent component in Components)
            component.Focus();
    }

    public virtual void LostFocus()
    {
        // bad version of ECS
        foreach (BaseComponent component in Components)
            component.LostFocus();
    }

    public virtual void JoystickButtonPressed(JoystickButtonEventArgs e)
    {
        // bad version of ECS
        foreach (BaseComponent component in Components)
            component.JoystickButtonPressed(e);
    }

    public virtual void JoystickButtonReleased(JoystickButtonEventArgs e)
    {
        // bad version of ECS
        foreach (BaseComponent component in Components)
            component.JoystickButtonReleased(e);
    }

    public virtual void JoystickConnected(JoystickConnectEventArgs e)
    {
        // bad version of ECS
        foreach (BaseComponent component in Components)
            component.JoystickConnected(e);
    }

    public virtual void JoystickDisconnected(JoystickConnectEventArgs e)
    {
        // bad version of ECS
        foreach (BaseComponent component in Components)
            component.JoystickDisconnected(e);
    }

    public virtual void JoystickMoved(JoystickMoveEventArgs e)
    {
        // bad version of ECS
        foreach (BaseComponent component in Components)
            component.JoystickMoved(e);
    }

    public virtual void KeyPressed(KeyEventArgs e)
    {
        // bad version of ECS
        foreach (BaseComponent component in Components)
            component.KeyPressed(e);
    }

    public virtual void KeyReleased(KeyEventArgs e)
    {
        // bad version of ECS
        foreach (BaseComponent component in Components)
            component.KeyReleased(e);
    }

    public virtual void MouseButtonPressed(MouseButtonEventArgs e)
    {
        // bad version of ECS
        foreach (BaseComponent component in Components)
            component.MouseButtonPressed(e);
    }

    public virtual void MouseButtonReleased(MouseButtonEventArgs e)
    {
        // bad version of ECS
        foreach (BaseComponent component in Components)
            component.MouseButtonReleased(e);
    }

    public virtual void MouseMoved(MouseMoveEventArgs e)
    {
        //update guidata
        Instance.GuiData.CursorPos = new Vector2f(e.X, e.Y);

        // bad version of ECS
        foreach (BaseComponent component in Components)
            component.MouseMoved(e);
    }

    public virtual void MouseWheelScrolled(MouseWheelScrollEventArgs e)
    {
        // bad version of ECS
        foreach (BaseComponent component in Components)
            component.MouseWheelScrolled(e);
    }

    public virtual void MouseEntered()
    {
        // bad version of ECS
        foreach (BaseComponent component in Components)
            component.MouseEntered();
    }

    public virtual void MouseLeft()
    {
        // bad version of ECS
        foreach (BaseComponent component in Components)
            component.MouseLeft();
    }

    public virtual void SensorChanged(SensorEventArgs e)
    {
        // bad version of ECS
        foreach (BaseComponent component in Components)
            component.SensorChanged(e);
    }

    public virtual void TextEntered(TextEventArgs e)
    {
        // bad version of ECS
        foreach (BaseComponent component in Components)
            component.TextEntered(e);
    }

    public virtual void TouchBegan(TouchEventArgs e)
    {
        // bad version of ECS
        foreach (BaseComponent component in Components)
            component.TouchBegan(e);
    }

    public virtual void TouchEnded(TouchEventArgs e)
    {
        // bad version of ECS
        foreach (BaseComponent component in Components)
            component.TouchEnded(e);
    }

    public virtual void TouchMoved(TouchEventArgs e)
    {
        // bad version of ECS
        foreach (BaseComponent component in Components)
            component.TouchMoved(e);
    }

    #endregion
}