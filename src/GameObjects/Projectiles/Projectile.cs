using System;
using Microsoft.Xna.Framework;

/** Class for the projectiles **/
namespace src.GameObjects
{
    public class Projectile : GameModel
    {
        // Private fields:
        public ProjectileType Type { get; set; }
        protected float velocity;

        // Constructor:
        public Projectile(ProjectileType type, Vector3 origin, Vector3 orientation)
        {
            Type = type;
            Position = origin;
            Orientation = Vector3.Normalize(orientation);
        }

        // Factory method to create a random projectile:
#nullable enable
        public static Projectile createProjectile(ProjectileType type, Vector3 origin, Vector3 target)
        {
            // Randomly create a projectile:
            switch (type)
            {
                case ProjectileType.Frog:
                    return new Frog(type, origin, target);
                case ProjectileType.Swordfish:
                    return new Swordfish(type, origin, target);
                default:
                    throw new ArgumentException("Invalid projectile type");
            }
        }


        // Move the projectile:
        public virtual void Move(float dt) { }
    }
}
