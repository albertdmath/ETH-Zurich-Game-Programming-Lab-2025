using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace src.ObjectClasses
{
    /** Superclass for all moving objects **/
    public class MoveableObject {
        protected static Random rng = new Random();
        protected Vector3 position;
        protected Vector3 velocity; 
        protected Vector3 orientation; 
        protected Model model; 

    }
}