using System;
using GameLab;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace src.GameObjects
{
    public class Frog : Projectile
    {
        // Private fields:
        private const float HOP_TIME = 1.0f; // 1 seconds for the hop time
        private new const float velocity = 0.5f;
        private float timeBeforeHop = 0f;
        // Constructor:
        public Frog(ProjectileType type, Vector3 origin, Vector3 target,Model model) : base(type, origin, target, model) { }

        public override void Move(float dt)
        {
            if ((timeBeforeHop += dt) < HOP_TIME) return;

            // After HOP_TIME seconds, move the frog
            float jumpProgress = (timeBeforeHop - HOP_TIME) / HOP_TIME;
            
            // Orient the frog towards the player
            Orientation = Vector3.Normalize(GameLabGame.players[0].Position - Position);
            Orientation = new Vector3(Orientation.X, 0, Orientation.Z);
            Orientation.Normalize();
            
            // Update the position of the frog:
            Position += velocity * Orientation * dt;
            Position = new Vector3(Position.X, 0.07f + (float)Math.Sin(jumpProgress * Math.PI), Position.Z);
            
            // Reset the time before the next hop:
            if (timeBeforeHop > 2 * HOP_TIME) timeBeforeHop = 0f;
        }
    }
}
