using System;
using Microsoft.Xna.Framework;

namespace src.GameObjects;

public class Tomato : Projectile
{
    // Constants
    private const float HALF_GRAVITY = 4.9f; // Gravity effect
    private const float EXPLOSION_RADIUS = 1f; // Define the explosion radius
    private static readonly float angle = MathF.PI / 3; // angle of throw
    private static readonly float cos = MathF.Cos(angle), sin = MathF.Sin(angle);

    // Fields
    private float timeAlive;
    private Vector3 origin;

    // Constructor:
    public Tomato(ProjectileType type, DrawModel model, float scaling, float height) : base(type, model, scaling, height, IndicatorModels.Target) {}

    private static float CalculateVelocity(Vector3 origin, Vector3 target)
    {
        float distance = Vector3.Distance(target, origin);
        return MathF.Sqrt(HALF_GRAVITY * distance / (cos * sin));
    }

    public override void OnGroundHit()
    {
        base.OnGroundHit();
        Position = new(Position.X, 0, Position.Z);
        gameStateManager.CreateAreaDamage(Position, EXPLOSION_RADIUS, null, ProjectileType.Tomato);
    }

    public override void OnPlayerHit(Player player)
    {
        if (player.GetAffected(this))
        {
            ToBeDeleted = true;
            gameStateManager.CreateAreaDamage(Position, EXPLOSION_RADIUS, null, ProjectileType.Tomato);
        }
    }

    public override void Throw(Vector3 target)
    {
        base.Throw(target);
        velocity = CalculateVelocity(Position, target);
        origin = Position;
        timeAlive = 0f;
    }

    protected override void Move(float dt)
    {
        timeAlive += dt;

        Vector3 horizontalMotion = Orientation * velocity * cos;
        Vector3 verticalMotion = new(0, velocity * sin - HALF_GRAVITY * timeAlive, 0);

        Position = origin + (horizontalMotion + verticalMotion) * timeAlive;
    }
}
