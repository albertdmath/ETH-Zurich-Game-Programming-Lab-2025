using System;
using Microsoft.Xna.Framework;


namespace src.GameObjects;
public class Swordfish : Projectile
{
    // Constants
    private const float MAX_VELOCITY = 20;

    // Constructor:
    public Swordfish(ProjectileType type, Vector3 origin, Vector3 target, DrawModel model, float scaling, float height) : base(type, origin, target, model, scaling, height) {}

    protected override void Move(float dt)
    {
        Position += Velocity * Orientation * dt;
    }

    public override void Throw(float chargeUp)
    {
        base.Throw(chargeUp);
        Velocity = Math.Min(Velocity + chargeUp, MAX_VELOCITY);
    }

    public override void Throw(Vector3 origin, Vector3 target) 
    {
        base.Throw(origin, target);
        Velocity = 2f;
    }
}
