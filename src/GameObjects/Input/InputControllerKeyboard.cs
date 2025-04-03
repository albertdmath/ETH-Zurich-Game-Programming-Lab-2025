using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace src.GameObjects
{
    public class InputControllerKeyboard : Input
    {
        PlayerIndex p;
        float vibrate =0f;
        public InputControllerKeyboard(PlayerIndex p){
            this.p = p;
         }

        // The direction:
        public override Vector3 Direction()
        {
            //KEYBOARD
            Vector3 dirKeyboard = new Vector3(0, 0, 0);
            KeyboardState newState = Keyboard.GetState();
            if (newState.IsKeyDown(Keys.A))
                dirKeyboard.X -= 1;

            if (newState.IsKeyDown(Keys.D))
                dirKeyboard.X += 1;

            if (newState.IsKeyDown(Keys.W))
                dirKeyboard.Z -= 1;

            if (newState.IsKeyDown(Keys.S))
                dirKeyboard.Z += 1;

            if (dirKeyboard.Length() > 0)
            {
                dirKeyboard = Vector3.Normalize(dirKeyboard);
            }
            //CONTROLLER
            Vector2 input = Vector2.Zero;

            GamePadCapabilities gamePadCapabilities = GamePad.GetCapabilities(p);

            if (gamePadCapabilities.IsConnected)
            {
                input = GamePad.GetState(p).ThumbSticks.Left;
            }
            Vector3 dirController = new Vector3(input.X, 0, -1f*input.Y);
            if (dirController.Length() > 0)
            {
                dirController = Vector3.Normalize(dirController);
            }
            return Vector3.Normalize(dirController + dirKeyboard);
        }

        // Method returns true if action button is pressed
        public override bool Action()
        {
            return GamePad.GetState(p).Buttons.A == ButtonState.Pressed ||Keyboard.GetState().IsKeyDown(Keys.E);
        }
        // Method to dash:
        public override bool Dash()
        {
            return GamePad.GetState(p).Buttons.B == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Space);
        }
        public override void Vibrate(){
            GamePad.SetVibration(p,1,1);
            vibrate = .2f;
        }
        public override void EndVibrate(float dt){
            vibrate-=dt;
            if(vibrate<0f)
                GamePad.SetVibration(p,0,0);
        }

        public bool Emote()
        {
            return GamePad.GetState(p).Buttons.X == ButtonState.Pressed;
        }
    }
}