using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace src.GameObjects
{
    public class InputDual : Input
    {
        Input input1,input2;
        // Constructor: Only allow to assign position here, lifes stamina and so on are a global property and need to be the same for
        public InputDual(Input input1, Input input2){
            this.input1 = input1;
            this.input2 = input2;
         }

        // The direction:
        public override Vector3 Direction()
        {
            return input1.Direction() + input2.Direction();
        }

        // Method returns true if action button is pressed
        public override bool Action()
        {
            return input1.Action() || input1.Action();
        }
        // Method to dash:
        public override bool Dash()
        {
            return input1.Dash() || input1.Dash();
        }
        public override bool Jump()
        {
            return input1.Jump() || input2.Jump();
        }
        public override void Vibrate(){
            input1.Vibrate();
            input2.Vibrate();
        }
        public override void EndVibrate(float dt){
            input1.EndVibrate(dt);
            input2.EndVibrate(dt);
        }
    }
}