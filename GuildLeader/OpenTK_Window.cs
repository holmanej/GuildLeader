using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Diagnostics;

namespace GuildLeader
{
    // This is where all OpenGL code will be written.
    // OpenToolkit allows for several functions to be overriden to extend functionality; this is how we'll be writing code.
    public class Window : GameWindow
    {
        public List<RenderObject> RenderObjects = new List<RenderObject>();

        public Matrix4 Model;
        public Matrix4 View_Translate;
        public Matrix4 View_Rotate;
        public Matrix4 Projection;

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
            //var Readout_Position = new TextObject("Hello, World!", Resources.FontSets["DebugFont"], Resources.Shaders["debugText_shader"]) { Position = new Vector3(-1, 0.95f, 0), Scale  };
            //var Readout_Rotation = new TextObject("*", Resources.FontSets["DebugFont"], Resources.Shaders["debugText_shader"]) { Position = new Vector3(-1, 0.9f, 0) };
            //var Readout_FPS = new TextObject("*", Resources.FontSets["DebugFont"], Resources.Shaders["debugText_shader"]) { Position = new Vector3(-1, 0.85f, 0) };
            //RenderObjects.Add(Readout_Position);
            //RenderObjects.Add(Readout_Rotation);
            //RenderObjects.Add(Readout_FPS);

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
            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }
            //Debug.Print("Update T/f: {0} {1}", UpdateTime, UpdateFrequency);

            base.OnUpdateFrame(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            foreach (var obj in RenderObjects)
            {
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
