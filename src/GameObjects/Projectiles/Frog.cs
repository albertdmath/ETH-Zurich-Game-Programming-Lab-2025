using System;
using Microsoft.Xna.Framework;

namespace src.GameObjects
{
    public class Frog : Projectile
    {
        // Private fields:
        private const float HOP_TIME = 1.5f; // 1 second for the hop time
        private new const float velocity = 1.1f;
        private float timeBeforeHop = 0f;

        // Constructor:
        public Frog(ProjectileType type, Vector3 origin, Vector3 target) : base(type, origin, target) { }

        public override void Move(float dt, Vector3 playerPosition)
        {
            if ((timeBeforeHop += dt) < HOP_TIME) return;

            // After HOP_TIME seconds, move the frog
            float jumpProgress = (timeBeforeHop - HOP_TIME) / HOP_TIME;

            // Parabolic arc for Y position
            //Position = new Vector3(Position.X, 0, Position.Z); // Y = sin(t * pi), from 0 to 1 and back to 0
            Orientation = Vector3.Normalize(playerPosition - Position);
            Orientation = new Vector3(Orientation.X, 0, Orientation.Z);
            Orientation.Normalize();
            Position += velocity * Orientation * dt;
            Position = new Vector3(Position.X, (float)Math.Sin(jumpProgress * Math.PI), Position.Z);
      

            if (timeBeforeHop > 2 * HOP_TIME)
                timeBeforeHop = 0f;

        }
    }
}
