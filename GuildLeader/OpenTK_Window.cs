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

        public Matrix4 Model;
        public Matrix4 View_Translate;
        public Matrix4 View_Scale;
        public Matrix4 View_Rotate;
        public Matrix4 Projection;

        public string CamName = "cam";
        public int CamSel;
        public float ViewX = 0f;
        public float ViewY = 0;
        public float ViewZ = 0f;
        public float AngleX = 0f;
        public float AngleY = 0f;

        private Stopwatch UpdateSW = new Stopwatch();
        private Stopwatch RenderSW = new Stopwatch();
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
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            Projection = Matrix4.CreatePerspectiveFieldOfView(45f * 3.14f / 180f, (float)Size.X / Size.Y, 0.01f, 10000f);

            // Cameras
            Cameras = new List<Camera>()
            {
                new Camera()
                {
                    Name = "Spectator",
                    TranslationSpeed = 400f,
                    VerticalSpeed = 100f,
                    RotationSpeed = 80f,
                    Active = true,
                },
                new Camera()
                {
                    Name = "Player",
                    TranslationSpeed = 10f,
                    VerticalSpeed = 5f,
                    RotationSpeed = 70f,
                    Active = false,
                },
            };

            // Readouts
            FPSDebug = new TextObject("FpsDebug", Assets.FontSets["DebugFont"], Assets.Shaders["debugText_shader"]) { Position = new Vector3(-1f, 0.8375f, 0f), Scale = new Vector3(0.0015f, 0.0025f, 1f) };
            ViewDebug = new TextObject("ViewDebug", Assets.FontSets["DebugFont"], Assets.Shaders["debugText_shader"]) { Position = new Vector3(-1f, 0.775f, 0f), Scale = new Vector3(0.0015f, 0.0025f, 1f) };
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

            int keyint = (int)e.Key;
            if (keyint >= (int)Keys.D1 && keyint <= (int)Keys.D9 && (keyint - (int)Keys.D1) < Cameras.Count) 
            {
                Cameras.ForEach(c => c.Active = false);
                CamSel = keyint - (int)Keys.D1;
                Cameras[CamSel].Active = true;
            }

            base.OnKeyUp(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            var updateTime = UpdateSW.Elapsed;
            //Debug.Print("tick: {0}", updateTime);
            UpdateSW.Restart();
            var cam = Cameras[CamSel];
            cam.ReadInputs(KeyboardState, MouseState, updateTime.TotalSeconds);

            if (KeyboardState.IsKeyReleased(Keys.F1))
            {
                cam.Position = Vector3.Zero;
                cam.Rotation = Vector3.Zero;
            }
            ViewX = cam.Position.X;
            ViewY = cam.Position.Y;
            ViewZ = cam.Position.Z;
            AngleX = cam.Rotation.X;
            AngleY = cam.Rotation.Y;
            CamName = cam.Name;

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

            ViewDebug.Text = string.Format("X: {0:F2} Y: {1:F2} Z: {2:F2} Ay: {3:F0} Ax: {4:F0}", ViewX, ViewY, ViewZ, AngleY, AngleX);
            FPSDebug.Text = string.Format("Cam: {0} FPS: {1:F0} Update: {2:F0}", CamName, RenderTime_Queue.Average(), UpdateTime_Queue.Average());

            View_Translate = Matrix4.CreateTranslation(-ViewX, -ViewY, -ViewZ);
            View_Scale = Matrix4.CreateScale(1f, 1f, 1f);
            View_Rotate = Matrix4.CreateRotationY(AngleY * 3.14f / 180) * Matrix4.CreateRotationX(-AngleX * 3.14f / 180);
            Vector3 playerPosition = new Vector3(ViewX, ViewY, ViewZ);

            foreach (var obj in Objects)
            {
                obj.Shader.Use();
                obj.Shader.SetVector3("player_position", playerPosition);
                obj.Shader.SetMatrix4("view_translate", View_Translate);
                obj.Shader.SetMatrix4("view_rotate", View_Rotate);
                obj.Shader.SetMatrix4("projection", Projection);
                obj.Render();
            }

            GL.BindVertexArray(0);
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
