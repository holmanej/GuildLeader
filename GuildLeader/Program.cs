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
            //var cubex = Assets.ImportGLTF("cube_1m.gltf", Assets.Shaders["gltf_shader"]); cubex.SetPosition(5, 0, 0); cubex.ObjectUsage = OpenTK.Graphics.OpenGL4.BufferUsageHint.StreamDraw;
            //var cubey = Assets.ImportGLTF("cube_1m.gltf", Assets.Shaders["gltf_shader"]); cubey.SetPosition(0, 5, 0); cubey.ObjectUsage = OpenTK.Graphics.OpenGL4.BufferUsageHint.StreamDraw;
            //var cubez = Assets.ImportGLTF("cube_1m.gltf", Assets.Shaders["gltf_shader"]); cubez.SetPosition(0, 0, 5); cubez.ObjectUsage = OpenTK.Graphics.OpenGL4.BufferUsageHint.StreamDraw;
            //var g = Assets.ImportGLTF("ground_1.gltf", Assets.Shaders["gltf_shader"]); g.SetScale(40, 60, 40);
            //var p1 = Assets.ImportGLTF("person_1.gltf", Assets.Shaders["gltf_shader"]); p1.SetPosition(0, -0.3f, 0);
            //var p2 = Assets.ImportGLTF("person_1.gltf", Assets.Shaders["gltf_shader"]); p2.SetPosition(-75f, 5f, 450f);
            //var h1 = Assets.ImportGLTF("house_1.gltf", Assets.Shaders["gltf_shader"]); h1.SetPosition(-400f, 7.5f, 1720f);

            //for (int i = 0; i < 10; i++)
            //{
            //    for (int j = 0; j < 10; j++)
            //    {
            //        var g = Assets.ImportGLTF("ground_1.gltf", Assets.Shaders["gltf_shader"]);
            //        //g.SetScale(10, 20, 10);
            //        g.SetPosition(i * 102, 0, j * 102);
            //        window.Objects.Add(g);
            //    }
            //}

            var floor = Assets.ImportGLTF("tex2cube2_1m.gltf", Assets.Shaders["gltf_shader"]); floor.SetPosition(10f, -1f, 10f); floor.SetScale(1, 20, 20); floor.SetRotation(0, 0, 90);
            var cube1 = Assets.ImportGLTF("tex2cube2_1m.gltf", Assets.Shaders["gltf_shader"]); cube1.SetPosition(3, 0, 1); cube1.SetRotation(0, 90, 0);
            var cube2 = Assets.ImportGLTF("tex2cube2_1m.gltf", Assets.Shaders["gltf_shader"]); cube2.SetPosition(0, 3, 0); cube2.SetRotation(0, -45, -45);
            var cube3 = Assets.ImportGLTF("tex2cube2_1m.gltf", Assets.Shaders["gltf_shader"]); cube3.SetPosition(-3, 0, 0); cube3.SetRotation(0, -45, 0);
            var cube4 = Assets.ImportGLTF("cube_1m.gltf", Assets.Shaders["gltf_shader"]); cube4.SetPosition(0, 1, 2); cube4.SetRotation(0, -45, 0);

            window.Objects.Add(floor);
            window.Objects.Add(cube1);
            window.Objects.Add(cube2);
            window.Objects.Add(cube3);
            window.Objects.Add(cube4);

            window.VSync = VSyncMode.Off;
            window.UpdateFrequency = 120;
            window.Run();
        }
    }
}