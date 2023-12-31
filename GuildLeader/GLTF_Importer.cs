﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GuildLeader.resources
{
    public class GLTF_Importer
    {
        public class GLTF_Fields
        {
            public Dictionary<string, string>? asset;
            public int scene;
            public List<Scene>? scenes;
            public List<Node>? nodes;
            public List<Material>? materials;
            public List<Image>? images;
            public List<Texture>? textures;
            public List<Mesh>? meshes;
            public List<Accessor>? accessors;
            public List<BufferView>? bufferViews;
            public List<Sampler>? samplers;
            public List<Buffer>? buffers;
        }

        public class Asset
        {
            string? generator;
            string? version;
        }

        public class Scene
        {
            public string? name;
            public List<int>? nodes;
        }

        public class Node
        {
            public string? name;
            public int mesh;
            public List<float> translation;
            public List<float> rotation;
        }

        public class Material
        {
            public PBRMetallicRoughness? pbrMetallicRoughness;
            public string? name;
            public bool doubleSided;
        }

        public class PBRMetallicRoughness
        {
            public class BaseColorTexture
            {
                public int index;
                public int texCoord;
            }

            public List<float>? baseColorFactor;
            public BaseColorTexture? baseColorTexture;
            public float metallicFactor;
            public float roughnessFactor;
        }

        public class Image
        {
            public int bufferView;
            public string? mimeType;
            public string? name;
        }

        public class Texture
        {
            public int source;
            public int sampler;
        }

        public class Mesh
        {
            public class Primitive
            {
                public class Attribute
                {
                    public int POSITION;
                    public int NORMAL;
                    public int TEXCOORD_0;
                }
                public Attribute? attributes;
                public int indices;
                public int mode;
                public int material;
            }

            public string? name;
            public List<Primitive>? primitives;
        }

        public class Accessor
        {
            public int bufferView;
            public int componentType;
            public int count;
            public string? type;
            public int byteOffset;
            public List<float>? min;
            public List<float>? max;
        }

        public class BufferView
        {
            public int buffer;
            public int byteOffset;
            public int byteLength;
            public int target;
            public int byteStride;
        }

        public class Sampler
        {
            int magFilter;
            int minFilter;
        }

        public class Buffer
        {
            public string? uri;
            public int byteLength;
        }

        private GLTF_Fields ModelData = new GLTF_Fields();

        public GLTF_Importer(string path)
        {
            Stopwatch sw = new Stopwatch();
            using StreamReader streamReader = File.OpenText(path);
            sw.Start();
            ModelData = JsonConvert.DeserializeObject<GLTF_Fields>(streamReader.ReadToEnd());
            sw.Stop();
        }

        public RenderObject CreateGLTFRenderObject()
        {
            var polygons = new List<RenderObject.Polygon>();

            foreach (Mesh m in ModelData.meshes)
            {
                foreach (Mesh.Primitive p in m.primitives)
                {
                    var poly = new RenderObject.Polygon();
                    var vertexData = new List<float>();
                    var vertices = (List<float>)GetBufferData(p.attributes.POSITION);
                    var normals = (List<float>)GetBufferData(p.attributes.NORMAL);
                    var colors = new List<float>() { 1f, 1f, 1f, 1f };
                    PBRMetallicRoughness pbr = null;
                    if (ModelData.materials != null)
                    {
                        pbr = ModelData.materials[p.material].pbrMetallicRoughness;
                        poly.Metal = pbr.metallicFactor;
                        poly.Rough = pbr.roughnessFactor;
                        if (pbr.baseColorFactor != null)
                        {
                            colors = pbr.baseColorFactor;
                        }
                    }

                    List<float> texCoords = new List<float>();
                    if (p.attributes.TEXCOORD_0 != 0)
                    {
                        texCoords = (List<float>)GetBufferData(p.attributes.TEXCOORD_0);
                        if (pbr.baseColorTexture != null)
                        {
                            byte[] imageData = GetBufferImage(pbr.baseColorTexture.index);
                            using MemoryStream ms = new MemoryStream(imageData);
                            var bmp = new Bitmap(ms);
                            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                            poly.ImageData = new byte[bmp.Width * bmp.Height * 4];
                            Marshal.Copy(bmpData.Scan0, poly.ImageData, 0, poly.ImageData.Length);
                            bmp.UnlockBits(bmpData);
                            poly.ImageSize = bmp.Size;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < vertices.Count; i++)
                        {
                            texCoords.Add(0);
                        }
                    }
                    List<int> indices = new List<int>();
                    foreach (var n in GetBufferData(p.indices))
                    {
                        indices.Add(Convert.ToInt32(n));
                    }
                    vertexData.AddRange(OrderByIndices(vertices, normals, colors, texCoords, indices));
                    poly.VertexData = vertexData;

                    polygons.Add(poly);
                }
            }
            polygons.Sort((a, b) => b.VertexData[9].CompareTo(a.VertexData[9]));
            return new RenderObject() { Polygons = polygons };
        }

        public byte[] GetBufferImage(int bufferViewID)
        {
            BufferView bf = ModelData.bufferViews[ModelData.images[bufferViewID].bufferView];
            int bufferID = bf.buffer;
            string uri = ModelData.buffers[bufferID].uri;
            uri = uri.Substring(uri.LastIndexOf(',') + 1);
            byte[] data = Convert.FromBase64String(uri);
            List<byte> img = new List<byte>();
            for (int i = bf.byteOffset; i < bf.byteOffset + bf.byteLength; i++)
            {
                img.Add(data[i]);
            }
            return img.ToArray();
        }

        private IList GetBufferData(int accessorID)
        {
            int bufferViewID = ModelData.accessors[accessorID].bufferView;
            int componentType = ModelData.accessors[accessorID].componentType;
            string dataType = ModelData.accessors[accessorID].type;
            IList bufferData;
            int dataSize;
            int typeSize;

            switch (componentType)
            {
                case 5120:
                    bufferData = new List<sbyte>();
                    dataSize = sizeof(byte);
                    break;
                case 5121:
                    bufferData = new List<byte>();
                    dataSize = sizeof(sbyte);
                    break;
                case 5122:
                    bufferData = new List<short>();
                    dataSize = sizeof(short);
                    break;
                case 5123:
                    bufferData = new List<ushort>();
                    dataSize = sizeof(ushort);
                    break;
                case 5125:
                    bufferData = new List<uint>();
                    dataSize = sizeof(uint);
                    break;
                default:
                    bufferData = new List<float>();
                    dataSize = sizeof(float);
                    break;
            }

            switch (dataType)
            {
                case "SCALAR":
                    typeSize = 1;
                    break;
                case "VEC2":
                    typeSize = 2;
                    break;
                default:
                    typeSize = 3;
                    break;
            }

            BufferView bf = ModelData.bufferViews[bufferViewID];
            int bufferID = bf.buffer;
            string uri = ModelData.buffers[bufferID].uri;
            uri = uri.Substring(uri.LastIndexOf(',') + 1);
            byte[] data = Convert.FromBase64String(uri);

            int index = bf.byteOffset + ModelData.accessors[accessorID].byteOffset;
            int target = index + (ModelData.accessors[accessorID].count * dataSize * typeSize);
            for (; index < target; index += dataSize)
            {
                switch (componentType)
                {
                    case 5120:
                        bufferData.Add(data[index]);
                        break;
                    case 5121:
                        bufferData.Add(data[index]);
                        break;
                    case 5122:
                        bufferData.Add(BitConverter.ToInt16(data, index));
                        break;
                    case 5123:
                        bufferData.Add(BitConverter.ToUInt16(data, index));
                        break;
                    case 5125:
                        bufferData.Add(BitConverter.ToInt32(data, index));
                        break;
                    default:
                        bufferData.Add(BitConverter.ToSingle(data, index));
                        break;
                }
            }

            return bufferData;
        }

        private List<float> OrderByIndices(List<float> v, List<float> n, List<float> c, List<float> t, List<int> indices)
        {
            List<float> orderedData = new List<float>();

            for (int i = 0; i < indices.Count; i++)
            {
                orderedData.AddRange(v.GetRange(indices[i] * 3, 3));
                orderedData.AddRange(n.GetRange(indices[i] * 3, 3));
                orderedData.AddRange(c);
                orderedData.AddRange(t.GetRange(indices[i] * 2, 2));
            }

            return orderedData;
        }
    }
}
