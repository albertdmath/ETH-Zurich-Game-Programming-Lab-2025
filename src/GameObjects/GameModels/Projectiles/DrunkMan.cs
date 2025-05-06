using Microsoft.Xna.Framework;


namespace src.GameObjects;
public class DrunkMan : Projectile
{
    // Constants
    private const float MAX_VELOCITY = 5.0f;
    private const float MIN_VELOCITY = 2.5f;

    // Constructor:
    public DrunkMan(ProjectileType type, DrawModel model, float scaling, float height) : base(type, model, scaling, height, IndicatorModels.Arrow) 
    {
        velocity = MIN_VELOCITY;
    }

    public override void Catch(GameModel catcher)
    {
        (catcher as Player).CatchDrunkMan(this);
    }

    public override bool Action(float chargeUp, Vector3 aimPoint, bool isOutside)
    {
        velocity = MathHelper.Lerp(MIN_VELOCITY, MAX_VELOCITY, chargeUp);
        Throw(aimPoint);
        return true;
    }
}
