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
        private float ZombieSpeed = 2f;
        private Ellipse ellipse;
        public BoundingSphere BoundingSphere { get; set; }
        List<Zombie> zombies;


        // Constructor: Only allow to assign position here, lifes stamina and so on are a global property and need to be the same for
        public Zombie(Vector3 position, Ellipse ellipse, List<Zombie> zombies, Model model) : base(model)
        {
            Position = position;
            this.ellipse = ellipse;
            this.zombies = zombies;
            BoundingSphere = new BoundingSphere(Position,0.1f);

        }

        // The Zombie move method:
        private void Move(float dt)
        {
            Vector3 dir = -1f*Position;
            if (dir.Length() > 0)
            {
                dir = Vector3.Normalize(dir);
                Orientation = dir;
            }
            Position += ZombieSpeed * Orientation * dt;
            //BoundingSphere = new BoundingSphere(Position,0.1f);
        }
        private void MoveBack(float dt)
        {
            Position -= ZombieSpeed * Orientation * dt * 1f;
        }
        public void Update(float dt){
            Move(dt);
            if(ellipse.Inside(Position.X,Position.Z)||Close()){
                MoveBack(dt);
            }
        }

        private bool Intersects(){
            for(int i=0; i<zombies.Count;++i){
                if(this.BoundingSphere.Intersects(zombies[i].BoundingSphere)&&zombies[i]!=this){
                    return true;
                }
            }
            return false;
        }

        private bool Close(){
            for(int i=0; i<zombies.Count;++i){
                //if(this.BoundingSphere.Intersects(zombies[i].BoundingSphere)&&zombies[i]!=this){
                if(closeDistance(zombies[i])&&zombies[i]!=this){
                    return true;
                }
            }
            return false;
        }

        private bool closeDistance(Zombie zombie){
            float x = this.Position.X - zombie.Position.X;
            float y = this.Position.Y - zombie.Position.Y;
            return (x*x+y*y)< 0.01f && Position.LengthSquared() > zombie.Position.LengthSquared();
        }
    }
}