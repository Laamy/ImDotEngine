using SFML.Audio;

using System;
using System.IO;

namespace audiosample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var contents = File.ReadAllBytes("sound.ogg");

            Console.WriteLine(contents.Length);

            SoundBuffer sound = new SoundBuffer(contents);

            Sound music = new Sound(sound);

            music.Loop = true;
            music.RelativeToListener = true;

            music.Play();

            Console.ReadKey();
        }
    }
}
