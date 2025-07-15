using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Color = SFML.Graphics.Color;

#if CLIENT
class UISceneService : BaseService
{
    public UISceneService()
    {
        DebugLogger.Log("Components", $"Initialized : UISceneService");
    }

    private List<MenuScene> UIScene = new();

    public override void OnUpdate(RenderWindow ctx)
    {
        foreach (var item in UIScene)
        {
            if (!item.IsAlive)
                item.Awake();
            else item.OnUpdate(ctx);
        }
    }

    public override void OnFixedUpdate()
    {
        foreach (var item in UIScene)
            if (item.IsAlive)
                item.OnFixedUpdate();
    }

    public override void MouseButtonPressed(MouseButtonEventArgs e)
    {
        base.MouseButtonPressed(e);

        var Camera = ClientInstance.GetService<Camera2DService>();

        if (UIScene.Count <= 0)
            return;

        var currentScene = UIScene.Last();
        foreach (var item in currentScene.GetDescendants())
        {
            if (item is MenuButton btn)
            {
                var pos = Camera.WorldToScreen(ClientInstance.GetRenderWindow(), item.GlobalPosition());

                RectangleF bounds = new RectangleF(pos.X, pos.Y, btn.Size.X, btn.Size.Y);

                if (bounds.Contains(e.X, e.Y))
                {
                    btn.OnClick.TriggerActions();
                    DebugLogger.Log("UISceneService", $"Button clicked: {btn.Text}");
                    break;
                }
            }
        }
    }

    //public override void JoystickButtonPressed(JoystickButtonEventArgs e)
    //{
    //    base.JoystickButtonPressed(e);
    //}

    public void SetUIScene(MenuScene scene)
    {
        if (scene == null)
            throw new NullReferenceException("MenuScene cannot be null");

        if (UIScene.Count > 0)
            UIScene.Clear();

        scene.Awake();
        UIScene.Add(scene);
    }

    public void DropUIScene() => UIScene.RemoveAt(UIScene.Count - 1);

    public void DropAll() => UIScene.Clear();

    public List<MenuScene> GetUIScene() =>
        UIScene.Count > 0 ? UIScene : Array.Empty<MenuScene>().ToList();
}

// TODO: make a single DisposableActor & HierarchyActor that i inherit from in MenuScene & MenuItem
class MenuScene
{
    private readonly List<MenuItem> Items = new();

    public bool IsAlive { get; private set; } = false;

    public virtual void OnUpdate(RenderWindow ctx)
    {
        foreach (var item in Items)
        {
            if (!item.IsAlive)
                item.Awake();
            else item.OnUpdate(ctx);
        }
    }

    public virtual void OnFixedUpdate()
    {
        foreach (var item in Items)
            if (item.IsAlive)
                item.OnFixedUpdate();
    }

    public virtual void Awake()
    {
        IsAlive = true;
        DebugLogger.Log("UIScene", "Awake : " + this.GetType().Name);
    }

    public void Add(MenuItem item)
    {
        if (item == null)
            throw new NullReferenceException("MenuItem cannot be null");

        Items.Add(item);
        item.Scene = this;
        item.Awake();
    }

    public void Remove(MenuItem item)
    {
        if (item == null)
            throw new NullReferenceException("MenuItem cannot be null");

        if (Items.Contains(item))
            Items.Remove(item);
    }

    public IEnumerable<MenuItem> GetChildren()
    {
        foreach (var item in Items)
        {
            if (item.IsAlive)
                yield return item;
        }
    }

    public List<MenuItem> GetDescendants()
    {
        List<MenuItem> descendants = new();
        foreach (var item in GetChildren())
        {
            if (item.IsAlive)
            {
                descendants.Add(item);
                descendants.AddRange(item.GetDescendants());
            }
        }
        return descendants;
    }
}

// TODO: an OnPropertyChanged event & cache for textures
class MenuItem : IDisposable
{
    private readonly List<MenuItem> Items = new();

    public bool IsAlive { get; private set; } = false;
    public MenuItem Parent { get; private set; } = null;
    public MenuScene Scene { get; /*private */set; } = null;

    public Vector2f Position = new();

    public Vector2f GlobalPosition() =>
        Parent != null ? Parent.GlobalPosition() + Position : Position;

    public virtual void OnUpdate(RenderWindow ctx)
    {
        foreach (var item in Items)
        {
            if (!item.IsAlive)
                item.Awake(this);
            else item.OnUpdate(ctx);
        }
    }

    public virtual void OnFixedUpdate()
    {
        foreach (var item in Items)
            if (item.IsAlive)
                item.OnFixedUpdate();
    }

    public virtual void Awake(MenuItem parent = null)
    {
        Parent = parent;
        IsAlive = true;
        //DebugLogger.Log("MenuItem", $"Parent {Parent?.ToString()}");
    }

    public void Dispose()
    {
        if (Parent != null)
        {
            if (Parent != null)
            {
                Parent.Remove(this);
                DebugLogger.Log("MenuItem", $"Removed {this.GetType().Name} from {Parent.GetType().Name}");
            }
        }
        else
        {
            if (Scene != null)
                Scene.Remove(this);
        }
    }

    public void Add(MenuItem item)
    {
        if (item == null)
            throw new NullReferenceException("MenuItem cannot be null");

        Items.Add(item);
        item.Awake(this); // v2
    }

    public void Remove(MenuItem item)
    {
        if (item == null)
            throw new NullReferenceException("MenuItem cannot be null");

        if (Items.Contains(item))
            Items.Remove(item);
    }

    public IEnumerable<MenuItem> GetChildren()
    {
        foreach (var item in Items)
        {
            if (item.IsAlive)
                yield return item;
        }
    }

    public List<MenuItem> GetDescendants()
    {
        List<MenuItem> descendants = new();
        foreach (var item in GetChildren())
        {
            if (item.IsAlive)
            {
                descendants.Add(item);
                descendants.AddRange(item.GetDescendants());
            }
        }
        return descendants;
    }
}

class MenuRect(Vector2f position, Vector2f size, Color color) : MenuItem
{
    public override void Awake(MenuItem parent = null)
    {
        base.Awake(parent);
        Position = position;
    }

    public Vector2f Size { get; private set; } = size;
    public Color Color { get; private set; } = color;

    public override void OnUpdate(RenderWindow ctx)
    {
        base.OnUpdate(ctx);

        // NOTE: this is very basic menus with minimal optimizations
        RectangleShape rect = new RectangleShape(Size)
        {
            Position = GlobalPosition(),
            FillColor = Color
        };
        ctx.Draw(rect);
    }
}

class MenuText(Vector2f position, string text, Color color, uint fontSize = 20) : MenuItem
{
    public override void Awake(MenuItem parent = null)
    {
        base.Awake(parent);
        Position = position;
    }

    public string Text { get; set; } = text;
    public Color Color { get; set; } = color;
    public uint FontSize { get; set; } = fontSize;

    public override void OnUpdate(RenderWindow ctx)
    {
        base.OnUpdate(ctx);

        Text sfmlText = new Text(Text, ClientInstance.instance.FontRepository.GetFont("Arial"), FontSize)
        {
            Position = GlobalPosition(),
            FillColor = Color
        };
        ctx.Draw(sfmlText);
    }
}

class ClickListener
{
    private readonly List<Action> ClickActions = new();

    public void AddAction(Action action)
    {
        if (action == null)
            throw new NullReferenceException("Action cannot be null");

        ClickActions.Add(action);
    }

    public void TriggerActions()
    {
        foreach (var action in ClickActions)
        {
            try
            {
                action?.Invoke();
            }
            catch (Exception ex)
            {
                DebugLogger.Log("ClickListener", $"Error executing action: {ex.Message}");
            }
        }
    }
}

class MenuButton(Vector2f position, Vector2f size, string text, Color color, Action onClick) : MenuItem
{
    public override void Awake(MenuItem parent = null)
    {
        base.Awake(parent);
        Position = position;
        Size = size;
        Text = text;
        Color = color;
        OnClick.AddAction(onClick);
    }

    public Vector2f Size { get; set; }
    public string Text { get; set; }
    public Color Color { get; set; }
    public ClickListener OnClick { get; private set; } = new ClickListener();

    public override void OnUpdate(RenderWindow ctx)
    {
        base.OnUpdate(ctx);
        RectangleShape rect = new RectangleShape(Size)
        {
            Position = GlobalPosition(),
            FillColor = Color
        };
        ctx.Draw(rect);
        Text sfmlText = new Text(Text, ClientInstance.instance.FontRepository.GetFont("Arial"), 20)
        {
            Position = GlobalPosition() + new Vector2f(10, 10),
            FillColor = Color.White
        };
        ctx.Draw(sfmlText);
    }
}
#endif