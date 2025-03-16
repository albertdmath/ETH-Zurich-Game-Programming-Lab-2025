using System;
using Microsoft.Xna.Framework;

/** Class for the projectiles **/
namespace src.GameObjects
{
    public class Projectile : GameModel
    {
        // Private fields:
        protected int type;
        protected float velocity;

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
        public virtual void Move(float dt) { }
    }



   

}
