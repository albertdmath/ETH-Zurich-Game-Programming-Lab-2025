using Microsoft.Xna.Framework;
using System;


namespace src.GameObjects
{
    /** Anything regarding aim indicator for players**/
    public class AimIndicator : GameModel
    {
        // Private fields:
        private Player player;
        DrawModel targetModel;
        DrawModel arrowModel;
        
        public AimIndicator(Player player, DrawModel targetModel,DrawModel arrowModel,float scale) : base(arrowModel,scale)
        {
            this.player=player;
            this.targetModel = targetModel;
            this.arrowModel = arrowModel;
        }
        // Places the hand next to the body

        // Places the indicator
        public void PlaceIndicator(float timeSpentCharging, float speedOfCharging,bool arrow)
        {
            Orientation = player.Orientation;
            if(arrow)
            {
                UpdateScale(1f+timeSpentCharging * speedOfCharging);
                this.DrawModel = this.arrowModel;
                Position = player.Position;
            }else{
                this.DrawModel = this.targetModel;
                UpdateScale(1f);
                Position = player.Position + player.Orientation * timeSpentCharging * speedOfCharging;
            }
            
        }
    }
}
