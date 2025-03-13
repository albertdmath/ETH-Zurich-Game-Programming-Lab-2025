
using System.Numerics;

namespace src.ObjectClasses
{
/** Anything regarding player characters, from movement to the actual Model files goes here. **/
    public class Player : MoveableObject {

        private int life; 

        private int stamina; 

        private Projectile projectileHeld; 

        private bool isHoldingProjectile;
    }
}