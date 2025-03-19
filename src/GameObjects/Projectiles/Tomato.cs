using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace src.GameObjects
{
    public class Tomato : Projectile
    {
        // Private fields:
        private new const float velocity = 1.1f;
        private const float FLIGHT_TIME = 2f; // Total time to reach the target
        private float timeAlive = 0f;
        private Vector3 origin; // Starting position
        private Vector3 target; // Target position
        private const float A = 5f; // Height factor for the parabola

        // Constructor:
        public Tomato(ProjectileType type, Vector3 origin, Vector3 target,Model model) : base(type, origin, target, model)
        {
            this.origin = origin;
            this.target = target;
        }

        public override void Move(float dt)
        {
            // Stop moving if the flight time is over
            if ((timeAlive += dt) > FLIGHT_TIME) return;

            // Calculate the progress (normalized time between 0 and 1)
            float timeProgress = timeAlive / FLIGHT_TIME;

            // Horizontal motion: Linear interpolation between origin and target
            Vector3 horizontalPosition = Vector3.Lerp(origin, target, timeProgress);

            // Vertical motion: Parabolic trajectory
            float verticalOffset = A * timeProgress * (1 - timeProgress);

            // Update the tomato's position
            Position = new Vector3(horizontalPosition.X, verticalOffset, horizontalPosition.Z);
        }
    }
}