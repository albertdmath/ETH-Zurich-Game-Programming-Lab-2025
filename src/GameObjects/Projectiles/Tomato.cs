using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace src.GameObjects
{
    public class Tomato : Projectile
    {
        // Private fields:
        private new const float velocity = 1.1f; // This will be overridden by calculated velocities
        private const float FLIGHT_TIME = 2f; // Total time to reach the target
        private const float GRAVITY = 9.8f; // Gravity constant
        private float timeAlive = 0f;
        private Vector3 target;
        private Vector3 initialVelocity; // Calculated initial velocity

        // Constructor:
        public Tomato(ProjectileType type, Vector3 origin, Vector3 target) : base(type, origin, target)
        {
            this.target = target;
            CalculateInitialVelocity(origin, target);
        }

        // Calculate the initial velocity required to reach the target in FLIGHT_TIME seconds
        private void CalculateInitialVelocity(Vector3 origin, Vector3 target)
        {
            // Calculate the horizontal distance (ignore Y-axis for horizontal motion)
            Vector3 horizontalDelta = new Vector3(target.X - origin.X, 0, target.Z - origin.Z);
            float horizontalDistance = horizontalDelta.Length();

            // Calculate the horizontal velocity (constant)
            float horizontalVelocity = horizontalDistance / FLIGHT_TIME;

            // Calculate the vertical velocity (accounts for gravity)
            float verticalVelocity = (target.Y - origin.Y) / FLIGHT_TIME + 0.5f * GRAVITY * FLIGHT_TIME;

            // Combine horizontal and vertical velocities
            Vector3 horizontalDirection = Vector3.Normalize(horizontalDelta);
            initialVelocity = horizontalDirection * horizontalVelocity + Vector3.UnitY * verticalVelocity;
        }

        public override void Move(float dt)
        {
            if ((timeAlive += dt) > FLIGHT_TIME)
            {
                // Ensure the projectile stops at the target
                Position = target;
                return;
            }

            // Update position based on initial velocity and gravity
            Position += initialVelocity * dt;
            initialVelocity += Vector3.UnitY * (-GRAVITY * dt); // Apply gravity to vertical velocity
        }
    }
}