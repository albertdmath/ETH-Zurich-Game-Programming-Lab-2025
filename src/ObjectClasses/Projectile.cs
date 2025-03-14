using System;
using Microsoft.Xna.Framework;

/** Class for the projectiles **/
namespace src.ObjectClasses
{
    public class Projectile : MoveableObject
    {
        protected int type;

        public Projectile(int type, Vector3 origin, Vector3 target)
        {
            this.position = origin;
            this.orientation = Vector3.Normalize(target - origin);
            this.type = type;
        }

        // Factory method to create a random projectile
        #nullable enable
        public static Projectile CreatePrj(int type, Vector3 origin, Vector3 target)
        {
            // Randomly create a projectile
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

        //instantiate the projectile

        public Vector3 GetPosition() {
            return this.position;
        }

        public int getType() {
            return this.type;
        }
        //move the projectile
        public virtual void Move(GameTime gameTime) {}
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
