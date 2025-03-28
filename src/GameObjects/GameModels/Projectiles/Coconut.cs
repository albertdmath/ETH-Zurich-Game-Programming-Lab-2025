using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace src.GameObjects
{
    public class Coconut : Projectile
    {
        // Private fields:
        private const float MAX_VELOCITY = 15;
        private const int MAX_BOUNCES = 3;
        private int _bounces;

        // Constructor:
        public Coconut(ProjectileType type, Vector3 origin, Vector3 target, Model model, float scaling) : base(type, origin, target, model, scaling) {}

        public override void Move(float dt)
        {
            Position += Velocity * Orientation * dt;
        }

        public override void Throw(float chargeUp)
        {
            base.Throw(chargeUp);
            Velocity = Math.Min(Velocity + chargeUp, MAX_VELOCITY);
            _bounces = MAX_BOUNCES;
        }

        public override void Throw(Vector3 origin, Vector3 target) 
        {
            base.Throw(origin, target);
            Velocity = 3f;
            _bounces = MAX_BOUNCES;
        }

        public override void Hit()
        {
            base.Hit();
            
            // Check intersection with players
            bool hit = false;
            foreach (Player player in Player.active.Where(p => p.Life > 0))
            {
                if (this.Hitbox.Intersects(player.Hitbox))
                {
                    player.Life--;
                    hit = true;
                }
            }

            //if intersects, update
            if (hit)
            {
                _bounces--;
                if (_bounces <= 0)
                {
                    active.Remove(this);
                }
                else
                {
                    Velocity *= 0.9f;
                    Position += Velocity * Orientation; // Adjust position after bounce
                }
            }
            
        }
    }
}