using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/** Class for the projectiles **/
namespace src.ObjectClasses
{
    public class Projectile : MoveableObject
    {
        private const float MAX_TIME_BETWEEN_PROJECTILES = 5000f; // 5 seconds in milliseconds
        private static float timeUntilNextProjectile = 5000f; // Random interval before next projectile
        // Factory method to create a random projectile
        #nullable enable
        public static Projectile? CreateRandomProjectile(GameTime gameTime)
        {
            timeUntilNextProjectile -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            // Check if enough time has passed
            if (timeUntilNextProjectile > 0) return null;

            // Reset timer and generate new random cooldown
            timeUntilNextProjectile = (float)rng.NextDouble() * MAX_TIME_BETWEEN_PROJECTILES;

            // Randomly create a projectile
            switch (rng.Next(0, 2))
            {
                case 0:
                    return new Frog();
                case 1:
                    return new Swordfish();
                default:
                    return null;
            }
        }

        //instantiate the projectile
        public void Throw(Vector3 origin, Vector3 target) {
            this.position = origin;
            this.orientation = Vector3.Normalize(target - origin);
            //also connect the sprite to the projectile
        }

        public void DrawProjectile(GraphicsDevice graphicsDevice, BasicEffect effect) {
            
        }

        public Vector3 GetPosition() {
            return this.position;
        }

        //move the projectile
        public virtual void Move(GameTime gameTime) {}
    }

    public class Frog : Projectile
    {
        private const float HOP_TIME = 1000f; // 1 second in milliseconds
        private const int velocity = 20;
        private float timeBeforeHop = 0f;

        public override void Move(GameTime gameTime)
        {
            timeBeforeHop += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (timeBeforeHop < HOP_TIME) return;
            
            this.position += velocity * orientation;
            this.timeBeforeHop = 0f;
        }
    }

    public class Swordfish : Projectile
    {
        private const int velocity = 50;

        public override void Move(GameTime gameTime)
        {
            this.position += velocity * orientation * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
