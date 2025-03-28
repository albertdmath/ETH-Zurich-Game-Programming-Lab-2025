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
        Tomato,
        Coconut
    }

    /** Class for the projectiles **/
    public class Projectile : GameModel
    {
        // Static fields
        public static List<Projectile> active = new List<Projectile>();

        // Projectile properties
        public ProjectileType Type { get; private set; }
        protected float Velocity { get; set; }
        protected GameModel Holder { get; set; }

        // Projectile spawn probabilities (can be adjusted via UI)
        public static Dictionary<ProjectileType, float> ProjectileProbability = new Dictionary<ProjectileType, float>
        {
            { ProjectileType.Frog, 1f },
            { ProjectileType.Swordfish, 1f },
            { ProjectileType.Tomato, 1f },
            { ProjectileType.Coconut, 1f }
        };

        // Constructor:
        public Projectile(ProjectileType type, Vector3 origin, Vector3 target, Model model, float scaling) : base(model, scaling) 
        {
            Type = type;
            this.Throw(origin,target);
        }

        // Factory method to create a projectile
        public static Projectile CreateProjectile(ProjectileType type, Vector3 origin, Vector3 target)
        {
            Projectile projectile;
            switch (type)
            {
                case ProjectileType.Frog:
                    projectile = new Frog(type, origin, target, GameLabGame.projectileModels[ProjectileType.Frog], 0.7f);
                    break;
                case ProjectileType.Swordfish:
                    projectile = new Swordfish(type, origin, target, GameLabGame.projectileModels[ProjectileType.Swordfish], 0.9f);
                    break;
                case ProjectileType.Tomato:
                    projectile = new Tomato(type, origin, target, GameLabGame.projectileModels[ProjectileType.Tomato], 1f);
                    break;
                case ProjectileType.Coconut:
                    projectile = new Coconut(type, origin, target, GameLabGame.projectileModels[ProjectileType.Coconut], 1f);
                    break;
                default:
                    throw new ArgumentException("Invalid projectile type: ", type.ToString());
            }
            active.Add(projectile);
            return projectile;
        }

        // Virtual methods for derived classes to override
        protected virtual void Move(float dt) { }

        protected virtual void Hit() 
        { 
            // Check if projectile is out of bounds
            if (MathF.Abs(Position.X) > GameLabGame.ARENA_HEIGHT * 0.5f || 
                MathF.Abs(Position.Z) > GameLabGame.ARENA_WIDTH * 0.5f)
            {
                active.Remove(this);
                return;
            }
        }

        public virtual void Throw(float chargeUp) {
            this.Position = Holder.Position + Holder.Orientation;
            this.Orientation = Holder.Orientation;
            this.Holder = null;
            //Console.WriteLine("Projectile thrown with orientation: " + Orientation + " and speedup: " + chargeUp);
        }

        public virtual void Throw(Vector3 origin, Vector3 target) 
        {
            this.Holder = null;
            Position = origin;
            Orientation = Vector3.Normalize(target - origin);
        }

        // Update the projectile's state
        public override void Update(float dt)
        {
            Hit();

            if (Holder == null)
                Move(dt);
            else 
            {
                // Ensures projectile is held in right hand for a more realistic look:
                Vector3 orthogonalHolderOrientation = new Vector3(-Holder.Orientation.Z, Holder.Orientation.Y, Holder.Orientation.X);
                Position = Holder.Position + orthogonalHolderOrientation * 0.2f;
                Orientation = Holder.Orientation;
            }
        }

        // Catch the projectile
        public void Catch(GameModel player) { Holder = player; }

        public bool Free() { return Holder == null; }
    }
}