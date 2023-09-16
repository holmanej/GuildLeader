using GuildLeader.resources;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuildLeader
{
    static class Assets
    {
        static public Dictionary<string, Image> Textures;
        static public Dictionary<string, OpenGL_Shader> Shaders;
        static public Dictionary<string, FontFamily> Fonts;
        static public Dictionary<string, RenderObject> FontSets;

        public static void LoadAssets()
        {
            Stopwatch LoadSW = Stopwatch.StartNew();
            Textures = LoadTextures();
            Shaders = LoadShaders();
            Fonts = LoadFonts();
            FontSets = new Dictionary<string, RenderObject>
            {
                { "DebugFont", CreateFontsetRender(Fonts["times"], Color.White, Color.Black, 48, Shaders["debugText_shader"]) },
                { "BigFont", CreateFontsetRender(Fonts["times"], Color.White, Color.Black, 48, Shaders["debugText_shader"]) },
                { "DedFont", CreateFontsetRender(Fonts["times"], Color.Red, Color.Black, 48, Shaders["debugText_shader"]) },
                { "WinFont", CreateFontsetRender(Fonts["times"], Color.Blue, Color.Black, 48, Shaders["debugText_shader"]) },
                { "UIFont", CreateFontsetRender(Fonts["times"], Color.Black, Color.White, 24, Shaders["debugText_shader"]) },
                { "HUDFont", CreateFontsetRender(Fonts["times"], Color.Black, Color.White, 16, Shaders["debugText_shader"]) }
            };
            LoadSW.Stop(); Debug.Print("Load Assets Time: {0}", LoadSW.Elapsed);
        }

        private static Dictionary<string, OpenGL_Shader> LoadShaders()
        {
            SetDir(@"/resources/shaders");

            Debug.WriteLine("Loading Shaders");
            Dictionary<string, OpenGL_Shader> shaders = new Dictionary<string, OpenGL_Shader>();
            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory());

            for (int i = 0; i < files.Length; i += 2)
            {
                //Debug.WriteLine(files[i + 1]);
                OpenGL_Shader shader = new OpenGL_Shader(files[i + 1], files[i]);
                string label = files[i].Substring(files[i].LastIndexOf('\\') + 1).Split('.')[0];
                Debug.WriteLine(label);

                shaders.Add(label, shader);
            }

            return shaders;
        }

        private static Dictionary<string, FontFamily> LoadFonts()
        {
            SetDir(@"/resources/fonts");
            Debug.WriteLine("Loading Fonts");

            var fonts = new Dictionary<string, FontFamily>();
            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.ttf");
            foreach (string f in files)
            {
                PrivateFontCollection pfc = new PrivateFontCollection();
                pfc.AddFontFile(f);
                string label = f.Substring(f.LastIndexOf('\\') + 1).Split('.')[0];
                Debug.WriteLine(label);

                fonts.Add(label, pfc.Families[0]);
            }

            return fonts;
        }

        private static RenderObject CreateFontsetRender(FontFamily fontfamily, Color fgColor, Color bgColor, int size, OpenGL_Shader shader)
        {
            var fontset = new RenderObject() { Geometry_Shader = shader };
            fontset.Polygons = new List<RenderObject.Polygon>();
            var font = new Font(fontfamily, size, GraphicsUnit.Pixel);
            for (int i = 32; i < 127; i++)
            {
                Bitmap bmp = new Bitmap(1, 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                Graphics g = Graphics.FromImage(bmp);
                SizeF charSize = g.MeasureString(Convert.ToChar(i).ToString(), font);
                Bitmap charBmp = new Bitmap((int)charSize.Width, font.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                g = Graphics.FromImage(charBmp);
                g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
                g.Clear(bgColor);
                g.DrawString(Convert.ToChar(i).ToString(), font, new SolidBrush(fgColor), 0, 0, StringFormat.GenericTypographic);
                int tbo = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, tbo);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.BindTexture(TextureTarget.Texture2D, 0);
                fontset.Polygons.Add(new RenderObject.Polygon(charBmp) { TextureBufferObject = tbo });
                bmp.Dispose();
                charBmp.Dispose();
            }

            return fontset;
        }

        private static Dictionary<string, Image> LoadTextures()
        {
            SetDir(@"/resources/textures");

            Debug.WriteLine("Loading Textures");
            var textures = new Dictionary<string, Image>();
            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory());
            foreach (string f in files)
            {
                Image texture = Image.FromFile(f);
                string label = f.Substring(f.LastIndexOf('\\') + 1).Split('.')[0];
                Debug.WriteLine(label);
                textures.Add(label, texture);
            }

            return textures;
        }

        public static RenderObject ImportGLTF(string file, OpenGL_Shader shader)
        {
            SetDir(@"/resources/models");
            var importer = new GLTF_Importer(file);
            var obj = importer.CreateGLTFRenderObject();
            obj.Geometry_Shader = shader;
            return obj;
        }

        private static void SetDir(string name)
        {
            for (int i = 0; i < 10 && !Directory.GetCurrentDirectory().EndsWith("GuildLeader"); i++)
            {
                Directory.SetCurrentDirectory("..");
            }
            Directory.SetCurrentDirectory("." + name);
            //Debug.WriteLine("Setting Directory");
            //Debug.WriteLine(Directory.GetCurrentDirectory());
        }
    }
}
