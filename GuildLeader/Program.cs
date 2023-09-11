using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System.Diagnostics;
using static GuildLeader.RenderObject;
using static OpenTK.Graphics.OpenGL.GL;
using static System.Formats.Asn1.AsnWriter;

namespace GuildLeader
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // To create a new window, create a class that extends GameWindow, then call Run() on it.
            using var window = new Window(GameWindowSettings.Default, new NativeWindowSettings()
            {
                Size = new Vector2i(1280, 720),
                Location = new Vector2i(100, 50),
                Title = "Guild Leader",
                // This is needed to run on macos
                Flags = ContextFlags.ForwardCompatible,
            });
            // Load Assets
            Assets.LoadAssets();
            window.RenderObjects.Add(new TextObject("suh dude", Assets.FontSets["DebugFont"], Assets.Shaders["debugText_shader"]) { Position = new Vector3(-0.7f, 0.5f, 0), Scale = new Vector3(0.003f, 0.005f, 1f) });

            window.VSync = VSyncMode.Off;
            window.UpdateFrequency = 120;
            window.Run();
        }
    }
}