using System;
using Microsoft.Xna.Framework;


namespace src.GameObjects;
public class Swordfish : Projectile
{
    // Constants
    private const float MAX_VELOCITY = 20;
    private const float MIN_VELOCITY = 2.5f;

    // Constructor:
    public Swordfish(ProjectileType type, Vector3 origin, Vector3 target, DrawModel model, float scaling, float height) : base(type, origin, target, model, scaling, height) {
        aimIndicatorIsArrow = true;
    }

    protected override void Move(float dt)
    {
        Position += Velocity * Orientation * dt;
    }

    private static float CalculateVelocity(Vector3 origin, Vector3 target)
    {
        // Calculate the horizontal distance (XZ-plane)
        float distance = Math.Abs(Vector3.Distance(target, origin)) * 3;

        // Calculate the initial velocity using the simplified formula
        return Math.Clamp(distance, MIN_VELOCITY, MAX_VELOCITY);
    }

    public override void Throw(Vector3 origin, Vector3 target) 
    {
        Velocity = (Holder is Player) ? CalculateVelocity(origin, target) : MIN_VELOCITY;
        base.Throw(origin, target);
    }
}
