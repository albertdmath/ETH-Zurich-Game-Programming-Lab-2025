using System;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace src.GameObjects;
public class Mjoelnir : Projectile
{
    // Constants
    private const float MIN_VELOCITY = 2.0f;
    private const float MAX_VELOCITY = 20f;
    private const float EXPLOSION_TIME = 0.5f;
    private const float EXPLOSION_RADIUS = 1.7f;
    private const float RATIO = 1.7647f;
    private readonly DrawModel explosionModel;

    private float explodeTime = 0f;
    private bool DestroysOtherProjectiles = false;

    // Constructor:
    public Mjoelnir(ProjectileType type, DrawModel model, DrawModel explosion, float scaling, float height) : base(type, model, scaling, height, IndicatorModels.Target) 
    {
        this.velocity = MIN_VELOCITY;
        this.explosionModel = explosion;
    }

    public void Explode()
    {
        explodeTime = EXPLOSION_TIME;
        Position = new(Position.X, 0, Position.Z);
        DrawModel = explosionModel;
        Scaling = Matrix.CreateScale(EXPLOSION_RADIUS * RATIO);
        Hitbox = new Sphere(Position, EXPLOSION_RADIUS);
        Holder = null;
    }

    public override void OnPlayerHit(Player player) 
    {  
        if(DrawModel == explosionModel && player.GetAffected(this))
        {
            player.StunAndSlip(0.8f, 0);
            player.GetHit(this); 
        }
        else
            ToBeDeleted = player.GetHit(this);  
    }

    public override void OnProjectileHit(Projectile projectile)
    {
        if(DestroysOtherProjectiles)
            projectile.ToBeDeleted = true;
    }

    public override bool Catch(GameModel player)
    {
        if (explodeTime > 0)
            return false;

        return base.Catch(player);
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
        Orientation = Holder.Orientation;
        (Holder as Player).JumpAndStrike();
        return false;
    }

    public override void Update(float dt)
    {
        explodeTime -= dt;
        if(explodeTime > 0) 
            return;

        if(DrawModel == explosionModel)
            ToBeDeleted = true;
        else if (DestroysOtherProjectiles) 
            Position = Holder.Position + Holder.Orientation * 0.3f + new Vector3(0,0.2f,0);
        else
            base.Update(dt);
    }
}
