using System;
using Microsoft.Xna.Framework;

namespace src.GameObjects;

public enum IndicatorModels{ Target, Arrow, None };

/** Anything regarding aim indicator for players**/
public class AimIndicator : GameModel
{
    // Private fields:
    private readonly Player player;
    private readonly DrawModel targetModel;
    private readonly DrawModel arrowModel;
    public Vector3 Target { get; private set; } = Vector3.Zero;
    
    public AimIndicator(Player player, DrawModel targetModel,DrawModel arrowModel,float scale) : base(arrowModel,scale)
    {
        this.player=player;
        this.targetModel = targetModel;
        this.arrowModel = arrowModel;
    }
    // Places the hand next to the body

    // Places the indicator
    public void PlaceIndicator(float timeSpentCharging, float speedOfCharging, IndicatorModels indicatorModel)
    {
        Orientation = player.Orientation;
        Target = player.Position + Orientation * (0.2f + timeSpentCharging * speedOfCharging);

        switch(indicatorModel)
        {
            case IndicatorModels.Target:
                this.DrawModel = this.targetModel;
                Position = Target;
                break;
            case IndicatorModels.Arrow:
                this.DrawModel = this.arrowModel;
                Scaling = Matrix.CreateScale(1f + timeSpentCharging, 1f, 1f + timeSpentCharging);
                Position = player.Position;
                break;
            default:
                this.DrawModel = null;
                break;
        }
    }
}
