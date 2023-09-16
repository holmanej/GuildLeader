using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuildLeader
{
    internal class TextObject : RenderObject
    {
        public string Text
        {
            get => _Text;
            set
            {
                if (value != _Text)
                {
                    _Text = value;
                    WriteString();
                    UpdateGeometry = true;
                }
            }
        }

        public RenderObject FontSet;
        public string _Text;

        public TextObject(string text, RenderObject fontset, OpenGL_Shader shader)
        {
            FontSet = fontset;
            Geometry_Shader = shader;
            _Text = text;
            WriteString();
        }

        private void WriteString()
        {
            var chars = new List<Polygon>();
            float width = 0;
            try
            {
                for (int i = 0; i < _Text.Length; i++)
                {
                    var c = FontSet.Polygons[' '];
                    if (_Text[i] - ' ' < FontSet.Polygons.Count)
                    {
                        c = FontSet.Polygons[_Text[i] - ' '];
                    }
                    int cx = c.ImageSize.Width;
                    int cy = c.ImageSize.Height;
                    chars.Add(new Polygon()
                    {
                        VertexData = new List<float>()
                        {
                            width,      0,  0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                            width + cx, 0,  0, 0, 0, 0, 0, 0, 0, 0, 1, 1,
                            width,      cy, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                            width + cx, 0,  0, 0, 0, 0, 0, 0, 0, 0, 1, 1,
                            width,      cy, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                            width + cx, cy, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0
                        },
                        ImageData = c.ImageData,
                        ImageSize = c.ImageSize,
                        TextureBufferObject = c.TextureBufferObject,
                        ImageUpdate = true,
                        Metal = 0.5f,
                        Rough = 0.5f
                    });
                    width += cx;
                    //height = Math.Max(height, cy);
                }
            }
            catch (Exception e)
            {
                Debug.Print("TextObject Invalid Character Exception: {0} - Code: {1}", e.Message, e.HResult);
            }

            Polygons = chars;
        }

        public override void RenderGeometry()
        {
            Stopwatch sw = new Stopwatch();
            if (Visible && Geometry_Shader != null)
            {
                Geometry_Shader.Use();
                Geometry_Shader.SetMatrix4("obj_translate", PositionMatrix);
                Geometry_Shader.SetMatrix4("obj_scale", ScalingMatrix);
                Geometry_Shader.SetFloat("tex_alpha", Alpha);

                foreach (Polygon poly in Polygons)
                {
                    if (poly.TextureBufferObject == 0)
                    {
                        poly.TextureBufferObject = GL.GenTexture();
                        poly.ImageUpdate = true;
                    }

                    GL.ActiveTexture(TextureUnit.Texture0);
                    GL.BindTexture(TextureTarget.Texture2D, poly.TextureBufferObject);

                    if (poly.ImageUpdate)
                    {
                        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.SrgbAlpha, poly.ImageSize.Width, poly.ImageSize.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, poly.ImageData);
                        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
                        poly.ImageUpdate = false;
                    }
                    sw.Start();

                    GL.BindVertexArray(VertexBufferObject);
                    GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
                    GL.BufferData(BufferTarget.ArrayBuffer, poly.VertexData.Count * sizeof(float), poly.VertexData.ToArray(), ObjectUsage);
                    GL.DrawArrays(PrimitiveType.Triangles, 0, poly.VertexData.Count);
                    GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                    sw.Stop();
                }
            }
        }
    }
}
