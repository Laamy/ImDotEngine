using SFML.System;
using System;
using System.Diagnostics;

internal class GuiData
{
    public Vector2f CursorPos = new(0, 0);
    public Stopwatch FrameTime = new(); // time info
    public Stopwatch StepTime = new(); // step time info
}