using System;
using Microsoft.Xna.Framework;

namespace src.GameObjects;

public class Tomato : Projectile
{
    // Constants
    private const float HALF_GRAVITY = 4.9f; // Gravity effect
    private const float EXPLOSION_RADIUS = 1f; // Define the explosion radius
    private const float EXPLOSION_TIME = 0.5f;
    private static readonly float angle = MathF.PI / 3; // angle of throw
    private static readonly float cos = MathF.Cos(angle), sin = MathF.Sin(angle);
    private readonly DrawModel explodedModel;

    // Fields
    private float timeAlive;
    private Vector3 origin;
    private enum status{normal, exploded}
    private status currStatus = status.normal;

    // Constructor:
    public Tomato(ProjectileType type, DrawModel model, DrawModel exploded, float scaling, float height) : base(type, model, scaling, height, IndicatorModels.Target, HitboxType.Sphere) 
    {
        this.explodedModel = exploded;
    }

    private static float CalculateVelocity(Vector3 origin, Vector3 target)
    {
        float distance = Vector3.Distance(target, origin);
        return MathF.Sqrt(HALF_GRAVITY * distance / (cos * sin));
    }

    private void Explode()
    {
        timeAlive = EXPLOSION_TIME;
        Position = new(Position.X, 0, Position.Z);
        this.DrawModel = explodedModel;
        updateHitbox();
        currStatus = status.exploded;
    }

    public override void OnGroundHit(bool touching)
    {
        if(touching)
            Explode();
    }

    public override void OnPlayerHit(Player player)
    {
        if (currStatus == status.exploded)
            player.LoseLife();
        else if (player.GetAffected(this))
            Explode();
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

    public override void Update(float dt)
    {
        if(currStatus == status.normal)
            base.Update(dt);
        else
        {
            timeAlive -= dt;
            if(timeAlive <= 0)
                ToBeDeleted = true;
        }
    }
}
