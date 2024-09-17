using SFML.Audio;
using SFML.System;
using System;

internal class SoundEffect
{
    public Sound Sound { get; set; }

    public Vector2f Position { get; set; } = new Vector2f(100, 0);

    public void Play()
    {
        Sound.Position = new Vector3f(Position.X, Position.Y, 0);
        Sound.Play();
    }
    public void Stop() => Sound.Stop();
}