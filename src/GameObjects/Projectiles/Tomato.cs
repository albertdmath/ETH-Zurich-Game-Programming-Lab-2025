using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace src.GameObjects
{
    public class Tomato : Projectile
    {
        // Private fields:
        private new const float velocity = 1.1f;
        private const float FLIGHT_TIME = 2f;
        private float timeAlive = 0f;

        // Constructor:
        public Tomato(ProjectileType type, Vector3 origin, Vector3 target,Model model) : base(type, origin, target, model) { }

        public override void Move(float dt)
        {
            if ((timeAlive += dt) > FLIGHT_TIME) return;

            Position += velocity * Orientation * dt;

            float timeProgress = timeAlive / FLIGHT_TIME;

            float A = 5f;

            Position = new Vector3(Position.X, A * timeProgress * (1 - timeProgress), Position.Z);
        }
    }
}
