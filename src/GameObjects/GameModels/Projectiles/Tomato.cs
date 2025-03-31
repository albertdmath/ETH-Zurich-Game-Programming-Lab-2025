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
        private const float SQUARED_EXPLOSION_RADIUS = 0.8f; // Define the explosion radius
        private float timeAlive = 0f;
        private Vector3 origin;

        // Constructor:
        public Tomato(ProjectileType type, Vector3 origin, Vector3 target,DrawModel model, float scaling) : base(type, origin, target, model, scaling) {}

        private float CalculateVelocity(Vector3 origin, Vector3 target)
        {
            // Calculate the horizontal distance (XZ-plane)
            float distance = Vector3.Distance(target, origin);

            // Calculate the initial velocity using the simplified formula
            return (float)Math.Sqrt((HALF_GRAVITY * distance) / (cos * sin));
        }

        protected override void Move(float dt)
        {
            timeAlive += dt;
            
            Vector3 horizontalMotion = Orientation * Velocity * cos;
            Vector3 verticalMotion = new Vector3(0, Velocity * sin - HALF_GRAVITY * timeAlive, 0);

            Position = origin + (horizontalMotion + verticalMotion) * timeAlive;
        }

        protected override void Hit()
        {
            base.Hit();

            // Check intersection with players
            bool hit = false;
            foreach (Player player in Player.active.Where(p => p.Life > 0))
            {
                if(Hitbox.Intersects(player.Hitbox))
                    hit = player.GetHit(this);  
            
            }

            // Check intersection with ground
            if (Position.Y < 0f)
                hit = true; 
            
            //if intersects, update
            if(hit)
            {
                Explode();
                MusicAndSoundEffects.playProjectileSFX(ProjectileType.Tomato);
                active.Remove(this);
            }
        }    

        private void Explode()
        {
            foreach (Player player in Player.active.Where(p => p.Life > 0))
            {
                if (Vector3.DistanceSquared(this.Position, player.Position) <= SQUARED_EXPLOSION_RADIUS)
                    player.GetHit(this);
            }
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
    }
}