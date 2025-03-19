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
        private Vector2 ZombieSpeed = new Vector2(0,0);
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
            Orientation = new Vector3(ZombieSpeed.X,0f,ZombieSpeed.Y);
            if(ellipse.Inside(Position.X,Position.Z)){
                Orientation = ellipse.tangentPart(Position.X,Position.Z,Orientation);
            }
            Position += Orientation * dt;
            while(ellipse.Inside(Position.X,Position.Z)){
                Position += ellipse.Normal(Position.X,Position.Z) * dt * -0.1f;
            }
            //BoundingSphere = new BoundingSphere(Position,0.1f);
        }
        private void MoveBack(float dt)
        {
            Position -= Orientation * dt * 1f;
        }
        public override void Update(float dt){
            Move(dt);
            /* else{
                while(ellipse.Inside(Position.X,Position.Z)){
                    Position += ZombieSpeed * ellipse.Normal(Position.X,Position.Z) * dt * -0.1f;
                }
            } */
        }

        private bool Intersects(){
            for(int i=0; i<zombies.Count;++i){
                if(this.BoundingSphere.Intersects(zombies[i].BoundingSphere)&&zombies[i]!=this){
                    return true;
                }
            }
            return false;
        }

        private void Close(){
            for(int i=0; i<zombies.Count;++i){
                if(closeDistance(zombies[i])&&zombies[i]!=this){
                    Vector3 diff = zombies[i].Position-Position;
                    Orientation = Orientation - Vector3.Dot(Orientation,diff)/Vector3.Dot(diff,diff)*diff;

                }
            }
            //Console.WriteLine("Zombie runs with orientation: " + Orientation);
        }
        public void Force(){
            Vector3 dir = -1f*Position;
            if (dir.Length() > 0)
            {
                dir = Vector3.Normalize(dir);
            }
            ZombieSpeed = new Vector2(dir.X, dir.Z);
            for(int i=0; i<zombies.Count;++i){
                if(closeDistance(zombies[i])&&zombies[i]!=this){
                    Vector2 diff = Vector2.Normalize(new Vector2(Position.X-zombies[i].Position.X,Position.Y-zombies[i].Position.Y));
                    ZombieSpeed = ZombieSpeed + 2*diff/(diff.LengthSquared());

                }
            }
            //Console.WriteLine("Zombie runs with orientation: " + Orientation);
        }

        private bool closeDistance(Zombie zombie){
            float x = this.Position.X - zombie.Position.X;
            float y = this.Position.Y - zombie.Position.Y;
            //return (x*x+y*y)< 1f && Position.LengthSquared() > zombie.Position.LengthSquared();
            return (x*x+y*y)< 4f;
        }
        
    }
}