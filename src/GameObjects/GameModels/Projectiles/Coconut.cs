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
        private Random random = new Random();
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
                } 
                else 
                { // Otherwise bounce the coconut
                    Velocity *= 0.9f; // Reduce speed after bounce

                    // Generate a random angle between -30° and +30°
                    float randomAngle = MathHelper.ToRadians(random.Next(130, 230));

                    // Create a rotation matrix around the Y-axis
                    Matrix rotationMatrix = Matrix.CreateRotationY(randomAngle);

                    // Apply the rotation to the orientation vector
                    Orientation = Vector3.Transform(Orientation, rotationMatrix);
                    Orientation = Vector3.Normalize(Orientation); // Normalize to maintain direction
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