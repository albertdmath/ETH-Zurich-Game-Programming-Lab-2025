using GameLab;
using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

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
        protected float baseVelocity;
        protected Player holdByPlayer = null;

        //there should be an UI element that lets you change this
        public static Dictionary<ProjectileType, float> projectileProbability = new Dictionary<ProjectileType, float>
        {
            { ProjectileType.Frog, 0.1f },
            { ProjectileType.Swordfish, 0.45f },
            { ProjectileType.Tomato, 0.45f }
        };

        // Constructor:
        public Projectile(ProjectileType type, Vector3 origin, Vector3 target, Model model) : base(model) 
        {
            Type = type;
            Position = origin;
            Orientation = Vector3.Normalize(target - origin);
            baseVelocity=velocity;
            // CalculateTransform(); think this can go in da trash
        }

        // Static functions:
        // Factory method to create a random projectile:
        public static Projectile createProjectile(ProjectileType type, Vector3 origin, Vector3 target ,Model model)
        {
            // Randomly create a projectile:
            switch (type)
            {
                case ProjectileType.Frog:
                    return new Frog(type, origin, target, model);
                case ProjectileType.Swordfish:
                    return new Swordfish(type, origin, target, model);
                case ProjectileType.Tomato:
                    return new Tomato(type, origin, target, model);
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
                if (rng.NextDouble() > entry.Value * 0.1) continue;

                Vector3 origin = Ring.active.RndCircPoint();
                Vector3 target = Player.active[rng.Next(0, Player.active.Count)].Position;
                active.AddLast(createProjectile(entry.Key, origin, target, GameLabGame.projectileModels[entry.Key]));
            }

            timeUntilNextProjectile = 0f;
        }

        public virtual void Move(float dt) { }
        // Move the projectile:
        public override void Update(float dt)
        {
            if (holdByPlayer == null)
                this.Move(dt);
            else
                this.Position = holdByPlayer.Position + holdByPlayer.Orientation * 0.3f + new Vector3(.1f, 0.2f, -.1f);
        }

        public void Throw(float speedUp)
        {
            this.Position = holdByPlayer.Position + holdByPlayer.Orientation + new Vector3(0, 0.2f, 0);
            this.Orientation = holdByPlayer.Orientation;
            this.holdByPlayer = null;
            velocity = baseVelocity * speedUp;
        }
        public void Caught(Player player)
        {
            this.holdByPlayer = player;
        }
    }
}

