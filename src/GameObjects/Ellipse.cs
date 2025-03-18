using GameLab;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;

namespace src.GameObjects
{
    /** Anything regarding player characters, from movement to the actual Model files goes here. **/
    public class Ellipse
    {
        private float a;
        private float b;
        // Constructor: Only allow to assign position here, lifes stamina and so on are a global property and need to be the same for
        public Ellipse(float a, float b){ 
            this.a = a;
            this.b = b;
        }
        
        // Method returns true if inside elipse
        public bool Inside(float x, float y)
        {
            return ((x*x)/(a*a)+(y*y)/(b*b)<=1f);
        }
        // Method to dash:
        public bool Outside(float x, float y)
        {
            return ((x*x)/(a*a)+(y*y)/(b*b)>1f);
        }
        public void Set(float a, float b){ 
            this.a = a;
            this.b = b;
        }
    }
}