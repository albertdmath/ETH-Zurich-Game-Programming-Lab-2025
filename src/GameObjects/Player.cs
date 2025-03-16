using GameLab;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;

namespace src.GameObjects
{
    /** Anything regarding player characters, from movement to the actual Model files goes here. **/
    public class Player : GameModel
    {
        // Private fields:
        private float playerSpeed = 2f;
        private int life = 3;
        private int stamina = 3;
        //private Projectile *projectileHeld;
        private int typeOfProjectileHeld = -1; // -1 if no projectile held
        private bool isDashing = false;
        private const float TIME_CATCH_THROW = 0.5f;
        private float timeSinceCatch = 0f;

        // Constructor: Only allow to assign position here, lifes stamina and so on are a global property and need to be the same for
        public Player(Vector3 position)
        {
            this.position = position;
        }

        // Getters/Setters:
        public Vector3 Position
        {
            get { return this.position; }
            set { this.position = value; }
        }

        public int Life
        {
            get { return this.life; }
            set { this.life = value; }
        }

        // The player move method:
        public void Move(float dt)
        {
            Vector3 dir = new Vector3(0, 0, 0);
            KeyboardState newState = Keyboard.GetState();
            if (newState.IsKeyDown(Keys.A))
                dir.X -= 1;

            if (newState.IsKeyDown(Keys.D))
                dir.X += 1;

            if (newState.IsKeyDown(Keys.W))
                dir.Z -= 1;

            if (newState.IsKeyDown(Keys.S))
                dir.Z += 1;

            if (dir.Length() > 0)
            {
                dir = Vector3.Normalize(dir);
                this.orientation = dir;
            }

            position += playerSpeed * dir * dt;

            timeSinceCatch += dt;
        }

        // Method to grab an object:
        public bool Grab(Projectile projectile, LinkedList<Projectile> activeProjectiles)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.E) && this.typeOfProjectileHeld == -1 && Vector3.Distance(this.position, projectile.Position) < 2.0f && timeSinceCatch > TIME_CATCH_THROW)
            {
                this.typeOfProjectileHeld = projectile.Type;
                activeProjectiles.Remove(projectile);
                timeSinceCatch = 0f;
                Console.WriteLine("Grabbing " + projectile.Type);
                return true;
            }

            return false;
        }

        // Method to throw an object:
        public void Throw(LinkedList<Projectile> activeProjectiles)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.E) && this.typeOfProjectileHeld != -1 && timeSinceCatch > TIME_CATCH_THROW)
            {
                activeProjectiles.AddLast(Projectile.createProjectile(typeOfProjectileHeld, this.position + this.orientation, this.orientation));
                this.typeOfProjectileHeld = -1;
                timeSinceCatch = 0f;

                Console.WriteLine("Throwing projectile with orientation: " + this.orientation);
            }
        }


        public bool GetHit(Projectile projectile, LinkedList<Projectile> activeProjectiles, Player[] Players)
        {
            if (Vector3.Distance(this.position, projectile.Position) < 0.5)
            {
                this.life--;
                activeProjectiles.Remove(projectile);
                return true;
            }

            return false;
        }

        // Method to dash:
        public void Dash()
        {
            this.isDashing = true;
        }
    }
}