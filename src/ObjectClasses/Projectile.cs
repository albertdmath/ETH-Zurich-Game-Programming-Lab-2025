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
        public virtual void Move(GameTime gameTime) {
            
        }
    }

    public class Frog : Projectile
    {
        public Frog(int type, Vector3 origin, Vector3 target) : base(type, origin, target) { }

        private const float HOP_TIME = 1000f; // 1 second in milliseconds
        private const int velocity = 2;
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
        public Swordfish(int type, Vector3 origin, Vector3 target) : base(type, origin, target) {

        }

        private const int velocity = 5;

        public override void Move(GameTime gameTime)
        {
            this.position += velocity * orientation * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }

}
