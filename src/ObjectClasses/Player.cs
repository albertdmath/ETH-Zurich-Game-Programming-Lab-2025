using GameLab;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using System.Runtime.CompilerServices;

namespace src.ObjectClasses
{
/** Anything regarding player characters, from movement to the actual Model files goes here. **/
    public class Player : MoveableObject {

        private Vector3 position = new Vector3(0,-3f,0);
        private float playerSpeed=10.0f;
        private int life = 3; 
        private int stamina = 3; 
        //private Projectile *projectileHeld;
        private KeyboardState oldState;
        private bool isHoldingProjectile = false;
        public Vector3 getposition(){
            return this.position;
        }
        public void Move(GameTime gameTime){
            float dt = (float) gameTime.ElapsedGameTime.TotalSeconds;
            Vector3 dir = new Vector3(0,0,0);
            KeyboardState newState = Keyboard.GetState();
                if(newState.IsKeyDown(Keys.A)){
                    dir.X += 1;
                    //position.X +=playerSpeed*dt;
                }
                if(newState.IsKeyDown(Keys.D)){
                    dir.X -= 1;
                    //position.X -=playerSpeed*dt;
                }
                if(newState.IsKeyDown(Keys.W)){
                    dir.Y -= 1;
                    //position.Y -=playerSpeed*dt;
                }
                if(newState.IsKeyDown(Keys.S)){
                    dir.Y += 1;
                    //position.Y +=playerSpeed*dt;
                }
                if(newState.IsKeyDown(Keys.R)){
                    playerSpeed*=1.02f;
                }
            position = position + dt * playerSpeed * dir;
            oldState = newState;
        }

        public void Grab(){
            if (this.isHoldingProjectile == true) return;
            
            //if(button pressed and projectile is in range)
            this.isHoldingProjectile = true;
            
        }

        public void Throw(){
            //if(button pressed)
            //looking direction
            //projectileHeld.Throw(position, new Vector3(0, 0, 0));
            this.isHoldingProjectile = false;
        }

        public void Dash(){

        }
    }
}