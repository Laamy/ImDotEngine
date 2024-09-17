using SFML.Window;
using System;

using static User32;

#if CLIENT
class BorderlessInfo
{
    public int Style = 0;
    public RECT Dimensions = new RECT();
}

class DefaultWindowBinds : BaseComponent
{
    public ClientInstance Instance = ClientInstance.GetSingle();
    
    // old data from when it wasn't fullscreen
    BorderlessInfo desktopMode;

    public DefaultWindowBinds()
    {
        DebugLogger.Log("Components", $"Initialized : DefaultWindowBinds");
    }

    // todo make it so this is actually ufcking useful so i can actually toggle fullscreen by modifying the games context flags
    public override void KeyPressed(KeyEventArgs e)
    {
        if (e.Code == Keyboard.Key.Escape)
            Instance.Engine.window.Close();

        if (e.Code == Keyboard.Key.F11)
        {
            Instance.GameContext.TryToggleFlag<FlagComponent<FullscreenFlag>>();

            IntPtr hwnd = Instance.Engine.window.SystemHandle;

            if (Instance.GameContext.HasComponent<FlagComponent<FullscreenFlag>>())
            {
                desktopMode = SetBorderlessFullscreen(hwnd);
            }
            else
            {
                RestoreWindowStyle(hwnd, desktopMode);
            }
        }
    }

    // borderless fullscreen
    private static BorderlessInfo SetBorderlessFullscreen(IntPtr hwnd)
    {
        int style = GetWindowLong(hwnd, GWL_STYLE);

        BorderlessInfo result = new BorderlessInfo();

        if (GetWindowRect(hwnd, out RECT rect))
        {
            result = new BorderlessInfo()
            {
                Style = style,
                Dimensions = rect
            };
        }

        SetWindowLong(hwnd, GWL_STYLE, style & ~WS_BORDER);

        // update da window a bit 
        ShowWindow(hwnd, SW_MINIMIZE);
        ShowWindow(hwnd, SW_MAXIMIZE);

        return result;
    }

    private static void RestoreWindowStyle(IntPtr form, BorderlessInfo info)
    {
        SetWindowLong(form, GWL_STYLE, info.Style);

        int x = info.Dimensions.Left;
        int y = info.Dimensions.Top;

        int width = info.Dimensions.Right - x;
        int height = info.Dimensions.Bottom - y;

        SetWindowPos(form, HWND_NOTOPMOST, x, y, width, height, SWP_SHOWWINDOW);
    }
}
#endif