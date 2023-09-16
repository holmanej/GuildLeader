using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Diagnostics;
using System.Drawing;

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

            //BuildWoodRoom(window);
            //BuildOutside(window);
            BuildStressTest(window, 10, 10);

            window.VSync = VSyncMode.Off;
            window.UpdateFrequency = 120;
            window.Run();
        }

        public static void BuildWoodRoom(Window w)
        {
            var floor = Assets.ImportGLTF("tex2cube2_1m.gltf", Assets.Shaders["gltf_shader"]); floor.SetPosition(10f, -1f, 10f); floor.SetScale(1, 20, 20); floor.SetRotation(0, 0, 90);
            var cube1 = Assets.ImportGLTF("tex2cube2_1m.gltf", Assets.Shaders["gltf_shader"]); cube1.SetPosition(3, 0, 1); cube1.SetRotation(0, 90, 0);
            var cube2 = Assets.ImportGLTF("tex2cube2_1m.gltf", Assets.Shaders["gltf_shader"]); cube2.SetPosition(0, 3, 0); cube2.SetRotation(0, -45, -45);
            var cube3 = Assets.ImportGLTF("tex2cube2_1m.gltf", Assets.Shaders["gltf_shader"]); cube3.SetPosition(-3, 0, 0); cube3.SetRotation(0, -45, 0);
            var cube4 = Assets.ImportGLTF("cube_1m.gltf", Assets.Shaders["gltf_shader"]); cube4.SetPosition(0, 1, 2); cube4.SetRotation(0, -45, 0);

            w.Objects.Add(floor);
            w.Objects.Add(cube1);
            w.Objects.Add(cube2);
            w.Objects.Add(cube3);
            w.Objects.Add(cube4);
        }

        public static void BuildOutside(Window w)
        {
            var cubex = Assets.ImportGLTF("cube_1m.gltf", Assets.Shaders["gltf_shader"]); cubex.SetPosition(5, 0, 0); cubex.ObjectUsage = OpenTK.Graphics.OpenGL4.BufferUsageHint.StreamDraw;
            var cubey = Assets.ImportGLTF("cube_1m.gltf", Assets.Shaders["gltf_shader"]); cubey.SetPosition(0, 5, 0); cubey.ObjectUsage = OpenTK.Graphics.OpenGL4.BufferUsageHint.StreamDraw;
            var cubez = Assets.ImportGLTF("cube_1m.gltf", Assets.Shaders["gltf_shader"]); cubez.SetPosition(0, 0, 5); cubez.ObjectUsage = OpenTK.Graphics.OpenGL4.BufferUsageHint.StreamDraw;
            var ground = Assets.ImportGLTF("ground_1.gltf", Assets.Shaders["gltf_shader"]); ground.SetScale(40, 60, 40);
            ground.ObjectUsage = OpenTK.Graphics.OpenGL4.BufferUsageHint.StaticDraw;

            w.Objects.Add(cubex);
            w.Objects.Add(cubey);
            w.Objects.Add(cubez);
            w.Objects.Add(ground);
        }

        public static void BuildStressTest(Window w, int x, int y)
        {
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    var g = Assets.ImportGLTF("ground_1.gltf", Assets.Shaders["gltf_shader"]);
                    //g.SetScale(10, 20, 10);
                    g.SetPosition(i * 102, 0, j * 102);
                    g.ObjectUsage = OpenTK.Graphics.OpenGL4.BufferUsageHint.StaticDraw;
                    w.Objects.Add(g);
                }
            }
        }
    }
}