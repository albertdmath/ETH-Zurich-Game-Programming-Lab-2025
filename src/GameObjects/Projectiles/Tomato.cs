using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace src.GameObjects
{
    public class Tomato : Projectile
    {
        // Private fields:
        private static readonly float angle = (float)Math.PI / 3; // angle of throw
        private static readonly float cos = (float)Math.Cos(angle), sin = (float)Math.Sin(angle);
        private static readonly float HALF_GRAVITY = 4.9f; // Gravity effect (adjust as needed)
        private float timeAlive = 0f;
        private Vector3 origin;

        // Constructor:
        public Tomato(ProjectileType type, Vector3 origin, Vector3 target, Model model) : base(type, origin, target, model)
        {
            Velocity = CalculateVelocity(origin, target);
            this.origin = origin;
        }

        private float CalculateVelocity(Vector3 origin, Vector3 target)
        {
            // Calculate the horizontal distance (XZ-plane)
            float distance = Vector3.Distance(target, origin);

            // Calculate the initial velocity using the simplified formula
            return (float)Math.Sqrt((HALF_GRAVITY * distance) / (cos * sin));
        }

        public override void Move(float dt)
        {
            timeAlive += dt;
            
            Vector3 horizontalMotion = Orientation * Velocity * cos;
            Vector3 verticalMotion = new Vector3(0, Velocity * sin - HALF_GRAVITY * timeAlive, 0);

            Position = origin + (horizontalMotion + verticalMotion) * timeAlive;
        }

        public override void Throw(float chargeUp)
        {
            this.Position = Holder.Position + Holder.Orientation;
            this.Orientation = Holder.Orientation;
            this.Holder = null;
            Velocity = chargeUp;
            timeAlive = 0f;
        }
    }
}