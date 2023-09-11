using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using OpenTK.Mathematics;
using System.Diagnostics;
using OpenTK.Graphics.OpenGL4;

namespace GuildLeader
{
    public class RenderObject
    {
        public List<RenderObject> Children = new List<RenderObject>();
        public List<Polygon> Polygons = new List<Polygon>();
        public OpenGL_Shader Shader;

        public bool Visible = true;
        public bool Collidable = true;

        public Matrix4 PositionMatrix = Matrix4.Identity;
        public Matrix4 ScalingMatrix = Matrix4.Identity;
        public Matrix4 RotationMatrix = Matrix4.Identity;

        private Vector3 _Position = new Vector3(0, 0, 0);
        private Vector3 _Scale = new Vector3(1, 1, 1);
        private Vector3 _Rotation = new Vector3(0, 0, 0);
        private float _Alpha = 1.0f;

        public virtual void Render(int vArrayObj)
        {
            Stopwatch sw = new Stopwatch();
            if (Visible && Shader != null)
            {
                //Debug.Print("Rendering object");
                Shader.Use();
                Shader.SetMatrix4("obj_translate", PositionMatrix);
                Shader.SetMatrix4("obj_scale", ScalingMatrix);
                Shader.SetMatrix4("obj_rotate", RotationMatrix);
                foreach (Polygon poly in Polygons)
                {
                    //Debug.Print("Rendering polygon {0}", poly.ImageHandle);
                    if (poly.ImageHandle == 0)
                    {
                        poly.ImageHandle = GL.GenTexture();
                        poly.ImageUpdate = true;
                    }

                    GL.ActiveTexture(TextureUnit.Texture0);
                    GL.BindTexture(TextureTarget.Texture2D, poly.ImageHandle);

                    if (poly.ImageUpdate)
                    {
                        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, poly.ImageSize.Width, poly.ImageSize.Height, 0, OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, poly.ImageData);
                        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
                        poly.ImageUpdate = false;
                    }
                    sw.Start();

                    //Debug.Print("Vcnt {0}", poly.VertexData.Count);
                    GL.BindVertexArray(vArrayObj);
                    GL.BindBuffer(BufferTarget.ArrayBuffer, vArrayObj);
                    GL.BufferData(BufferTarget.ArrayBuffer, poly.VertexData.Count * sizeof(float), poly.VertexData.ToArray(), BufferUsageHint.DynamicDraw);
                    GL.DrawArrays(PrimitiveType.Triangles, 0, poly.VertexData.Count);
                    GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                    sw.Stop();
                }
            }
        }

        public Vector3 Position
        {
            get { return _Position; }
            set
            {
                if (value != _Position)
                {
                    _Position = value;
                    PositionMatrix = Matrix4.CreateTranslation(_Position);
                }
            }
        }

        public Vector3 Scale
        {
            get { return _Scale; }
            set
            {
                if (value != _Scale)
                {
                    _Scale = value;
                    ScalingMatrix = Matrix4.CreateScale(_Scale);
                }
            }
        }

        public Vector3 Rotation
        {
            get { return _Rotation; }
            set
            {
                if (value != _Rotation)
                {
                    _Rotation = value;
                    RotationMatrix = Matrix4.CreateRotationX(_Rotation.X * 3.14f / 180) * Matrix4.CreateRotationY(_Rotation.Y * 3.14f / 180) * Matrix4.CreateRotationZ(_Rotation.Z * 3.14f / 180);
                }
            }
        }

        public float Alpha
        {
            get { return _Alpha; }
            set
            {
                if (value != _Alpha)
                {
                    _Alpha = value;
                    foreach (var poly in Polygons)
                    {
                        for (int i = 9; i + 12 < poly.VertexData.Count; i += 12)
                        {
                            poly.VertexData[i] = value;
                        }
                    }
                }
            }
        }

        public float GetDistance(Vector3 target)
        {
            return Vector3.Distance(Position, target);
        }

        public float GetAngle(Vector3 target)
        {
            float dx = target.X - Position.X;
            float dy = target.Y - Position.Y;
            float theta = (float)Math.Atan(dy / dx);
            if (dx < 0) { theta += 3.14f; }
            float dt = theta * 180f / 3.14f - Rotation.Z - 90;
            if (Math.Abs(dt) > 180) { dt -= Math.Sign(dt) * 360; }
            return dt;
        }

        public void Translate(float x, float y, float z)
        {
            _Position.X = Position.X + x;
            _Position.Y = Position.Y + y;
            _Position.Z = Position.Z + z;
            PositionMatrix = Matrix4.CreateTranslation(_Position);
        }

        public void ReSize(float x, float y, float z)
        {
            _Scale.X = Scale.X + x;
            _Scale.Y = Scale.Y + y;
            _Scale.Z = Scale.Z + z;
            ScalingMatrix = Matrix4.CreateScale(_Scale);
        }

        public void Rotate(float x, float y, float z)
        {
            _Rotation.X = Rotation.X + x;
            _Rotation.Y = Rotation.Y + y;
            _Rotation.Z = Rotation.Z + z;
            RotationMatrix = Matrix4.CreateRotationX(_Rotation.X * 3.14f / 180) * Matrix4.CreateRotationY(_Rotation.Y * 3.14f / 180) * Matrix4.CreateRotationZ(_Rotation.Z * 3.14f / 180);
        }

        public void SetPosition(float x, float y, float z)
        {
            _Position.X = x;
            _Position.Y = y;
            _Position.Z = z;
            PositionMatrix = Matrix4.CreateTranslation(_Position);
        }

        public void SetScale(float x, float y, float z)
        {
            _Scale.X = x;
            _Scale.Y = y;
            _Scale.Z = z;
            ScalingMatrix = Matrix4.CreateScale(_Scale);
        }

        public void SetRotation(float x, float y, float z)
        {
            _Rotation.X = x;
            _Rotation.Y = y;
            _Rotation.Z = z;
            RotationMatrix = Matrix4.CreateRotationX(_Rotation.X * 3.14f / 180) * Matrix4.CreateRotationY(_Rotation.Y * 3.14f / 180) * Matrix4.CreateRotationZ(_Rotation.Z * 3.14f / 180);
        }

        public class Polygon
        {
            public List<float> VertexData = new List<float>();
            public Vector3 SurfaceNormal = Vector3.Zero;
            public byte[] ImageData = Array.Empty<byte>();
            public Size ImageSize;
            public int ImageHandle;
            public bool ImageUpdate;
            public float Metal;
            public float Rough;
            public float Alpha = 1f;

            public Polygon() { }

            public Polygon(Polygon poly)
            {
                VertexData = poly.VertexData;
                SurfaceNormal = poly.SurfaceNormal;
                ImageData = poly.ImageData;
                ImageSize = poly.ImageSize;
                ImageHandle = 0;
                ImageUpdate = true;
                Metal = poly.Metal;
                Rough = poly.Rough;
                Alpha = poly.Alpha;
                //CalculateNormalData();
            }

            public Polygon(Image img)
            {
                Bitmap bmp;
                using (MemoryStream stream = new MemoryStream())
                {
                    img.Save(stream, ImageFormat.Bmp);
                    bmp = (Bitmap)Image.FromStream(stream);
                }

                bmp.MakeTransparent(bmp.GetPixel(0, 0));
                BitmapData fileData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                byte[] imgData = new byte[bmp.Width * bmp.Height * 4];
                Marshal.Copy(fileData.Scan0, imgData, 0, imgData.Length);
                bmp.UnlockBits(fileData);

                float w = 1f;
                float h = 1f;
                VertexData = new List<float>()
                {
                    -w, -h, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                    w,  -h, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1,
                    -w,  h, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                    w,  -h, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1,
                    -w,  h, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                    w,   h, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0
                };

                ImageData = imgData;
                ImageSize = bmp.Size;
                Metal = 0.5f;
                Rough = 0.5f;

                bmp.Dispose();
            }

            private void CalculateNormalData()
            {
                for (int i = 0; i < VertexData.Count; i += 36)
                {
                    Vector3 a = new Vector3(VertexData[i], VertexData[i + 1], VertexData[i + 2]);
                    Vector3 b = new Vector3(VertexData[i + 12], VertexData[i + 13], VertexData[i + 14]);
                    Vector3 c = new Vector3(VertexData[i + 24], VertexData[i + 25], VertexData[i + 26]);
                    SurfaceNormal = Vector3.Cross(b - a, c - a);
                }
            }
        }
    }
}
