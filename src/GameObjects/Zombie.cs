using GameLab;
using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using Accord.Math.Distances;
using Microsoft.Xna.Framework.Graphics;

namespace src.GameObjects
{
    /** Anything regarding player characters, from movement to the actual Model files goes here. **/
    public class Zombie : GameModel
    {
        // Private fields:
        public float ZombieSpeedX = 0;
        public float ZombieSpeedY = 0;

        private float movementSpeed = 0.3f;
        private Ellipse ellipse;
        // Constructor: Only allow to assign position here,
        public Zombie(Vector3 position, Ellipse ellipse, Model model, float scaling) : base(model, scaling)
        {
            Position = position;
            this.ellipse = ellipse;
        }

        // The Zombie move method:
        private void Move(float dt)
        {
            if(ellipse.Inside(Position.X, Position.Z)) return;

            Vector3 speed = new Vector3(ZombieSpeedX,0f,ZombieSpeedY);
            if (speed.LengthSquared() > 0){
                Orientation = Vector3.Normalize(speed);
            }
            /* if(ellipse.Inside(Position.X,Position.Z)){
                speed = ellipse.tangentPart(Position.X,Position.Z,speed);
            } */
            Position += speed * dt;
        /* while(ellipse.Inside(Position.X,Position.Z)){
            Position += ellipse.Normal(Position.X,Position.Z) * dt * -0.1f;
        } */
        }
        //Updating movement and gravity towards center
        public override void Update(float dt){
            Move(dt);
            Vector3 dir = -1f*Position;
            if (dir.Length() > 0)
                dir = Vector3.Normalize(dir);
            ZombieSpeedX = dir.X * movementSpeed;
            ZombieSpeedY = dir.Z * movementSpeed;
        }
        // Force update for movement
        public void Force(List<Zombie> zombies,int index){
            for(int i=index+1; i<zombies.Count;++i){
                float x = this.Position.X - zombies[i].Position.X;
                float y = this.Position.Z - zombies[i].Position.Z;
                float lengthSquared = (x*x+y*y);
                float length = (float)Math.Sqrt(lengthSquared);
                if(lengthSquared<0.5f&&zombies[i]!=this){
                    Vector2 diff = new Vector2(Position.X-zombies[i].Position.X,Position.Z-zombies[i].Position.Z);
                    ZombieSpeedX += 10f*x/length*(0.5f-diff.LengthSquared());
                    ZombieSpeedY += 10f*y/length*(0.5f-diff.LengthSquared());
                    zombies[i].ZombieSpeedX -= 10f*x/length*(0.5f-diff.LengthSquared());
                    zombies[i].ZombieSpeedY -= 10f*y/length*(0.5f-diff.LengthSquared());
                }
            }
        }
    }
}