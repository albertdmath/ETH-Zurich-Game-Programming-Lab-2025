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
        public Coconut(ProjectileType type, Vector3 origin, Vector3 target, DrawModel model, float scaling) : base(type, origin, target, model, scaling) {}

        protected override void Move(float dt)
        {
            Position += Velocity * Orientation * dt;
        }

        public override bool Hit()
        {
            bool isHit = base.Hit();
            
            //if intersects, update
            if (isHit)
            {
                _bounces--;
                // Return true for deletion if bounces are expended
                if (_bounces <= 0)
                {
                    return true;
                } else { // Otherwise bounce the coconut
                    Velocity *= 0.9f;
                    // Bounce effect, maybe change it depending on the surface hit
                    Orientation = new Vector3(-Orientation.X, Orientation.Y, -Orientation.Z);
                }
            }
            return false;
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