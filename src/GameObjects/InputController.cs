using GameLab;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;

namespace src.GameObjects
{
    /** Anything regarding player characters, from movement to the actual Model files goes here. **/
    public class InputController : Input
    {
        PlayerIndex p;
        // Constructor: Only allow to assign position here, lifes stamina and so on are a global property and need to be the same for
        public InputController(PlayerIndex p){
            this.p = p;
         }

        // The direction:
        public override Vector3 Direction()
        {
            Vector2 input = Vector2.Zero;

            GamePadCapabilities gamePadCapabilities = GamePad.GetCapabilities(p);

            if (gamePadCapabilities.IsConnected)
            {
                input = GamePad.GetState(p).ThumbSticks.Left;
            }
            Vector3 dir = new Vector3(input.X, 0, -1f*input.Y);
            if (dir.Length() > 0)
            {
                dir = Vector3.Normalize(dir);
            }
            return dir;
        }

        // Method returns true if action button is pressed
        public override bool Action()
        {
            return GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed;
        }
        // Method to dash:
        public override bool Dash()
        {
            return GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Pressed;
        }
    }
}