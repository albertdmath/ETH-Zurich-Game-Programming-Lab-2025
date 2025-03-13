using Microsoft.Xna.Framework.Input;

using System.Numerics;
using System.Runtime.CompilerServices;

namespace src.ObjectClasses
{
/** Anything regarding player characters, from movement to the actual Model files goes here. **/
    public class Player : MoveableObject {

        private Vector3 position = new Vector3(0,-3f,0);
        private float movespeed=0.2f;
        private int life = 3; 
        private int stamina = 3; 
        //private Projectile *projectileHeld;
        private bool isHoldingProjectile = false;
        public Vector3 getposition(){
            return this.position;
        }
        public void Move(){
            KeyboardState newState = Keyboard.GetState();
                if(newState.IsKeyDown(Keys.A)){
                    position.X += movespeed;
                }
                if(newState.IsKeyDown(Keys.W)){
                    position.Y -=movespeed;
                }
                if(newState.IsKeyDown(Keys.S)){
                    position.Y +=movespeed;
                }
                if(newState.IsKeyDown(Keys.D)){
                    position.X-=movespeed;
                }
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