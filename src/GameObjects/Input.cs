using GameLab;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;

namespace src.GameObjects
{
    /** Anything regarding player characters, from movement to the actual Model files goes here. **/
    public class Input
    {
        // Constructor: Only allow to assign position here, lifes stamina and so on are a global property and need to be the same for
        public Input(){ }

        // The direction:
        public virtual Vector3 Direction()
        {
            Vector3 dir = new Vector3(0, 0, 0);
            KeyboardState newState = Keyboard.GetState();
            if (newState.IsKeyDown(Keys.A))
                dir.X -= 1;

            if (newState.IsKeyDown(Keys.D))
                dir.X += 1;

            if (newState.IsKeyDown(Keys.W))
                dir.Z -= 1;

            if (newState.IsKeyDown(Keys.S))
                dir.Z += 1;

            if (dir.Length() > 0)
            {
                dir = Vector3.Normalize(dir);
            }

            return dir;
        }

        // Method returns true if action button is pressed
        public virtual bool Action()
        {
            return Keyboard.GetState().IsKeyDown(Keys.E);
        }
        // Method to dash:
        public virtual bool Dash()
        {
            return Keyboard.GetState().IsKeyDown(Keys.G);
        }
        public virtual void Vibrate(){}
        public virtual void EndVibrate(float dt){}
    }
}