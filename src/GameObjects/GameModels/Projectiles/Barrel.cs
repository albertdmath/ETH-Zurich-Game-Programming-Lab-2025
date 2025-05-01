using System;
using Microsoft.Xna.Framework;
using System.Linq;

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
    private static bool model1IsActive = true; // Flag to toggle between models

    // Constructor:
    public Barrel(ProjectileType type, Vector3 origin, Vector3 target, DrawModel model1, DrawModel model2, float scaling, float height) : base(type, origin, target, model1, scaling, height) { 
        aimIndicatorIsArrow = false;
        this.DrawModel = model1IsActive ? model1 : model2;
        model1IsActive = !model1IsActive;
    }

    private static float CalculateVelocity(Vector3 origin, Vector3 target)
    {
        // Calculate the horizontal distance (XZ-plane)
        float distance = Vector3.Distance(target, origin);

        // Calculate the initial velocity using the simplified formula
        return (float)Math.Sqrt((HALF_GRAVITY * distance) / (cos * sin));
    }

    protected override void Move(float dt)
    {
        if (notMoving) return;

        timeAlive += dt;

        Vector3 horizontalMotion = Orientation * Velocity * cos;
        Vector3 verticalMotion = new Vector3(0, Velocity * sin - HALF_GRAVITY * timeAlive, 0);

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
        if (!notMoving) 
            base.OnPlayerHit(player);
    }

    public override void Throw(Vector3 origin, Vector3 target)
    {
        base.Throw(origin, target);
        notMoving = false;
        Velocity = CalculateVelocity(origin, target);
        this.origin = origin;
        timeAlive = 0f;
    }
}
