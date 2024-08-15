using SFML.Window;
using System;

using static User32;

class BorderlessInfo
{
    public int Style = 0;
    public RECT Dimensions = new RECT();
}

class DefaultWindowBinds : BaseComponent
{
    public ClientInstance Instance = ClientInstance.GetSingle();

    bool isFullScreen = false;

    // old data from when it wasn't fullscreen
    BorderlessInfo desktopMode;

    public override void KeyPressed(KeyEventArgs e)
    {
        if (e.Code == Keyboard.Key.Escape)
            Instance.Engine.window.Close();

        if (e.Code == Keyboard.Key.F11)
        {
            isFullScreen = !isFullScreen;

            IntPtr hwnd = Instance.Engine.window.SystemHandle;

            if (isFullScreen)
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

        SetWindowPos(form, HWND_NOTOPMOST, x, y, width, height, SWP_NOACTIVATE | SWP_SHOWWINDOW);
    }
}