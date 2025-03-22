using GameLab;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace src.GameObjects
{
    public enum ProjectileType
    {
        Frog,
        Swordfish,
        Tomato
    }

    /** Class for the projectiles **/
    public class Projectile : GameModel
    {
        // Static fields
        public static LinkedList<Projectile> active = new LinkedList<Projectile>();
        private static float timeUntilNextProjectile;

        // Projectile properties
        public ProjectileType Type { get; private set; }
        protected float Velocity { get; set; }
        protected Player Holder { get; set; }

        // Projectile spawn probabilities (can be adjusted via UI)
        public static Dictionary<ProjectileType, float> ProjectileProbability = new Dictionary<ProjectileType, float>
        {
            { ProjectileType.Frog, 0.1f },
            { ProjectileType.Swordfish, 0.45f },
            { ProjectileType.Tomato, 0.45f }
        };

        // Constructor
        public Projectile(ProjectileType type, Vector3 origin, Vector3 target, Model model) : base(model)
        {
            Type = type;
            Position = origin;
            Orientation = Vector3.Normalize(target - origin);
        }

        // Factory method to create a projectile
        public static Projectile CreateProjectile(ProjectileType type, Vector3 origin, Vector3 target, Model model)
        {
            switch (type)
            {
                case ProjectileType.Frog:
                    return new Frog(type, origin, target, model);
                case ProjectileType.Swordfish:
                    return new Swordfish(type, origin, target, model);
                case ProjectileType.Tomato:
                    return new Tomato(type, origin, target, model);
                default:
                    throw new ArgumentException("Invalid projectile type: ", type.ToString());
            }
        }

        // Mob shooting logic
        public static void MobShoot(float dt, Random rng)
        {
            //the probability to shoot is once every 0.1 second
            if ((timeUntilNextProjectile += dt) < 0.01f) return;

            foreach (var entry in ProjectileProbability)
            {
                if (rng.NextDouble() * 100 > entry.Value) continue;

                Vector3 origin = Mob.active[rng.Next(0, Mob.active.Length)].Position;
                Vector3 target = Player.active[rng.Next(0, Player.active.Count)].Position;
                active.AddLast(CreateProjectile(entry.Key, origin, target, GameLabGame.projectileModels[entry.Key]));
            }

            timeUntilNextProjectile = 0f;
        }

        // Virtual methods for derived classes to override
        public virtual void Move(float dt) { }
        public virtual void Throw(float chargeUp) { }

        // Update the projectile's state
        public override void Update(float dt)
        {
            if (Holder == null)
                Move(dt);
            else
                Position = Holder.Position + Holder.Orientation * 0.3f + new Vector3(0.1f, 0f, -0.1f);
        }

        // Catch the projectile
        public void Catch(Player player)
        {
            Holder = player;
        }
    }
}