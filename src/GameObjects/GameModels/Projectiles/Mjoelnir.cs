using System;
using Microsoft.Xna.Framework;


namespace src.GameObjects;
public class Mjoelnir : Projectile
{
    // Constants
    private const float MAX_VELOCITY = 20;
    private bool DestroysOtherProjectiles = false;

    // Constructor:
    public Mjoelnir(ProjectileType type, DrawModel model, float scaling, float height) : base(type, model, scaling, height, IndicatorModels.Arrow) {}

    public override bool Action(float chargeUp, Vector3 aimPoint)
    {
        DestroysOtherProjectiles = true;
        (Holder as Player).JumpAndStrike();
        return false;
    }

    public override void OnProjectileHit(Projectile projectile)
    {
        if(DestroysOtherProjectiles)
            projectile.ToBeDeleted = true;
    }

    public override void Throw(Vector3 origin, Vector3 target) 
    {
        velocity = 2f;
        base.Throw(origin, target);
    }

    public override void Update(float dt)
    {
        if (DestroysOtherProjectiles) 
        {
            Position = Holder.Position + Holder.Orientation * 0.3f + new Vector3(0,0.2f,0);
            Orientation = Holder.Orientation;
        }
        else
        {
            base.Update(dt);
        }
    }
}
