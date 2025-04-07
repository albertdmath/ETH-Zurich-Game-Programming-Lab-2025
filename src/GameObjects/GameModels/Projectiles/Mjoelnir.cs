using System;
using Microsoft.Xna.Framework;


namespace src.GameObjects;
public class Mjoelnir : Projectile
{
    // Constants
    private const float MAX_VELOCITY = 20;

    // Constructor:
    public Mjoelnir(ProjectileType type, Vector3 origin, Vector3 target, DrawModel model, float scaling, float height) : base(type, origin, target, model, scaling, height) {}

    protected override void Move(float dt)
    {
        Position += Velocity * Orientation * dt;
    }

    public override bool Action(float chargeUp)
    {
        if(Holder is Player)
        {
            DestroysOtherProjectiles = true;
            ((Player)Holder).JumpAndStrike();
        }
        return false;
    }

    public override void Throw(Vector3 origin, Vector3 target) 
    {
        base.Throw(origin, target);
        Velocity = 2f;
    }

    public override void Update(float dt)
    {
        if (DestroysOtherProjectiles) 
        {
            Position = Holder.Position + Holder.Orientation * 0.3f + new Vector3(0,0.2f,0);
            Orientation = Holder.Orientation;
        }else{
            base.Update(dt);
        }
    }
}
