using GameLab;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

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
        // Projectile properties
        public ProjectileType Type { get; private set; }
        protected float Velocity { get; set; }
        public GameModel Holder { get; set; }
        protected GameStateManager gameStateManager;
        public bool ToBeDeleted { get; set; } = false;


        // Projectile spawn probabilities (can be adjusted via UI)
        public static Dictionary<ProjectileType, float> ProjectileProbability = new Dictionary<ProjectileType, float>
        {
            { ProjectileType.Frog, 1f },
            { ProjectileType.Swordfish, 1f },
            { ProjectileType.Tomato, 1f },
            { ProjectileType.Coconut, 1f }
        };

        public Projectile(ProjectileType type, Vector3 origin, Vector3 target, DrawModel model, float scaling) : base(model, scaling) 
        {
            Type = type;
            gameStateManager = GameStateManager.GetGameStateManager();
            Throw(origin,target);
        }

        // Virtual methods for derived classes to override
        protected virtual void Move(float dt) { }

        public virtual void OnPlayerHit(Player player) 
        {             
            ToBeDeleted = player.GetHit(this);
            
        }

        public virtual void OnMobHit()
        {
            return;
        }
        public virtual void OnGroundHit()
        {
            ToBeDeleted = true;
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
            if (Holder == null)
            {
                Move(dt);
            }
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