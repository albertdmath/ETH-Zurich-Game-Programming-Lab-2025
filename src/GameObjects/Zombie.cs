using GameLab;
using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using Accord.Math.Distances;

namespace src.GameObjects
{
    /** Anything regarding player characters, from movement to the actual Model files goes here. **/
    public class Zombie : GameModel
    {
        // Private fields:
        private float ZombieSpeed = 2f;
        private Ellipse ellipse;
        public BoundingSphere BoundingSphere { get; set; }


        // Constructor: Only allow to assign position here, lifes stamina and so on are a global property and need to be the same for
        public Zombie(Vector3 position, Ellipse ellipse)
        {
            Position = position;
            this.ellipse = ellipse;
        }

        // The Zombie move method:
        public void Move(float dt)
        {
            Vector3 dir = -1f*Position;
            if (dir.Length() > 0)
            {
                dir = Vector3.Normalize(dir);
                Orientation = dir;
            }
            Position += ZombieSpeed * Orientation * dt;
        }
        public void MoveBack(float dt)
        {
            Position -= ZombieSpeed * Orientation * dt * 0.2f;
        }
        public void Update(float dt){
            Move(dt);
            while(ellipse.Inside(Position.X,Position.Z)){
                MoveBack(dt);
            }
        }
    }
}