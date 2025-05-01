using System;
using Microsoft.Xna.Framework;


namespace src.GameObjects;
public class Swordfish : Projectile
{
    // Constants
    private const float MAX_VELOCITY = 20;
    private const float MIN_VELOCITY = 2.5f;

    // Constructor:
    public Swordfish(ProjectileType type, DrawModel model, float scaling, float height) : base(type, model, scaling, height, IndicatorModels.Arrow) {}

    private static float CalculateVelocity(Vector3 origin, Vector3 target)
    {
        // Calculate the horizontal distance (XZ-plane)
        float distance = Math.Abs(Vector3.Distance(target, origin)) * 3;

        // Calculate the initial velocity using the simplified formula
        return Math.Clamp(distance, MIN_VELOCITY, MAX_VELOCITY);
    }

    public override void Throw(Vector3 origin, Vector3 target) 
    {
        velocity = (Holder is Player) ? CalculateVelocity(origin, target) : MIN_VELOCITY;
        base.Throw(origin, target);
    }
}
