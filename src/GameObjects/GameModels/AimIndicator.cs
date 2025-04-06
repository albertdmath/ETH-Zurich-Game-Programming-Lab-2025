using Microsoft.Xna.Framework;
using System;


namespace src.GameObjects
{
    /** Anything regarding aim indicator for players**/
    public class AimIndicator : GameModel
    {
        // Private fields:
        private Player player;
        
        public AimIndicator(Player player, DrawModel model,float scale) : base(model,scale)
        {
            this.player=player;
        }
        // Places the hand next to the body

        // Places the indicator
        public void PlaceIndicator(float timeSpentCharging, float speedOfCharging)
        {
            Position = player.Position + player.Orientation * timeSpentCharging * speedOfCharging;
        }
    }
}