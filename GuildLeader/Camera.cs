using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuildLeader
{
    public class Camera
    {
        public string Name = "cam";
        public bool Active;
        public Vector3 Position;
        public Vector3 Rotation;
        public float TranslationSpeed;
        public float VerticalSpeed;
        public float RotationSpeed;

        public Matrix4 ViewTranslation;
        public Matrix4 ViewScale;
        public Matrix4 ViewRotation;
        public Matrix4 Projection;

        public void UpdateTransform()
        {
            ViewTranslation = Matrix4.CreateTranslation(-Position.X, -Position.Y, -Position.Z);
            ViewScale = Matrix4.CreateScale(1f, 1f, 1f);
            ViewRotation = Matrix4.CreateRotationY(Rotation.Y * 3.14f / 180) * Matrix4.CreateRotationX(-Rotation.X * 3.14f / 180);
        }

        public void ReadInputs(KeyboardState kb, MouseState mouse, double deltaTime)
        {
            float dsin = (float)Math.Sin(Rotation.Y * 3.14f / 180) * TranslationSpeed;
            float dcos = (float)Math.Cos(Rotation.Y * 3.14f / 180) * TranslationSpeed;
            float xMove = 0;
            float yMove = 0;
            float zMove = 0;

            if (kb.IsKeyDown(Keys.W))
            {
                xMove += dsin;
                zMove += -dcos;
            }
            else if (kb.IsKeyDown(Keys.S))
            {
                xMove += -dsin;
                zMove += dcos;
            }
            if (kb.IsKeyDown(Keys.A))
            {
                xMove += -dcos;
                zMove += -dsin;
            }
            else if (kb.IsKeyDown(Keys.D))
            {
                xMove += dcos;
                zMove += dsin;
            }

            if (kb.IsKeyDown(Keys.Space))
            {
                yMove += VerticalSpeed;
            }
            else if (kb.IsKeyDown(Keys.LeftControl))
            {
                yMove -= VerticalSpeed;
            }

            if (kb.IsKeyDown(Keys.E))
            {
                Rotation.Y += (float)(RotationSpeed * deltaTime);
                Rotation.Y = Rotation.Y >= 360 ? Rotation.Y - 360 : Rotation.Y;
            }
            else if (kb.IsKeyDown(Keys.Q))
            {
                Rotation.Y -= (float)(RotationSpeed * deltaTime);
                Rotation.Y = Rotation.Y < 0 ? Rotation.Y + 360 : Rotation.Y;
            }
            if (kb.IsKeyDown(Keys.R))
            {
                Rotation.X += (float)(RotationSpeed * deltaTime);
                Rotation.X = Rotation.X >= 360 ? Rotation.X - 360 : Rotation.X;
            }
            else if (kb.IsKeyDown(Keys.F))
            {
                Rotation.X -= (float)(RotationSpeed * deltaTime);
                Rotation.X = Rotation.X < 0 ? Rotation.X + 360 : Rotation.X;
            }

            Position.X += (float)(xMove * deltaTime);
            Position.Y += (float)(yMove * deltaTime);
            Position.Z += (float)(zMove * deltaTime);
        }
    }
}
