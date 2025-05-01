using System;
using Microsoft.Xna.Framework;


namespace src.GameObjects;
public class Swordfish : Projectile
{
    // Constants
    private const float MAX_VELOCITY = 20;
    private const float MIN_VELOCITY = 2.5f;

    // Constructor:
    public Swordfish(ProjectileType type, DrawModel model, float scaling, float height) : base(type, model, scaling, height, IndicatorModels.Arrow) 
    {
        velocity = MIN_VELOCITY;
    }

    public override bool Action(float chargeUp, Vector3 aimPoint, bool isOutside)
    {
        velocity = MathHelper.Lerp(MIN_VELOCITY, MAX_VELOCITY, chargeUp);
        Throw(aimPoint);
        return true;
    }
}
