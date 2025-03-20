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
        private static float timeUntilNextProjectile = 0f;
        public ProjectileType Type { get; set; }
        protected float velocity;
        protected Player holdByPlayer = null;

        //there should be an UI element that lets you change this
        public static Dictionary<ProjectileType, float> projectileProbability = new Dictionary<ProjectileType, float>
        {
            { ProjectileType.Frog, 0.1f },
            { ProjectileType.Swordfish, 0f },
            { ProjectileType.Tomato, 0f }
        };

        // Constructor:
        public Projectile(ProjectileType type, Vector3 origin, Vector3 target)
        {
            Type = type;
            Position = origin;
            Orientation = Vector3.Normalize(target - origin);
        }

        // Static functions:
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
            //the probability to shoot is once every 0.1 second
            if ((timeUntilNextProjectile += dt) < 0.1f) return;
            
            foreach (var entry in projectileProbability)
            {
                if (rng.NextDouble() * 10 > entry.Value) continue;

                Vector3 origin = Ring.active.RndCircPoint();
                Vector3 target = Player.active[rng.Next(0, Player.active.Count)].Position;
                active.AddLast(createProjectile(entry.Key, origin, target));
            }

            timeUntilNextProjectile = 0f;
        }

        public virtual void Move(float dt) { }
        // Move the projectile:
        public void Update(float dt)
        {
            if (holdByPlayer == null)
                this.Move(dt);
            else
                this.Position = holdByPlayer.Position + holdByPlayer.Orientation * 0.3f + new Vector3(.1f, 0f, -.1f);
        }

        // this method since it does not use the constructor it doesnt reset any timers
        public void Throw()
        {
            this.Position = holdByPlayer.Position + holdByPlayer.Orientation;
            this.Orientation = holdByPlayer.Orientation;
            this.holdByPlayer = null;
        }

        public void Caught(Player player)
        {
            this.holdByPlayer = player;
        }
    }
}

