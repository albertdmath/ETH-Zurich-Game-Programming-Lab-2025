using System;
using Microsoft.Xna.Framework;

namespace src.GameObjects
{
    public class Coconut : Projectile
    {
        // Constants
        private const float MAX_VELOCITY = 15;
        private const int MAX_BOUNCES = 3;
        private const float TIME_BETWEEN_BOUNCES = 0.5f;

        // Fields
        private readonly Random random = new();
        private int _bounces;
        private float _timeSinceBounce;


        // Constructor:
        public Coconut(ProjectileType type, Vector3 origin, Vector3 target, DrawModel model, float scaling) : base(type, origin, target, model, scaling) {}

        protected override void Move(float dt)
        {
            _timeSinceBounce -= dt;
            Position += Velocity * Orientation * dt;
        }

        private void Bounce()
        {
            // Otherwise bounce the coconut
            Velocity *= 0.9f;
            // Generate a random angle between -30° and +30°
            float randomAngle = MathHelper.ToRadians(random.Next(150, 210));

            // Create a rotation matrix around the Y-axis
            Matrix rotationMatrix = Matrix.CreateRotationY(randomAngle);

            // Apply the rotation to the orientation vector
            Orientation = Vector3.Transform(Orientation, rotationMatrix);
        }

        public override void OnPlayerHit(Player player) 
        {             
            player.GetHit(this);
            
            if (_timeSinceBounce > 0) 
                return;
        
            _bounces--;
            _timeSinceBounce = TIME_BETWEEN_BOUNCES;
    
            if (_bounces <= 0)
                ToBeDeleted = true;
            else 
                Bounce();
        }

        public override void OnMobHit()
        {
            if (_timeSinceBounce > 0) 
                return;
        
            _bounces--;
            _timeSinceBounce = TIME_BETWEEN_BOUNCES;

            if (_bounces <= 0) 
                return;

            Bounce();
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