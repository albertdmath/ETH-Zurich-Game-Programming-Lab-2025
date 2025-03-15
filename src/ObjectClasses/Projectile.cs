using System;
using Microsoft.Xna.Framework;

/** Class for the projectiles **/
namespace src.ObjectClasses
{
    public class Projectile : MoveableObject
    {
        // Private fields:
        protected int type;
        protected Vector3 position;
        protected Vector3 orientation;

        // Constructor:
        public Projectile(int type, Vector3 origin, Vector3 target)
        {
            this.type = type;
            this.position = origin;
            this.orientation = Vector3.Normalize(target - origin);
        }

        // Factory method to create a random projectile:
        #nullable enable
        public static Projectile createProjectile(int type, Vector3 origin, Vector3 target)
        {
            // Randomly create a projectile:
            switch (type)
            {
                case 0:
                    return new Frog(type, origin, target);
                case 1:
                    return new Swordfish(type, origin, target);
                default:
                    throw new ArgumentException("Invalid projectile type");
            }
        }

        // Getters/Setters:
        public int Type {
            get { return this.type; }
            set { this.type = value; }
        }
        public Vector3 Position {
            get { return this.position; }
            set { this.position = value; }
        }
        public Vector3 Orientation {
            get { return this.orientation; }
            set { this.orientation = value; }
        }
        // Move the projectile:
        public virtual void Move(GameTime gameTime)
        {    
        }
    }

    public class Frog : Projectile
    {
        // Private fields:
        private const float HOP_TIME = 1000f; // 1 second in milliseconds
        private const int velocity = 1;
        private float timeBeforeHop = 0f;

        // Constructor:
        public Frog(int type, Vector3 origin, Vector3 target) : base(type, origin, target) { }

        public override void Move(GameTime gameTime)
        {
            // timeBeforeHop += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            // if (timeBeforeHop < HOP_TIME) return;
            // Maybe frog can sit still for one second. Too tired right now to figure out how to do this.
            this.position += velocity * orientation * (float)gameTime.ElapsedGameTime.TotalSeconds;
            // Just a small trick to make the frog bounce, it's more visually appealing than teleporting frog:
            this.position.Y = (float)Math.Abs(Math.Sin(this.position.X));
            this.timeBeforeHop = 0f;
        }
    }

    public class Swordfish : Projectile
    {
        // Private fields:
        private const int velocity = 5;

        // Constructor:
        public Swordfish(int type, Vector3 origin, Vector3 target) : base(type, origin, target) { }

        public override void Move(GameTime gameTime)
        {
            this.position += velocity * orientation * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }

}
