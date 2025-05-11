using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace src.GameObjects
{
    /** Anything regarding player characters, from movement to the actual Model files goes here. **/
    public class Zombie : GameModel
    {
        // Private fields:
        public float ZombieSpeedX = 0;
        public float ZombieSpeedY = 0;

        private float movementSpeed = 0.6f;
        private Ellipse ellipse;
        public Vector3 Target = new Vector3(0f,0f,0f); // Target for movement
        private Player targetThrow; // Target for throw
        public Projectile projectileHeld;
        private float timeSinceSpawn = 0f;
        private GameStateManager gameStateManager;
        // Constructor: Only allow to assign position here,
        public Zombie(Vector3 position, Ellipse ellipse, DrawModel model, float scaling) : base(model, scaling)
        {
            Position = position;
            this.ellipse = ellipse;
            gameStateManager = GameStateManager.GetGameStateManager();
        }

        // The Zombie move method:
        private void Move(float dt)
        {   
            
            Vector3 oldPosition = Position;
            
             if(ellipse.Outside(Position.X, Position.Z) && (ZombieSpeedY*ZombieSpeedY+ZombieSpeedX*ZombieSpeedX)>0.15f){
                Vector3 speed = new Vector3(ZombieSpeedX,0f,ZombieSpeedY);
                if (speed.LengthSquared() > 0 && dt>0f){
 
                    Orientation = Vector3.Normalize(((1f-dt)*Orientation)+(dt*speed));
                }
            /* if(ellipse.Inside(Position.X,Position.Z)){
                speed = ellipse.tangentPart(Position.X,Position.Z,speed);
            } */
                Position += 0.5f*speed * dt;
            } 
            while(ellipse.Inside(Position.X,Position.Z)){
                Position += ellipse.Normal(Position.X,Position.Z) * dt * -0.1f;
            }

            if(Vector3.Distance(oldPosition, Position) < 0.005f)
            {
                SwitchAnimation(0,true,0.1f);
            } else {
                 SwitchAnimation(1,true,0.1f);
            }
        }
        //Updating movement and gravity towards center
        public override void Update(float dt){
            Move(dt);
            if(projectileHeld != null) Throw(dt);
            Vector3 dir = Target - Position;
            if (dir.Length() > 0)
            {
                dir = Vector3.Normalize(dir);
                //Orientation = dir;
            }
            ZombieSpeedX = dir.X * movementSpeed;
            ZombieSpeedY = dir.Z * movementSpeed;

        }
        // Force update for movement
        public void Force(List<Zombie> zombies,int index){
            for(int i=index+1; i<zombies.Count;++i){
                float x = this.Position.X - zombies[i].Position.X;
                float y = this.Position.Z - zombies[i].Position.Z;
                float lengthSquared = (x*x+y*y);
                float length = MathF.Sqrt(lengthSquared);
                if(lengthSquared<0.5f&&zombies[i]!=this){
                    float temp = (0.5f-lengthSquared)/length;
                    ZombieSpeedX += 10f*x*temp;
                    ZombieSpeedY += 10f*y*temp;
                    zombies[i].ZombieSpeedX -= 10f*x*temp;
                    zombies[i].ZombieSpeedY -= 10f*y*temp;
                }
            }
        }

        public void ForceByPlayer(Player player){
            float x = this.Position.X - player.Position.X;
            float y = this.Position.Z - player.Position.Z;
            float lengthSquared = (x*x+y*y);
            float length = MathF.Sqrt(lengthSquared);
            if(lengthSquared<0.5f){
                float temp = (0.5f-lengthSquared)/length;
                ZombieSpeedX += 10f*x*temp;
                ZombieSpeedY += 10f*y*temp;
            }
        }
        public void ForceByMarket(Market market){
            float x = this.Position.X - market.Position.X;
            float y = this.Position.Z - market.Position.Z;
            float lengthSquared = (x*x+y*y);
            float length = MathF.Sqrt(lengthSquared);
            if(lengthSquared<1.5f){
                float temp = (1.5f-lengthSquared)/length;
                ZombieSpeedX += 10f*x*temp;
                ZombieSpeedY += 10f*y*temp;
            }
        }
        
        public bool Spawn(ProjectileType type, Player target)
        {
            if(projectileHeld == null)
            {
                targetThrow = target;
                projectileHeld = gameStateManager.CreateProjectile(type);
                projectileHeld.Catch(this);
                timeSinceSpawn = 0f;
                return false;
            }
            return true;
        }
        
        private void Throw(float dt)
        {
            if(timeSinceSpawn < 2f)
            {
                timeSinceSpawn += dt;
                return;
            }
            //float speedUp = 1f;
            projectileHeld.Throw(targetThrow.Position + targetThrow.Orientation * Rng.NextFloat());
            projectileHeld = null;
            //Console.WriteLine("Mob throwing projectile with orientation: " + Orientation+ " and speedup: " + speedUp);
        }
    }
}