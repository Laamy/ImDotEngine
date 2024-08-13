using SFML.Graphics;
using System;

internal class Assets
{
    public static Image Icon_x32 { get; set; }
    public static Image Icon_x64 { get; set; }

    public static Image Image_x32 { get; set; }
    public static Image Image_x64 { get; set; }

    public static Image Intro_x580 { get; set; }

    internal static void Initialize()
    {
        // for visual studio
        //Icon_x32 = new Image("Data\\Assets\\icon_x32.ico");
        //Icon_x64 = new Image("Data\\Assets\\icon_x64.ico");

        Image_x32 = new Image("Data\\Assets\\icon_x32.png");
        Image_x64 = new Image("Data\\Assets\\icon_x64.png");

        Intro_x580 = new Image("Data\\Assets\\icon_intro.png");
    }
}