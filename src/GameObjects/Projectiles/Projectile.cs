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
        public static int frogCount = 0;
        public static int maxFrogs = 5;

        protected Player holdByPlayer = null;

        // Constructor:
        public Projectile(ProjectileType type, Vector3 origin, Vector3 orientation)
        {
            Type = type;
            Position = origin + new Vector3(0,0.2f,0);
            Orientation = Vector3.Normalize(target - origin);
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
                case ProjectileType.Tomato:
                    return new Tomato(type, origin, target);
                default:
                    throw new ArgumentException("Invalid projectile type");
            }
        }

        // Move the projectile:
        public virtual void Move(float dt, Vector3 playerPosition) { }
        public void Update(float dt) { 
            if(holdByPlayer==null){
                this.Move(dt);
            }else{
                this.Position = holdByPlayer.Position + holdByPlayer.Orientation*0.3f+new Vector3(.1f, 0.2f, -.1f);
            }
        }
    
        // Move the projectile:
        public virtual void Move(float dt) { }

        public  void Throw() { 
            this.Position = holdByPlayer.Position + holdByPlayer.Orientation + new Vector3(0,0.2f,0);
            this.Orientation = holdByPlayer.Orientation;
            holdByPlayer = null;
        }
        public  void Caught(Player player) { 
            this.holdByPlayer=player;
        }
    }
}

