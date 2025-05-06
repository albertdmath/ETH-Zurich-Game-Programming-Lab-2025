using System;
using Microsoft.Xna.Framework;

namespace src.GameObjects;

public class Barrel : Projectile
{
    // Constants
    private const float HALF_GRAVITY = 4.9f; // Gravity effect
    private static readonly float angle = MathF.PI / 3; // angle of throw
    private static readonly float cos = MathF.Cos(angle), sin = MathF.Sin(angle);

    // Fields
    private float timeAlive;
    private Vector3 origin;
    private bool notMoving = false;

    // Constructor:
    public Barrel(ProjectileType type, DrawModel model1, DrawModel model2, float scaling, float height) : base(type, model1, scaling, height, IndicatorModels.Target) 
    { 
        this.DrawModel = Rng.NextBool() ? model1 : model2;
    }

    public override void OnGroundHit()
    {
        Position = new Vector3(Position.X, 0, Position.Z);
        notMoving = true;
    }

    public override void OnProjectileHit(Projectile projectile)
    {
        if(projectile.Holder == null)
        {
            projectile.ToBeDeleted = true;
            ToBeDeleted = true;
        }
    }

    public override void OnPlayerHit(Player player)
    {
        if (notMoving)
            player.ObjectCollision(this);
        else
            base.OnPlayerHit(player);
    }

    private static float CalculateVelocity(Vector3 origin, Vector3 target)
    {
        float distance = Vector3.Distance(target, origin);
        return MathF.Sqrt(HALF_GRAVITY * distance / (cos * sin));
    }

    public override void Throw(Vector3 target)
    {
        base.Throw(target);
        notMoving = false;
        velocity = CalculateVelocity(Position, target);
        origin = Position;
        timeAlive = 0f;
    }

    protected override void Move(float dt)
    {
        if (notMoving) return;

        timeAlive += dt;

        Vector3 horizontalMotion = Orientation * velocity * cos;
        Vector3 verticalMotion = new(0, velocity * sin - HALF_GRAVITY * timeAlive, 0);

        Position = origin + (horizontalMotion + verticalMotion) * timeAlive;
    }
}