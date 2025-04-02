using System;
using Microsoft.Xna.Framework;


namespace src.GameObjects;
public class Turtle : Projectile
{
    // Constants
    private const float TIME_TO_EAT = 5;

    // Constructor:
    public Turtle(ProjectileType type, Vector3 origin, Vector3 target, DrawModel model, float scaling) : base(type, origin, target, model, scaling) {}

    protected override void Move(float dt)
    {
        Position += Velocity * Orientation * dt;
    }

    public override void Throw(float chargeUp)
    {
        if(chargeUp > TIME_TO_EAT)
        {
            this.Holder = null;
            //Holder.WearTurtle();
        }
    }

    public override void Throw(Vector3 origin, Vector3 target) 
    {
        base.Throw(origin, target);
        Velocity = 2f;
    }
}
