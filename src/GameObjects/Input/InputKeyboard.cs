using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace src.GameObjects
{
    /** Anything regarding player characters, from movement to the actual Model files goes here. **/
    public class InputKeyboard : Input
    {
        // Constructor: Only allow to assign position here, lifes stamina and so on are a global property and need to be the same for
        public InputKeyboard(){ }

        // The direction:
        public override Vector3 Direction()
        {
            Vector3 dir = new Vector3(0, 0, 0);
            KeyboardState newState = Keyboard.GetState();
            if (newState.IsKeyDown(Keys.Left))
                dir.X -= 1;

            if (newState.IsKeyDown(Keys.Right))
                dir.X += 1;

            if (newState.IsKeyDown(Keys.Up))
                dir.Z -= 1;

            if (newState.IsKeyDown(Keys.Down))
                dir.Z += 1;

            if (dir.Length() > 0)
            {
                dir = Vector3.Normalize(dir);
            }

            return dir;
        }

        // Method returns true if action button is pressed
        public override bool Action()
        {
            return Keyboard.GetState().IsKeyDown(Keys.P);
        }
        // Method to dash:
        public override bool Dash()
        {
            return Keyboard.GetState().IsKeyDown(Keys.L);
        }
        public override bool Jump()
        {
            return Keyboard.GetState().IsKeyDown(Keys.Enter);
        }
    }
}