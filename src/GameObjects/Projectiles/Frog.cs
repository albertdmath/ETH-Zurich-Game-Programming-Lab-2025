using System;
using Microsoft.Xna.Framework;
using System.Linq;

namespace src.GameObjects
{
    public class Frog : Projectile
    {
        // Private fields:
        private const float HOP_TIME = 1.0f; // 1 second hop time
        private new const float velocity = 0.7f;
        private float timeBeforeHop = 0f;
        private const float ROTATION_SPEED = 3.0f; // Adjust for smoother turning

        // Constructor:
        public Frog(ProjectileType type, Vector3 origin, Vector3 target) : base(type, origin, target) { }

        public override void Move(float dt)
        {
            if ((timeBeforeHop += dt) < HOP_TIME)
            {
                Player nearestPlayer = Player.active
                    .OrderBy(player => Vector3.Distance(Position, player.Position))
                    .First();

                // Desired direction toward the player
                Vector3 targetDirection = Vector3.Normalize(nearestPlayer.Position - Position);

                // Smoothly interpolate (lerp) towards the target direction
                Orientation = Vector3.Lerp(Orientation, targetDirection, ROTATION_SPEED * dt);
                Orientation.Normalize(); // Ensure it's a unit vector
            }
            else
            {
                // After HOP_TIME seconds, move the frog
                float jumpProgress = (timeBeforeHop - HOP_TIME) / HOP_TIME;

                // Update the position of the frog:
                Position += velocity * Orientation * dt;
                Position = new Vector3(Position.X, (float)Math.Sin(jumpProgress * Math.PI), Position.Z);
            }

            // Reset the time before the next hop:
            if (timeBeforeHop > 2 * HOP_TIME) timeBeforeHop = 0f;
        }
    }
}
