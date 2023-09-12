using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
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

        public float ViewX = 0f;
        public float ViewY = 0;
        public float ViewZ = 0f;
        public float AngleX = 0f;
        public float AngleY = 0f;
        private TextObject ViewDebug;
        private TextObject FPSDebug;
        private Queue<int> FPSQueue = new Queue<int>();

        private const int VPosition_loc = 0;
        private const int VNormal_loc = 1;
        private const int VColor_loc = 2;
        private const int TexCoord_loc = 3;
        private int VertexArrayObject;
        private int VertexBufferObject;
        private int TextureBufferObject;

        // A simple constructor to let us set properties like window size, title, FPS, etc. on the window.
        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
            VertexArrayObject = GL.GenVertexArray();
            VertexBufferObject = GL.GenBuffer();
        }

        protected override void OnLoad()
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            Projection = Matrix4.CreatePerspectiveFieldOfView(45f * 3.14f / 180f, (float)Size.X / Size.Y, 0.01f, 100f);

            int stride = 12;
            GL.BindVertexArray(VertexArrayObject);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.EnableVertexAttribArray(VPosition_loc);
            GL.VertexAttribPointer(VPosition_loc, 3, VertexAttribPointerType.Float, false, stride * sizeof(float), 0);

            GL.EnableVertexAttribArray(VNormal_loc);
            GL.VertexAttribPointer(VNormal_loc, 3, VertexAttribPointerType.Float, false, stride * sizeof(float), 3 * sizeof(float));

            GL.EnableVertexAttribArray(VColor_loc);
            GL.VertexAttribPointer(VColor_loc, 4, VertexAttribPointerType.Float, false, stride * sizeof(float), 6 * sizeof(float));

            GL.EnableVertexAttribArray(TexCoord_loc);
            GL.VertexAttribPointer(TexCoord_loc, 2, VertexAttribPointerType.Float, false, stride * sizeof(float), 10 * sizeof(float));
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            // Readouts
            ViewDebug = new TextObject("ViewDebug", Assets.FontSets["DebugFont"], Assets.Shaders["debugText_shader"]) { Position = new Vector3(-1f, 0.825f, 0f), Scale = new Vector3(0.0015f, 0.0025f, 1f) };
            FPSDebug = new TextObject("FpsDebug", Assets.FontSets["DebugFont"], Assets.Shaders["debugText_shader"]) { Position = new Vector3(-1f, 0.75f, 0f), Scale = new Vector3(0.0015f, 0.0025f, 1f) };
            Objects.Add(ViewDebug);
            Objects.Add(FPSDebug);

            FPSQueue.Enqueue(0);

            base.OnLoad();
        }

        protected override void OnUnload()
        {
            GL.DeleteVertexArray(VertexArrayObject);
            GL.DeleteBuffer(VertexBufferObject);
            GL.DeleteTexture(TextureBufferObject);
            //RenderObjects.ForEach(obj => obj.Shader.Dispose());

            base.OnUnload();
        }

        // This function runs on every update frame.
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            
            if (KeyboardState.IsKeyReleased(Keys.Escape))
            {
                Close();
            }

            float moveSpeed = 0.02f;
            float turnSpeed = 0.5f;

            float dsin = (float)Math.Sin(AngleY * 3.14f / 180) * moveSpeed;
            float dcos = (float)Math.Cos(AngleY * 3.14f / 180) * moveSpeed;
            float xMove = 0;
            float yMove = 0;
            float zMove = 0;

            if (KeyboardState.IsKeyDown(Keys.W))
            {
                xMove += dsin;
                zMove += -dcos;
            }
            if (KeyboardState.IsKeyDown(Keys.S))
            {
                xMove += -dsin;
                zMove += dcos;
            }
            if (KeyboardState.IsKeyDown(Keys.A))
            {
                xMove += -dcos;
                zMove += -dsin;
            }
            if (KeyboardState.IsKeyDown(Keys.D))
            {
                xMove += dcos;
                zMove += dsin;
            }
            
            if (KeyboardState.IsKeyDown(Keys.Space))
            {
                yMove += moveSpeed;
            }
            else if (KeyboardState.IsKeyDown(Keys.LeftControl))
            {
                yMove -= moveSpeed;
            }
            
            if (KeyboardState.IsKeyDown(Keys.E))
            {
                AngleY += turnSpeed;
                AngleY = AngleY >= 360 ? AngleY - 360 : AngleY;
            }
            else if (KeyboardState.IsKeyDown(Keys.Q))
            {
                AngleY -= turnSpeed;
                AngleY = AngleY < 0 ? AngleY + 360 : AngleY;
            }
            if (KeyboardState.IsKeyDown(Keys.R))
            {
                AngleX += turnSpeed;
                AngleX = AngleX >= 360 ? AngleX - 360 : AngleX;
            }
            else if (KeyboardState.IsKeyDown(Keys.F))
            {
                AngleX -= turnSpeed;
                AngleX = AngleX < 0 ? AngleX + 360 : AngleX;
            }
            if (KeyboardState.IsKeyReleased(Keys.F1))
            {
                ViewX = 0;
                ViewY = 0;
                ViewZ = 0;
                AngleX = 0;
                AngleY = 0;
            }

            ViewX += xMove;
            ViewY += yMove;
            ViewZ += zMove;

            // Debug stats
            FPSQueue.Enqueue((int)UpdateFrequency);
            if (FPSQueue.Count > 32)
            {
                FPSQueue.Dequeue();
            }

            base.OnUpdateFrame(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            ViewDebug.Text = string.Format("X: {0:F2} Y: {1:F2} Z: {2:F2} Ay: {3:F0} Ax: {4:F0}", ViewX, ViewY, ViewZ, AngleY, AngleX);
            FPSDebug.Text = string.Format("FPS: {0:F0}", FPSQueue.Average());

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
                obj.Render(VertexArrayObject);
            }

            GL.BindVertexArray(0);
            SwapBuffers();
            base.OnRenderFrame(e);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, Size.X, Size.Y);
            base.OnResize(e);
        }
    }
}
