using Newtonsoft.Json.Serialization;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Diagnostics;
using System.Drawing;

namespace GuildLeader
{
    // This is where all OpenGL code will be written.
    // OpenToolkit allows for several functions to be overriden to extend functionality; this is how we'll be writing code.
    public class Window : GameWindow
    {
        public List<RenderObject> Objects = new List<RenderObject>();

        private Stopwatch UpdateSW = new Stopwatch();
        private Stopwatch RenderSW = new Stopwatch();
        private Camera ActiveCam;
        private List<Camera> Cameras;
        private TextObject ViewDebug;
        private TextObject FPSDebug;
        private Queue<double> UpdateTime_Queue = new Queue<double>();
        private Queue<double> RenderTime_Queue = new Queue<double>();

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        protected override void OnLoad()
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.FramebufferSrgb);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            // Cameras
            Cameras = new List<Camera>()
            {
                new Camera()
                {
                    Name = "Spectator",
                    ResetKey = Keys.F1,
                    SelectKey = Keys.D1,
                    Projection = Matrix4.CreatePerspectiveFieldOfView(45f * 3.14f / 180f, (float)Size.X / Size.Y, 0.01f, 10000f),
                    TranslationSpeed = 400f,
                    VerticalSpeed = 100f,
                    RotationSpeed = 80f,
                },
                new Camera()
                {
                    Name = "Player",
                    ResetKey = Keys.F1,
                    SelectKey = Keys.D2,
                    Projection = Matrix4.CreatePerspectiveFieldOfView(45f * 3.14f / 180f, (float)Size.X / Size.Y, 0.01f, 10000f),
                    Position = new Vector3(0, 3, -7),
                    Rotation = new Vector3(350, 180, 0),
                    TranslationSpeed = 10f,
                    VerticalSpeed = 5f,
                    RotationSpeed = 70f,
                },
            };
            ActiveCam = Cameras[1];

            // Readouts
            FPSDebug = new TextObject("FpsDebug", Assets.FontSets["DebugFont"], Assets.Shaders["debugText_shader"]) { Position = new Vector3(-1f, 0.825f, 0f), Scale = new Vector3(0.001f, 0.0015f, 1f) };
            ViewDebug = new TextObject("ViewDebug", Assets.FontSets["DebugFont"], Assets.Shaders["debugText_shader"]) { Position = new Vector3(-1f, 0.75f, 0f), Scale = new Vector3(0.001f, 0.0015f, 1f) };
            Objects.Add(FPSDebug);
            Objects.Add(ViewDebug);

            UpdateTime_Queue.Enqueue(0);
            RenderTime_Queue.Enqueue(0);

            base.OnLoad();
        }

        protected override void OnUnload()
        {
            //RenderObjects.ForEach(obj => obj.Shader.Dispose());

            base.OnUnload();
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            if (e.Key == Keys.Escape) { Close(); }

            base.OnKeyUp(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            var updateTime = UpdateSW.Elapsed;
            //Debug.Print("tick: {0}", updateTime);
            UpdateSW.Restart();

            foreach (var c in Cameras)
            {
                if (c.CheckActive(KeyboardState))
                {
                    ActiveCam = c;
                }
            }
            ActiveCam.ReadInputs(KeyboardState, MouseState, updateTime.TotalSeconds);

            // Debug stats
            UpdateTime_Queue.Enqueue(1000000 / updateTime.TotalMicroseconds);
            if (UpdateTime_Queue.Count > 32)
            {
                UpdateTime_Queue.Dequeue();
            }

            base.OnUpdateFrame(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            RenderSW.Restart();
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            ViewDebug.Text = string.Format("X: {0:F2} Y: {1:F2} Z: {2:F2} Ay: {3:F0} Ax: {4:F0}", 
                ActiveCam.Position.X, ActiveCam.Position.Y, ActiveCam.Position.Z, ActiveCam.Rotation.Y, ActiveCam.Rotation.X);
            FPSDebug.Text = string.Format("Cam: {0} FPS: {1:F0} Update: {2:F0}", 
                ActiveCam.Name, RenderTime_Queue.Average(), UpdateTime_Queue.Average());

            ActiveCam.UpdateTransform();

            GL.Viewport(0, 0, Size.X, Size.Y);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            foreach (var obj in Objects)
            {
                obj.Geometry_Shader.Use();
                obj.Geometry_Shader.SetTransform("viewT", ActiveCam.ViewTranslation, ActiveCam.ViewScale, ActiveCam.ViewRotation);
                obj.Geometry_Shader.SetMatrix4("view_projection", ActiveCam.Projection);
                obj.RenderGeometry();
            }

            SwapBuffers();
            base.OnRenderFrame(e);

            var renderTime = RenderSW.Elapsed;
            //Debug.Print("Render Time: {0}", renderTime);
            RenderTime_Queue.Enqueue(1000000 / renderTime.TotalMicroseconds);
            if (RenderTime_Queue.Count > 32)
            {
                RenderTime_Queue.Dequeue();
            }
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, Size.X, Size.Y);
            base.OnResize(e);
        }
    }
}
