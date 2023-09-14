using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuildLeader
{
    internal class Camera
    {
        public string Name = "cam";
        public bool Active;
        public Vector3 Position;
        public Vector3 Rotation;
        public float TranslationSpeed;
        public float RotationSpeed;

        public void ReadInputs(KeyboardState kb, MouseState mouse)
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
                yMove += TranslationSpeed;
            }
            else if (kb.IsKeyDown(Keys.LeftControl))
            {
                yMove -= TranslationSpeed;
            }

            if (kb.IsKeyDown(Keys.E))
            {
                Rotation.Y += RotationSpeed;
                Rotation.Y = Rotation.Y >= 360 ? Rotation.Y - 360 : Rotation.Y;
            }
            else if (kb.IsKeyDown(Keys.Q))
            {
                Rotation.Y -= RotationSpeed;
                Rotation.Y = Rotation.Y < 0 ? Rotation.Y + 360 : Rotation.Y;
            }
            if (kb.IsKeyDown(Keys.R))
            {
                Rotation.X += RotationSpeed;
                Rotation.X = Rotation.X >= 360 ? Rotation.X - 360 : Rotation.X;
            }
            else if (kb.IsKeyDown(Keys.F))
            {
                Rotation.X -= RotationSpeed;
                Rotation.X = Rotation.X < 0 ? Rotation.X + 360 : Rotation.X;
            }

            Position.X += xMove;
            Position.Y += yMove;
            Position.Z += zMove;
        }
    }
}
