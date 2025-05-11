using System;
using Microsoft.Xna.Framework;
using System.Linq;

namespace src.GameObjects;

public class Frog : Projectile
{
    // Constants
    private const float HOP_TIME = 1.0f;
    private const float ROTATION_SPEED = 3.0f;
    private const float THROW_ANGLE = MathF.PI / 3;
    private const float HALF_GRAVITY = 4.9f;
    private const float WALKING_VELOCITY = 0.7f;

    // Fields
    private float timeAlive;
    private bool beingThrown = false;
    private Vector3 origin;

    // Constructor:
    public Frog(ProjectileType type, DrawModel model, float scaling, float height) : base(type, model, scaling, height, IndicatorModels.Target) {}

    public override void Throw(Vector3 target) {
        origin = Position;
        beingThrown = true;
        velocity = CalculateVelocity(Position, target);
        timeAlive = 0f;
        base.Throw(target);
    }

    public override bool Action(float chargeUp, Vector3 aimPoint, bool isOutside)
    {
        var player = Holder as Player;

        if (isOutside || !player.GainLife())
        {
            Throw(aimPoint);
            return true;
        }

        player.Drop();
        return false;
    }
    
    protected override void Move(float dt)
    {
        timeAlive += dt;

        if (beingThrown)
            ThrownMove();
        else
            HopMove(dt);
    }

    private void ThrownMove()
    {
        SwitchAnimation(0, true, 0.1f);
        // Calculate horizontal and vertical motion

        Vector3 horizontalMotion = Orientation * velocity * MathF.Cos(THROW_ANGLE);
        Vector3 verticalMotion = new Vector3(0, velocity * MathF.Sin(THROW_ANGLE) - HALF_GRAVITY * timeAlive, 0);

        // Update position
        Position = origin + (horizontalMotion + verticalMotion) * timeAlive;

        // Check if the frog has landed
        if (Position.Y < 0)
        {

            beingThrown = false;
            timeAlive = 0f;
            velocity = WALKING_VELOCITY;
            Position = new Vector3(Position.X, 0, Position.Z);
        }
    }

    private void HopMove(float dt)
    {
        if (timeAlive < HOP_TIME)
            TurnToPlayer(dt);
        else
            Hop(dt);

        // Reset the time before the next hop
        if (timeAlive > 2 * HOP_TIME)
            timeAlive = 0f;
    }

    private void TurnToPlayer(float dt)
    {
        SwitchAnimation(0, true, 0.1f);
        Player nearestPlayer = gameStateManager.livingPlayers
            .OrderBy(player => Vector3.DistanceSquared(Position, player.Position))
            .First();

        // Desired direction toward the player
        Vector3 targetDirection = Vector3.Normalize(nearestPlayer.Position - Position);

        // Smoothly interpolate (lerp) towards the target direction
        Orientation = Vector3.Lerp(Orientation, targetDirection, ROTATION_SPEED * dt);
        Orientation.Normalize(); // Ensure it's a unit vector
    }

    private void Hop(float dt)
    {
        SwitchAnimation(1, false, 1.0f, 0.0f, 1.5f);
        float jumpProgress = (timeAlive - HOP_TIME) / HOP_TIME;
        float position_y = MathF.Max(0, MathF.Sin(jumpProgress * MathF.PI));

        // Update the position of the frog
        Position += velocity * Orientation * dt;
        Position = new Vector3(Position.X, position_y, Position.Z);
    }

    private static float CalculateVelocity(Vector3 origin, Vector3 target)
    {
        float distance = Vector3.Distance(target, origin);
        return MathF.Sqrt(HALF_GRAVITY * distance / (MathF.Cos(THROW_ANGLE) * MathF.Sin(THROW_ANGLE)));
    }
}
