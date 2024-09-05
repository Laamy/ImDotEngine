public class DebugConfig
{
    /// <summary>
    /// shows the prevPos and curPos aswell as the velocity information and next spot the physics
    /// object is most likely to end up at.
    /// 
    /// FILE: DebugOverlay\DebugPhysicsDetails.cs
    /// </summary>
    public static bool ShowPhysicsDetails { get; set; } = true;

    /// <summary>
    /// show the game FPS, CPU usage, GPU usage latency between frames & physic steps aswell
    /// as memory usage per spatial partioning layer
    /// </summary>
    public static bool ShowUsage { get; set; } = true;

    /// <summary>
    /// unused, I saw this feature when reverse engineering minecraft bedrock editions param screen
    ///         they show boxes (with outlines) around all objects aswell as the hitboxes of their
    ///         parents. they used orange for the root objects and red for the children, they also
    ///         showed debug text like names sizes and other bits of useful info in the top right corner.
    /// TODO: implement this cuz i found it useful asf when developing cheats for MCBE
    /// </summary>
    public static bool ShowInspector { get; set; } = false;
}