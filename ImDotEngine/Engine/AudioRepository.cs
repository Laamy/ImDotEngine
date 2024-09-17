#region Includes

using SFML.Audio;

using System;
using System.Collections.Generic;
using System.IO;

#endregion

#if CLIENT
internal class AudioRepository
{
    private Dictionary<string, SoundBuffer> m_cache = new Dictionary<string, SoundBuffer>();

    private BundleInfo m_assetBundle;

    private List<SoundEffect> active_sounds = new List<SoundEffect>();

    public void Initialize()
    {
        DebugLogger.Log("Audio", "Caching audio..");

        m_cache.Clear();

        ClientInstance Instance = ClientInstance.GetSingle();

        m_assetBundle = Instance.BundleRepository.GetBundle("Audio");

        foreach (var file in m_assetBundle.Contents)
        {
            var fileName = file.Key;
            if (fileName.EndsWith(".ogg")) // NOTE: to lazy to have like 2 extra lines for wav mp3 aiff wav and the other shit it supports
            {
                var sound = new SoundBuffer(file.Value);
                m_cache.Add(fileName, sound);

                DebugLogger.Log("Audio", $"Loaded : {fileName}");
            }
        }

        DebugLogger.Log("Audio", $"Cached all audio");
    }

    public SoundBuffer GetAudio(string name)
    {
        if (m_cache.ContainsKey(name))
            return m_cache[name];

        throw new Exception("Invalid audio");
    }

    public Sound GetSound(string name)
    {
        var audio = GetAudio(name);

        Sound sound = new Sound(audio);

        return sound;
    }

    public void Play(SoundEffect sound)
    {
        sound.Play();

        active_sounds.Add(sound); // keep it in references to stop GC collection
    }

    public List<SoundEffect> GetActiveSounds() => active_sounds;

    public void Stop(SoundEffect sound)
    {
        sound.Stop();

        active_sounds.Remove(sound);
    }
}
#endif