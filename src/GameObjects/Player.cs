using GameLab;
using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using Accord.Math.Distances;

namespace src.GameObjects
{
    /** Anything regarding player characters, from movement to the actual Model files goes here. **/
    public class Player : GameModel
    {
        // Private fields:
        private float playerSpeed = 2f;
        public int Life { get; set; } = 3;
        public float Stamina { get; set; } = 0f;
        private Projectile projectileHeld;
        //private ProjectileType typeOfProjectileHeld = ProjectileType.None;
        private float dashTime = 0f;
        private const float TIME_CATCH_THROW = 0.5f;
        private float timeSinceThrow = 0f;
        private bool actionPushed;

        private Input input;

        // Constructor: Only allow to assign position here, lifes stamina and so on are a global property and need to be the same for
        public Player(Vector3 position, Input input)
        {
            Position = position;
            this.input = input;
            projectileHeld = null;
        }

        // The player move method:
        public void Move(float dt)
        {
            Vector3 dir = input.Direction();
            if (dir.Length() > 0)
            {
                dir = Vector3.Normalize(dir);
                Orientation = dir;
            }
            Position += playerSpeed * dir * dt;
        }

        // Method to grab an object:
        public bool GrabOrHit(Projectile projectile, LinkedList<Projectile> activeProjectiles)
        {   
            if(projectile != projectileHeld){
                if (input.Action() 
                    && projectileHeld == null
                    && Vector3.Distance(Position, projectile.Position) < 1.0f
                    && timeSinceThrow>1f)
                {
                    projectileHeld = projectile;
                    projectile.Caught(this);
                    Console.WriteLine("Grabbing " + projectile.Type);
                    return false;
                }else if (Vector3.Distance(Position, projectile.Position) < 0.5)
                {
                    Life--;
                    return true;
                }
            }
            return false;
        }

        // Method to throw an object:
        public void Throw(float dt)
        {
            if (input.Action() && !actionPushed)
            {
                projectileHeld.Throw();
                projectileHeld = null;
                timeSinceThrow = 0f;
                Console.WriteLine("Throwing projectile with orientation: " + Orientation);
            }
        }

        // Method to dash:
        public bool Dash(float dt)
        {
            if(input.Dash() &&dashTime<=0f && Stamina>60f){
                dashTime=0.1f;
                Stamina-=60f;
            }
            if(dashTime>0f){
                Position += 6*playerSpeed * Orientation * dt;
                dashTime -= dt;
                return true;
            }else{
                return false;
            }
        }

        public void update(float dt){
            Stamina +=dt*10;
            Stamina = (Stamina >100f) ? 100f : Stamina;
            if(!Dash(dt)){
                Move(dt);
                if(projectileHeld != null){
                    Throw(dt);
                }else {
                    timeSinceThrow += dt;
                }
            }
            actionPushed = input.Action();
        }
    }
}