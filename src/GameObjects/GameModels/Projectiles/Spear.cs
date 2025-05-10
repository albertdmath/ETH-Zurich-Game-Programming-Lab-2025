using Microsoft.Xna.Framework;


namespace src.GameObjects;
public class Spear : Projectile
{
    // Constants
    private const float MIN_VELOCITY = 2.0f;
    private const float MAX_VELOCITY = 20f;
    private const float DASH_VELOCITY = 4f;
    private bool DestroysOtherProjectiles = false;

    // Constructor:
    public Spear(ProjectileType type, DrawModel model, float scaling, float height) : base(type, model, scaling, height, IndicatorModels.Target) 
    {
        this.velocity = MIN_VELOCITY;
    }

    public override void OnProjectileHit(Projectile projectile)
    {
        if(DestroysOtherProjectiles)
            projectile.ToBeDeleted = true;
    }

    public override bool Action(float chargeUp, Vector3 aimPoint, bool isOutside)
    {
        if (isOutside)
        {
            velocity = MathHelper.Lerp(MIN_VELOCITY, MAX_VELOCITY, chargeUp);
            Throw(aimPoint);
            return true;
        }

        DestroysOtherProjectiles = true;
        (Holder as Player).UseSpear(DASH_VELOCITY);
        return false;
    }

    public override void Update(float dt)
    {
        if (DestroysOtherProjectiles) 
        {
            Orientation = Holder.Orientation;
            Vector3 orthogonalHolderOrientation = new(-Orientation.Z, Orientation.Y, Orientation.X);
            Position = Holder.Position + Orientation * 0.5f + orthogonalHolderOrientation * 0.2f + new Vector3(0,0.2f,0);
        }
        else
        {
            base.Update(dt);
        }
    }
}
