using Microsoft.Xna.Framework;


namespace src.GameObjects;
public class Spear : Projectile
{
    // Constants
    private const float MIN_VELOCITY = 2.0f;
    private const float MAX_VELOCITY = 20f;
    private const float MIN_DASH_VELOCITY = 4f;
    private const float MAX_DASH_VELOCITY = 10f;
    private const float TIMER = 0.8f;

    private bool DestroysOtherProjectiles = false;

    // Constructor:
    public Spear(ProjectileType type, DrawModel model, float scaling, float height) : base(type, model, scaling, height, IndicatorModels.Arrow) 
    {
        this.velocity = MIN_VELOCITY;
    }

    public override void OnPlayerHit(Player player) 
    {   
        if(DestroysOtherProjectiles)
            player.GetHit(this);
        else
            base.OnPlayerHit(player);
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

        velocity = MathHelper.Lerp(MIN_DASH_VELOCITY, MAX_DASH_VELOCITY, chargeUp);
        DestroysOtherProjectiles = true;
        (Holder as Player).UseSpear(TIMER);
        Holder = null;
        return false;
    }
}
