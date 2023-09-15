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
            var cubex = Assets.ImportGLTF("cube_1m.gltf", Assets.Shaders["gltf"]); cubex.SetPosition(5, 0, 0);
            var cubey = Assets.ImportGLTF("cube_1m.gltf", Assets.Shaders["gltf"]); cubey.SetPosition(0, 5, 0);
            var cubez = Assets.ImportGLTF("cube_1m.gltf", Assets.Shaders["gltf"]); cubez.SetPosition(0, 0, 5);
            var g = Assets.ImportGLTF("ground_1.gltf", Assets.Shaders["gltf"]); g.SetScale(40, 60, 40);
            var p1 = Assets.ImportGLTF("person_1.gltf", Assets.Shaders["gltf"]); p1.SetPosition(0, -0.3f, 0);
            var p2 = Assets.ImportGLTF("person_1.gltf", Assets.Shaders["gltf"]); p2.SetPosition(-75f, 5f, 450f);
            var h1 = Assets.ImportGLTF("house_1.gltf", Assets.Shaders["gltf"]); h1.SetPosition(-400f, 7.5f, 1720f);

            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    //var g = Assets.ImportGLTF("ground_1.gltf", Assets.Shaders["gltf"]);
                    //g.SetPosition(i * 101, 0, j * 101);
                    //window.Objects.Add(g);
                }
            }

            window.Objects.Add(cubex);
            window.Objects.Add(cubey);
            window.Objects.Add(cubez);
            window.Objects.Add(g);
            window.Objects.Add(p1);
            window.Objects.Add(p2);
            window.Objects.Add(h1);

            window.VSync = VSyncMode.Off;
            window.UpdateFrequency = 120;
            window.Run();
        }
    }
}