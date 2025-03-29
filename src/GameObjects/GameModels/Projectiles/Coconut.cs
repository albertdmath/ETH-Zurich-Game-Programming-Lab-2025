using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using System.Collections.Generic;

namespace src.GameObjects
{
    public class Coconut : Projectile
    {
        // Private fields:
        private const float MAX_VELOCITY = 15;
        private const int MAX_BOUNCES = 3;
        // this is done to avoid the player to be hit multiple times
        private int _bounces;

        // Constructor:
        public Coconut(ProjectileType type, Vector3 origin, Vector3 target, Model model, float scaling) : base(type, origin, target, model, scaling) {}

        protected override void Move(float dt)
        {
            Position += Velocity * Orientation * dt;
        }

        protected override void Hit()
        {
            base.Hit();
            
            // Check intersection with players
            bool hit = false;
            foreach (Player player in Player.active.Where(p => p.Life > 0))
            {   
                if (Hitbox.Intersects(player.Hitbox))
                    hit = player.GetHit(this);
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
                    // Bounce effect, maybe change it depending on the surface hit
                    Orientation = new Vector3(-Orientation.X, Orientation.Y, -Orientation.Z); 
                }
            }
            
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
    }
}