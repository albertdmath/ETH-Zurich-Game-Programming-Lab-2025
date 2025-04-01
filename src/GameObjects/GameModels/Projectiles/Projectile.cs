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


        // Projectile spawn probabilities (can be adjusted via UI)
        public static Dictionary<ProjectileType, float> ProjectileProbability = new Dictionary<ProjectileType, float>
        {
            { ProjectileType.Frog, 0f },
            { ProjectileType.Swordfish, 0f },
            { ProjectileType.Tomato, 0f },
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

        public virtual bool Hit() 
        { 
            // Check for player intersections
            bool isHit = false;
            foreach (Player player in gameStateManager.players.Where(p => p.Life > 0))
            {   
                if (Hitbox.Intersects(player.Hitbox))
                    isHit = player.GetHit(this);
            }

            // Play soundeffect if it's a hit
            if(isHit)
                MusicAndSoundEffects.playProjectileSFX(Type);
            
            // Return true if it's a hit or if projectile is out of bounds so the projectile gets deleted
            return isHit || MathF.Abs(Position.X) > GameLabGame.ARENA_HEIGHT * 0.5f || MathF.Abs(Position.Z) > GameLabGame.ARENA_WIDTH * 0.5f;
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