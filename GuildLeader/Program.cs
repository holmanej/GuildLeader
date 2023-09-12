using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System.Diagnostics;

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
                Location = new Vector2i(100, 100),
                Title = "Guild Leader",
                // This is needed to run on macos
                Flags = ContextFlags.ForwardCompatible,
            });
            // Load Assets
            Assets.LoadAssets();
            var cubex = Assets.ImportGLTF("cube_1m.gltf", Assets.Shaders["gltf"]);
            var cubey = Assets.ImportGLTF("cube_1m.gltf", Assets.Shaders["gltf"]);
            var cubez = Assets.ImportGLTF("cube_1m.gltf", Assets.Shaders["gltf"]);
            var cubetex = Assets.ImportGLTF("tex2cube2_1m.gltf", Assets.Shaders["gltf"]);
            cubex.SetPosition(5, 0, 0);
            cubey.SetPosition(0, 5, 0);
            cubez.SetPosition(0, 0, 5);
            window.Objects.Add(cubex);
            window.Objects.Add(cubey);
            window.Objects.Add(cubez);
            window.Objects.Add(cubetex);

            window.VSync = VSyncMode.Off;
            window.UpdateFrequency = 120;
            window.Run();
        }
    }
}