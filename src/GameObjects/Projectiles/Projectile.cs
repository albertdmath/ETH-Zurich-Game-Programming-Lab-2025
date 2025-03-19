using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

/** Class for the projectiles **/
namespace src.GameObjects
{
    public class Projectile : GameModel
    {
        // Private fields:
        public static LinkedList<Projectile> active = new LinkedList<Projectile>();
        private static ProjectileType[] values = (ProjectileType[])Enum.GetValues(typeof(ProjectileType));
        private static float timeUntilNextProjectile = 5.0f;
        public ProjectileType Type { get; set; }
        protected float velocity;

        protected Player holdByPlayer = null;

        // Constructor:
        public Projectile(ProjectileType type, Vector3 origin, Vector3 orientation)
        {
            Type = type;
            Position = origin + new Vector3(0, 0.2f, 0);
            Orientation = Vector3.Normalize(orientation - origin);
        }

        // Factory method to create a random projectile:
        public static Projectile createProjectile(ProjectileType type, Vector3 origin, Vector3 target)
        {
            // Randomly create a projectile:
            switch (type)
            {
                case ProjectileType.Frog:
                    return new Frog(type, origin, target);
                case ProjectileType.Swordfish:
                    return new Swordfish(type, origin, target);
                case ProjectileType.Tomato:
                    return new Tomato(type, origin, target);
                default:
                    throw new ArgumentException("Invalid projectile type");
            }
        }

        public static void MobShoot(float dt, Random rng)
        {
            if ((timeUntilNextProjectile -= dt) <= 0)
            {
                ProjectileType type = (ProjectileType)values.GetValue(rng.Next(1, values.Length));
                Vector3 origin = Ring.active.RndCircPoint();
                Vector3 direction = Player.active[rng.Next(0, Player.active.Count)].Position - origin;
                Projectile newProjectile = createProjectile(type, origin, direction);
                active.AddLast(newProjectile);

                timeUntilNextProjectile = (float)rng.NextDouble() * 5f;
            }
        }

        public virtual void Move(float dt) { }
        // Move the projectile:
        public void Update(float dt)
        {
            if (holdByPlayer == null)
                this.Move(dt);
            else
                this.Position = holdByPlayer.Position + holdByPlayer.Orientation * 0.3f + new Vector3(.1f, 0.2f, -.1f);
        }

        public void Throw()
        {
            this.Position = holdByPlayer.Position + holdByPlayer.Orientation + new Vector3(0, 0.2f, 0);
            this.Orientation = holdByPlayer.Orientation;
            this.holdByPlayer = null;
        }
        public void Caught(Player player)
        {
            this.holdByPlayer = player;
        }
    }
}

