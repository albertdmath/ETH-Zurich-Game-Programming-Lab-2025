using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace src.GameObjects
{
    public class Tomato : Projectile
    {
        // Private fields:
        private static readonly float angle = (float)Math.PI / 3; // angle of throw
        private static readonly float cos = (float)Math.Cos(angle), sin = (float)Math.Sin(angle);
        private static readonly float HALF_GRAVITY = 4.9f; // Gravity effect
        private float timeAlive = 0f;
        private Vector3 origin;

        // Constructor:
        public Tomato(ProjectileType type, Vector3 origin, Vector3 target,Model model, float scaling) : base(type, origin, target, model, scaling) {}

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
            base.Throw(chargeUp);
            Throw(Position - Orientation, Position+chargeUp*Orientation);
        }

        public override void Throw(Vector3 origin, Vector3 target) 
        {
            base.Throw(origin, target);
            Velocity = CalculateVelocity(origin, target);
            this.origin = origin;
            timeAlive = 0f;
        }

        public override void Hit()
        {
            base.Hit();
            
            // Check intersection with players
            bool hit = false;
            foreach (Player player in Player.active.Where(p => p.Life > 0))
            {
                if(this.Hitbox.Intersects(player.Hitbox))
                {
                    hit = true;
                    //this breaks, because the losing life is done in the exploding logic
                    break;  
                }
            }

            // Check intersection with ground
            if (Position.Y < 0)
                hit = true; 
            

            //if intersects, update
            if(hit)
            {
                Explode();
                active.Remove(this);
            }
        }    

        private void Explode()
        {
            float explosionRadius = 5f; // Define the explosion radius

            foreach (Player player in Player.active.Where(p => p.Life > 0))
            {
                float distance = Vector3.Distance(this.Position, player.Position);

                if (distance <= explosionRadius)
                    player.Life--; // Reduce life if within explosion radius
                
            }
        }
    }
}