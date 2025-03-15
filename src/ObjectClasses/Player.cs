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
        private float playerSpeed = 1f;
        private int life = 3;
        private int stamina = 3;
        //private Projectile *projectileHeld;
        private int typeOfProjectileHeld = -1; // -1 if no projectile held
        private bool isDashing = false;

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
        public void Move(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
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
                dir.Z -= 1;
            }
            if (newState.IsKeyDown(Keys.S))
            {
                dir.Z += 1;
            }
            // if (newState.IsKeyDown(Keys.R))
            // {
            //     playerSpeed *= 1.02f;
            // }
            position += playerSpeed * dir * dt;
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