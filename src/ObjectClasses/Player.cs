using GameLab;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Runtime.CompilerServices;

namespace src.ObjectClasses
{
    /** Anything regarding player characters, from movement to the actual Model files goes here. **/
    public class Player : MoveableObject
    {
        // Private fields:
        private Vector3 position = new Vector3(0, 0, 0);
        private float playerSpeed = 5.0f;
        private int life = 3;
        private int stamina = 3;
        //private Projectile *projectileHeld;
        private KeyboardState oldState;
        private int typeOfProjectileHeld = -1; // -1 if no projectile held
        private bool isDashing = false;

        // Constructor:
        public Player(Vector3 position, float playerSpeed, int life, int stamina)
        {
            this.position = position;
            this.playerSpeed = playerSpeed;
            this.life = life;
            this.stamina = stamina;
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
            {
                dir.X -= 1;
            }
            if (newState.IsKeyDown(Keys.D))
            {
                dir.X += 1;
            }
            if (newState.IsKeyDown(Keys.W))
            {
                dir.Y += 1;
            }
            if (newState.IsKeyDown(Keys.S))
            {
                dir.Y -= 1;
            }
            // if (newState.IsKeyDown(Keys.R))
            // {
            //     playerSpeed *= 1.02f;
            // }
            position += playerSpeed * dir * dt;
            oldState = newState;
        }

        // Method to grab an object:
        public void Grab(Projectile projectile)
        {
            if (this.typeOfProjectileHeld != -1) return;
            //if(button pressed and projectile is in range)
            this.typeOfProjectileHeld = projectile.Type;
        }

        // Method to throw an object:
        public void Throw()
        {
            //if(button pressed)
            //looking direction
            Projectile.createProjectile(typeOfProjectileHeld, this.position, new Vector3(0, 0, 0));
            this.typeOfProjectileHeld = -1;
        }

        // Method to dash:
        public void Dash()
        {
            this.isDashing = true;
        }
    }
}