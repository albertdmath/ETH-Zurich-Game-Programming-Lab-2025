using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace src.GameObjects
{
    /** Superclass for all moving objects **/
    public class GameModel {
        protected Vector3 position;
        protected Vector3 orientation; 
        protected Model model; 
        protected Hitbox hitbox;
    
    }
}