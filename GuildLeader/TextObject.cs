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
        public RenderObject FontSet;
        public string _Text;

        public TextObject(string text, RenderObject fontset, OpenGL_Shader shader)
        {
            FontSet = fontset;
            Shader = shader;
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
                    var c = FontSet.Polygons[_Text[i] - ' '];
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

        public string Text
        {
            get { return _Text; }
            set
            {
                if (value != _Text)
                {
                    _Text = value;
                    WriteString();
                }
            }
        }
    }
}
