
using System.Numerics;
using System.Runtime.CompilerServices;

namespace src.ObjectClasses
{
/** Anything regarding player characters, from movement to the actual Model files goes here. **/
    public class Player : MoveableObject {

        private Vector2 position;
        private int life = 3; 
        private int stamina = 3; 
        private Projectile projectileHeld; 
        private bool isHoldingProjectile = false;

        public void Move(){

        }

        public void Grab(){

        }

        public void Dash(){

        }
    }
}