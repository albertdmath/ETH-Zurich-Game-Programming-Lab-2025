using Microsoft.Xna.Framework;
using System;


namespace src.GameObjects
{
    /** Anything regarding player characters, from movement to the actual Model files goes here. **/
    public class Hand : GameModel
    {
        public bool IsCatching { get; set; } = false;
        // Private fields:
        private float timeSpentCatching = 0f;
        private Player player;
        private const float CATCH_DURATION = 0.25f; 
        private const float CATCH_RADIUS = 0.6f;
        private const float HAND_RADIUS = 0.5f; // Radius of the hand
        public Hand(Player player, DrawModel model, float scale) : base(model,scale, HAND_RADIUS)
        {
            this.player=player;
            OnBody();
        }

        // Moves the hand in a half circle around the body
        public void Move(float dt)
        {
            timeSpentCatching += dt;
            //Vector3 orthogonalHolderOrientation = new Vector3(-player.Orientation.Z, player.Orientation.Y, player.Orientation.X);
            Position = new Vector3(0.1f,-0.1f,0f)+player.Position + Vector3.Transform(new Vector3(CATCH_RADIUS,0,0f),Matrix.CreateRotationY(MathF.Atan2(-1f*Orientation.X,-1f*Orientation.Z)+(MathF.PI/CATCH_DURATION*timeSpentCatching)));
            Orientation = player.Orientation;
        }
        // Places the hand next to the body
        private void OnBody()
        {
            //Vector3 orthogonalHolderOrientation = new Vector3(-player.Orientation.Z, player.Orientation.Y, player.Orientation.X);
            Position = player.Position - new Vector3(0f,0.05f,0f);
            Orientation = player.Orientation;
        }
       

        // Update function called each update
        //this needs to be stilla adjusted for hitting yourself
        public override void Update(float dt)
        {
            if(timeSpentCatching>CATCH_DURATION || player.projectileHeld != null)
                StopCatching();
            
            if(IsCatching)
                Move(dt);

            else 
                OnBody();
        }
        public void StopCatching()
        {
            timeSpentCatching = 0f;
            IsCatching = false;
        }
    }
}