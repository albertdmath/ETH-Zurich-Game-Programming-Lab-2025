using System;
using Microsoft.Xna.Framework;

namespace src.GameObjects;

public class Barrel : Projectile
{
    // Constants
    private const float HALF_GRAVITY = 4.9f; // Gravity effect
    private static readonly float angle = (float)Math.PI / 3; // angle of throw
    private static readonly float cos = (float)Math.Cos(angle), sin = (float)Math.Sin(angle);

    // Fields
    private float timeAlive;
    private Vector3 origin;
    private bool notMoving = false; // Flag to indicate if the barrel is moving

    // Constructor:
    public Barrel(ProjectileType type, Vector3 origin, Vector3 target, DrawModel model1, DrawModel model2, float scaling, float height) : base(type, origin, target, model1, scaling, height, IndicatorModels.Target) 
    { 
        DrawModel = Rng.NextBool() ? model1 : model2;
    }

    private static float CalculateVelocity(Vector3 origin, Vector3 target)
    {
        float distance = Vector3.Distance(target, origin);
        return (float)Math.Sqrt((HALF_GRAVITY * distance) / (cos * sin));
    }

    protected override void Move(float dt)
    {
        if (notMoving) return;

        timeAlive += dt;

        Vector3 horizontalMotion = Orientation * velocity * cos;
        Vector3 verticalMotion = new(0, velocity * sin - HALF_GRAVITY * timeAlive, 0);

        Position = origin + (horizontalMotion + verticalMotion) * timeAlive;
    }

    public override void OnGroundHit()
    {
        Position = new Vector3(Position.X, 0, Position.Z);
        notMoving = true;
    }

    public override void OnProjectileHit(Projectile projectile)
    {
        projectile.ToBeDeleted = true;
        this.ToBeDeleted = true;
    }

    public override void OnPlayerHit(Player player)
    {
        if (notMoving)
            player.ObjectCollision(this);
        else
            base.OnPlayerHit(player);
    }

    public override void Throw(Vector3 origin, Vector3 target)
    {
        base.Throw(origin, target);
        notMoving = false;
        velocity = CalculateVelocity(origin, target);
        this.origin = origin;
        timeAlive = 0f;
    }
}
