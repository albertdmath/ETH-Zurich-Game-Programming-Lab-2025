using System;
using Microsoft.Xna.Framework;
using System.Linq;

namespace src.GameObjects;

public class Tomato : Projectile
{
    // Constants
    private const float HALF_GRAVITY = 4.9f; // Gravity effect
    private const float SQUARED_EXPLOSION_RADIUS = 0.8f; // Define the explosion radius
    private static readonly float angle = (float)Math.PI / 3; // angle of throw
    private static readonly float cos = (float)Math.Cos(angle), sin = (float)Math.Sin(angle);

    // Fields
    private float timeAlive;
    private Vector3 origin;

    // Constructor:
    public Tomato(ProjectileType type, Vector3 origin, Vector3 target, DrawModel model, float scaling, float height) : base(type, origin, target, model, scaling, height) { }

    private static float CalculateVelocity(Vector3 origin, Vector3 target)
    {
        // Calculate the horizontal distance (XZ-plane)
        float distance = Vector3.Distance(target, origin);

        // Calculate the initial velocity using the simplified formula
        return (float)Math.Sqrt((HALF_GRAVITY * distance) / (cos * sin));
    }

    protected override void Move(float dt)
    {
        timeAlive += dt;

        Vector3 horizontalMotion = Orientation * Velocity * cos;
        Vector3 verticalMotion = new Vector3(0, Velocity * sin - HALF_GRAVITY * timeAlive, 0);

        Position = origin + (horizontalMotion + verticalMotion) * timeAlive;
    }

    public override void OnGroundHit()
    {
        Position = new Vector3(Position.X, 0, Position.Z);
        Explode();
        base.OnGroundHit();
    }

    public override void OnPlayerHit(Player player)
    {
        Explode();
        base.OnPlayerHit(player);
    }

    private void Explode()
    {
        /* foreach (Player player in gameStateManager.players.Where(p => p.Life > 0))
        {
            if (Vector3.DistanceSquared(this.Position, player.Position) <= SQUARED_EXPLOSION_RADIUS)
                player.GetHit(this);
        } */
        gameStateManager.CreateAreaDamage(Position,1f,null,ProjectileType.Tomato);
    }

    public override void Throw(Vector3 origin, Vector3 target)
    {
        base.Throw(origin, target);
        Velocity = CalculateVelocity(origin, target);
        this.origin = origin;
        timeAlive = 0f;
    }
}
