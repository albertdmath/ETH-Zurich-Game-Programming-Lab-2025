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
        // Constructor: assign radius a for x axis and b for z axis
        public Ellipse(float a, float b){ 
            this.a = a;
            this.b = b;
        }
        
        // Method returns true if inside elipse
        public bool Inside(float x, float y)
        {
            return ((x*x)/(a*a)+(y*y)/(b*b)<=1f);
        }
        // Method true if outside elipse
        public bool Outside(float x, float y)
        {
            return ((x*x)/(a*a)+(y*y)/(b*b)>1f);
        }
        public void Set(float a, float b){ 
            this.a = a;
            this.b = b;
        }

        // Returns vector tangent to ellipse in the given point
        private Vector3 Tangent(float x, float y){
            if(y==0f){
                return new Vector3(1f,0f,0f);
            }else{
                return Vector3.Normalize(new Vector3(1f,0f,-1f*b*b*x/(a*a*y)));
            }
        }
        // Returns vector normal to ellipse in the given point pointing inwards
        public Vector3 Normal(float x, float y){
            if(x==0f){
                return new Vector3(0f,0f,1f) * (y>0f?-1f:1f);
            }else{
                float temp = (x<0) ? 1f : -1f;
                return Vector3.Normalize(new Vector3(temp,0f,temp*(a*a*y)/(b*b*x)));
            }
        }

        public Vector3 tangentPart(float x, float y,Vector3 orientation){
            Vector3 temp = Tangent(x,y);
            return Vector3.Dot(orientation,temp)*temp;
        }
    }
}