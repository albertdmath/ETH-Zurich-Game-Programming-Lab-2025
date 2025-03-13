using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/** Class for the projectiles **/
namespace src.ObjectClasses
{
    public class Projectile : MoveableObject
    {
        protected Vector3 origin, target;
        private static Random rng = new Random();
        private const float MAX_TIME_BETWEEN_PROJECTILES = 5000f; // 5 seconds in milliseconds
        private static float timeUntilNextProjectile = 5000f; // Random interval before next projectile
        //can throw be simply the constructor?
        public virtual void Throw(Vector3 origin, Vector3 target) {} //instantiate the projectile

        public virtual void Move(GameTime gameTime) {} //move the projectile

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
    }

    public class Frog : Projectile
    {
        public override void Throw(Vector3 origin, Vector3 target)
        {
            this.origin = origin;
            this.target = target;
            //initialization
            Console.WriteLine("Frog thrown from " + origin + " to " + target);
        }

        public override void Move(GameTime gameTime)
        {
            //movement
            Console.WriteLine("Frog moving");
        }

    }

    public class Swordfish : Projectile
    {
        public override void Throw(Vector3 origin, Vector3 target)
        {
            this.origin = origin;
            this.target = target;
            Console.WriteLine("Swordfish thrown from " + origin + " to " + target);
        }

        public override void Move(GameTime gameTime)
        {
            Console.WriteLine("Swordfish moving");
        }
    }
}
