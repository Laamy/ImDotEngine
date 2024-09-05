using SFML.System;
using System;
using System.Diagnostics;

internal class GuiData
{
    public Vector2f CursorPos = new Vector2f(0, 0);
    public Stopwatch FrameTime = new Stopwatch(); // time info
    public Stopwatch StepTime = new Stopwatch(); // step time info
}