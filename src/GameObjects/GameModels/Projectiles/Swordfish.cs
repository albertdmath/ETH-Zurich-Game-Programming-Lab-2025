using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace src.GameObjects;
public class Swordfish : Projectile
{
    // Private fields:
    private const float MAX_VELOCITY = 20;

    // Constructor:
    public Swordfish(ProjectileType type, Vector3 origin, Vector3 target, DrawModel model, float scaling) : base(type, origin, target, model, scaling) {}

    protected override void Move(float dt)
    {
        Position += Velocity * Orientation * dt;
    }

    protected override void Hit()
    {   
        base.Hit();

        //check intersection with players
        bool hit = false;
        foreach (Player player in Player.active.Where(p => p.Life > 0))
        {
            if(Hitbox.Intersects(player.Hitbox))
                hit = player.GetHit(this);
            
        }

        //if intersects, update
        if(hit)
        {
            MusicAndSoundEffects.playProjectileSFX(ProjectileType.Swordfish);
            active.Remove(this);
        }
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
