using System;
using Microsoft.Xna.Framework;

namespace src.GameObjects;

public class Banana : Projectile
{
    // Constants
    private const float HALF_GRAVITY = 4.9f;
    private const float SLIP_DURATION = 1.0f;
    private const float PLAYER_INERTIA = 1.0f;
    private static readonly float angle = MathF.PI / 3; // angle of throw
    private static readonly float cos = MathF.Cos(angle), sin = MathF.Sin(angle);

    // Fields
    private bool onGround = false;
    private float timeAlive;
    private Vector3 origin;
    

    // Constructor:
    public Banana(ProjectileType type, DrawModel model, float scaling, float height) : base(type, model, scaling, height, IndicatorModels.Target) {}

    public override void OnPlayerHit(Player player)
    {
        ToBeDeleted = ToBeDeleted || player.GetAffected(this);
        if(onGround) player.StunAndSlip(SLIP_DURATION, PLAYER_INERTIA);
    }

    public override void OnGroundHit(bool touching)
    {
        if(!touching) return;

        Position = new Vector3(Position.X, height, Position.Z);
        onGround = true;
    }

    private static float CalculateVelocity(Vector3 origin, Vector3 target)
    {
        float distance = Vector3.Distance(target, origin);
        return MathF.Sqrt(HALF_GRAVITY * distance / (cos * sin));
    }

    public override void Throw(Vector3 target) 
    {
        base.Throw(target);
        onGround = false;
        velocity = CalculateVelocity(Position, target);
        origin = Position;
        timeAlive = 0f;
    }

    protected override void Move(float dt)
    {
        if (onGround) return;

        timeAlive += dt;
        
        Vector3 horizontalMotion = Orientation * velocity * cos;
        Vector3 verticalMotion = new(0, velocity * sin - HALF_GRAVITY * timeAlive, 0);

        Position = origin + (horizontalMotion + verticalMotion) * timeAlive;
    }
}
