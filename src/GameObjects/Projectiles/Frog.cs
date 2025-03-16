using System;
using Microsoft.Xna.Framework;

namespace src.GameObjects
{
    public class Frog : Projectile
    {
        // Private fields:
        private const float HOP_TIME = 1f; // 1 second for the hop time
        private const float velocity = 1.7f;
        private float timeBeforeHop = 0f;

        // Constructor:
        public Frog(int type, Vector3 origin, Vector3 target) : base(type, origin, target) { }

        public override void Move(float dt)
        {
            if ((timeBeforeHop += dt) < HOP_TIME) return;

            // After HOP_TIME seconds, move the frog
            float jumpProgress = (timeBeforeHop - HOP_TIME) / HOP_TIME;

            // Parabolic arc for Y position
            this.position.Y = (float)Math.Sin(jumpProgress * Math.PI); // Y = sin(t * pi), from 0 to 1 and back to 0
            this.position += velocity * orientation * dt;

            if (timeBeforeHop > 2 * HOP_TIME)
                timeBeforeHop = 0f;

        }
    }
}
