using SFML.Audio;
using SFML.System;

using System.Linq;

#if CLIENT
class SoundComponent : BaseComponent
{
    public SoundComponent()
    {
        DebugLogger.Log("Components", $"Initialized : SoundComponent");
    }

    public override void OnFixedUpdate()
    {
        var Instance = ClientInstance.GetSingle();

        var Audio = Instance.AudioRepository;

        var player = Instance.Engine.Components.OfType<LocalPlayer>().FirstOrDefault();

        Listener.Position = new Vector3f(player.curPos.X, player.curPos.Y, 0);
        Listener.Direction = new Vector3f(1, 0, 0);
        Listener.UpVector = new Vector3f(0, 1, 0);
    }

    public override void Initialized()
    {
        var Instance = ClientInstance.GetSingle();

        var Audio = Instance.AudioRepository;

        SoundEffect sound = new SoundEffect();

        {
            var music = Audio.GetSound("Audio\\boards_of_canada_5978.ogg");

            music.Loop = true;
            music.Volume = 10;
            music.Attenuation = 1;
            music.MinDistance = 0;

            sound.Sound = music;
        }

        Audio.Play(sound);
    }
}
#endif