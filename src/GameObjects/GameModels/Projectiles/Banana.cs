using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace src.GameObjects
{
    public class Banana : Projectile
    {
        // Private fields:
        private bool onGround = false;
        private static readonly float angle = (float)Math.PI / 3; // angle of throw
        private static readonly float cos = (float)Math.Cos(angle), sin = (float)Math.Sin(angle);
        private const float HALF_GRAVITY = 4.9f; // Gravity effect
        private const float SLIP_DURATION = 1.0f; // Slip duration
        private float timeAlive = 0f;
        private Vector3 origin;
        

        // Constructor:
        public Banana(ProjectileType type, Vector3 origin, Vector3 target, DrawModel model, float scaling) : base(type, origin, target, model, scaling) {}

        private float CalculateVelocity(Vector3 origin, Vector3 target)
        {
            // Calculate the horizontal distance (XZ-plane)
            float distance = Vector3.Distance(target, origin);

            // Calculate the initial velocity using the simplified formula
            return (float)Math.Sqrt((HALF_GRAVITY * distance) / (cos * sin));
        }

        protected override void Move(float dt)
        {
            if (onGround) return;

            timeAlive += dt;
            
            Vector3 horizontalMotion = Orientation * Velocity * cos;
            Vector3 verticalMotion = new Vector3(0, Velocity * sin - HALF_GRAVITY * timeAlive, 0);

            Position = origin + (horizontalMotion + verticalMotion) * timeAlive;
        }

        public override void OnPlayerHit(Player player)
        {
            base.OnPlayerHit(player);
            if(onGround) player.Slip(SLIP_DURATION);
        }

        public override void OnGroundHit()
        {
            Position = new Vector3(Position.X, 0, Position.Z);
            onGround = true;
        }

        public override void OnMobHit()
        {
            if (onGround) ToBeDeleted = true;
        }
        
        public override void Throw(float chargeUp)
        {
            onGround = false;
            base.Throw(chargeUp);
            Throw(Position - Orientation, Position+chargeUp*Orientation);
        }

        public override void Throw(Vector3 origin, Vector3 target) 
        {
            onGround = false;
            base.Throw(origin, target);
            Velocity = CalculateVelocity(origin, target);
            this.origin = origin;
            timeAlive = 0f;
        }
    }
}