using System;
using GameLab;
using Microsoft.Xna.Framework;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace src.GameObjects
{
    public class Frog : Projectile
    {
        // Constants
        private const float HOP_TIME = 1.0f; // 1 second hop time
        private const float ROTATION_SPEED = 3.0f; // Adjust for smoother turning
        private const float THROW_ANGLE = (float)Math.PI / 3; // Angle of throw
        private const float HALF_GRAVITY = 4.9f; // Gravity effect (adjust as needed)
        // Fields
        private float timeAlive = 0f;
        private bool beingThrown = false;
        private Vector3 origin;

        // Constructor:
        public Frog(ProjectileType type, Vector3 origin, Vector3 target,Model model, float scaling) : base(type, origin, target, model, scaling)
        {
            Throw(origin,target);
        }

        public override void Move(float dt)
        {
            timeAlive += dt;

            if (beingThrown)
                ThrownMove();
            else
                HopMove(dt);
            
        }

        private void ThrownMove()
        {
            // Calculate horizontal and vertical motion
            Vector3 horizontalMotion = Orientation * Velocity * (float)Math.Cos(THROW_ANGLE);
            Vector3 verticalMotion = new Vector3(0, Velocity * (float)Math.Sin(THROW_ANGLE) - HALF_GRAVITY * timeAlive, 0);

            // Update position
            Position = origin + (horizontalMotion + verticalMotion) * timeAlive;

            // Check if the frog has landed
            if (Position.Y < 0)
            {
                beingThrown = false;
                timeAlive = 0f;
                Velocity = 0.7f;
                Position = new Vector3(Position.X, 0, Position.Z);
            }
        }

        private void HopMove(float dt)
        {
            if (timeAlive < HOP_TIME)
                TurnToPlayer(dt);
            else
                Hop(dt);

            // Reset the time before the next hop
            if (timeAlive > 2 * HOP_TIME)
                timeAlive = 0f;
        }

        private void TurnToPlayer(float dt)
        {
            Player nearestPlayer = Player.active
                .OrderBy(player => (player.Life>0)?(Vector3.Distance(Position, player.Position)):1000)
                .First();

            // Desired direction toward the player
            Vector3 targetDirection = Vector3.Normalize(nearestPlayer.Position - Position);

            // Smoothly interpolate (lerp) towards the target direction
            Orientation = Vector3.Lerp(Orientation, targetDirection, ROTATION_SPEED * dt);
            Orientation.Normalize(); // Ensure it's a unit vector
        }

        private void Hop(float dt)
        {
            float jumpProgress = (timeAlive - HOP_TIME) / HOP_TIME;

            // Update the position of the frog
            Position += Velocity * Orientation * dt;
            Position = new Vector3(Position.X, (float)Math.Sin(jumpProgress * Math.PI), Position.Z);
        }

        public override void Throw(float chargeUp)
        {
            base.Throw(chargeUp);
            beingThrown = true;
            Velocity = chargeUp*0.1f;
            timeAlive = 0f;
        }
        public override void Throw(Vector3 origin, Vector3 target) {
            base.Throw(origin,target);
            Velocity = 0.7f;
            this.origin = origin;
            timeAlive = 0f;
        }
    }
}