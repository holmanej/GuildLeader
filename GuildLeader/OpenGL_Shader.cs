﻿using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuildLeader
{
    public class OpenGL_Shader
    {
        private int Handle;

        public OpenGL_Shader(string vertexPath, string fragmentPath)
        {
            string VertexShaderSource;

            using (StreamReader reader = new StreamReader(vertexPath, Encoding.UTF8))
            {
                VertexShaderSource = reader.ReadToEnd();
            }

            string FragmentShaderSource;

            using (StreamReader reader = new StreamReader(fragmentPath, Encoding.UTF8))
            {
                FragmentShaderSource = reader.ReadToEnd();
            }

            int VertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VertexShader, VertexShaderSource);

            int FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShader, FragmentShaderSource);

            GL.CompileShader(VertexShader);

            string infoLogVert = GL.GetShaderInfoLog(VertexShader);
            if (infoLogVert != string.Empty)
            {
                Debug.WriteLine(infoLogVert);
            }

            GL.CompileShader(FragmentShader);

            string infoLogFrag = GL.GetShaderInfoLog(FragmentShader);
            if (infoLogFrag != string.Empty)
            {
                Debug.WriteLine(infoLogFrag);
            }

            Handle = GL.CreateProgram();

            GL.AttachShader(Handle, VertexShader);
            GL.AttachShader(Handle, FragmentShader);

            GL.LinkProgram(Handle);

            GL.DetachShader(Handle, VertexShader);
            GL.DetachShader(Handle, FragmentShader);
            GL.DeleteShader(FragmentShader);
            GL.DeleteShader(VertexShader);


            // First, we have to get the number of active uniforms in the shader.
            GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);

            // Loop over all the uniforms,
            for (var i = 0; i < numberOfUniforms; i++)
            {
                // get the name of this uniform,
                //Debug.WriteLine(GL.GetActiveUniform(Handle, i, out _, out _) + GL.GetUniformLocation(Handle, GL.GetActiveUniform(Handle, i, out _, out _)));
            }
        }

        public void Use()
        {
            GL.UseProgram(Handle);
        }

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                GL.DeleteProgram(Handle);

                disposedValue = true;
            }
        }

        ~OpenGL_Shader()
        {
            GL.DeleteProgram(Handle);
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void SetInt(string name, int data)
        {
            GL.UseProgram(Handle);
            GL.Uniform1(GL.GetUniformLocation(Handle, name), 1, ref data);
        }

        public void SetFloat(string name, float data)
        {
            GL.UseProgram(Handle);
            GL.Uniform1(GL.GetUniformLocation(Handle, name), 1, ref data);
        }

        public void SetVector3(string name, Vector3 data)
        {
            GL.UseProgram(Handle);
            GL.Uniform3(GL.GetUniformLocation(Handle, name), data);
        }

        public void SetMatrix4(string name, Matrix4 data)
        {
            GL.UseProgram(Handle);
            GL.UniformMatrix4(GL.GetUniformLocation(Handle, name), true, ref data);
        }

        public void SetTexture(string name, int unit)
        {
            GL.UseProgram(Handle);
            GL.Uniform1(GL.GetUniformLocation(Handle, name), unit);
        }

        public void SetTransform(string name, Matrix4 trans, Matrix4 scale, Matrix4 rotat)
        {
            GL.UseProgram(Handle);
            GL.UniformMatrix4(GL.GetUniformLocation(Handle, name + ".Translation"), true, ref trans);
            GL.UniformMatrix4(GL.GetUniformLocation(Handle, name + ".Scale"), true, ref scale);
            GL.UniformMatrix4(GL.GetUniformLocation(Handle, name + ".Rotation"), true, ref rotat);
        }

        public void SetMaterial(string name, Vector3 ambi, Vector3 diff, Vector3 spec, float shiny)
        {
            GL.UseProgram(Handle);
            GL.Uniform3(GL.GetUniformLocation(Handle, name + ".AmbientFactor"), ambi);
            GL.Uniform3(GL.GetUniformLocation(Handle, name + ".DiffuseFactor"), diff);
            GL.Uniform3(GL.GetUniformLocation(Handle, name + ".SpecularFactor"), spec);
            GL.Uniform1(GL.GetUniformLocation(Handle, name + ".ShinyFactor"), 1, ref shiny);
        }
    }
}
