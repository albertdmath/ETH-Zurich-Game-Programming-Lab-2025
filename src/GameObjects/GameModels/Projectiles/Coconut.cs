using System;
using Microsoft.Xna.Framework;

namespace src.GameObjects;

public class Coconut : Projectile
{
    // Constants
    private const float MAX_VELOCITY = 15;
    private const float MIN_VELOCITY = 3;
    private const int MAX_BOUNCES = 3;
    private const float TIME_BETWEEN_BOUNCES = 0.5f;

    // Fields
    private int _bounces;
    private float _timeSinceBounce;


    // Constructor:
    public Coconut(ProjectileType type, Vector3 origin, Vector3 target, DrawModel model, float scaling, float height) : base(type, origin, target, model, scaling, height, IndicatorModels.Arrow) {}

    protected override void Move(float dt)
    {
        _timeSinceBounce -= dt;
        Position += velocity * Orientation * dt;
    }

    private void Bounce()
    {
        // Otherwise bounce the coconut
        velocity *= 0.9f;
        // Generate a random angle between -30° and +30°
        float randomAngle = MathHelper.ToRadians(Rng.NextInt(150, 210));

        // Create a rotation matrix around the Y-axis
        Matrix rotationMatrix = Matrix.CreateRotationY(randomAngle);

        // Apply the rotation to the orientation vector
        Orientation = Vector3.Transform(Orientation, rotationMatrix);
    }

    public override void OnPlayerHit(Player player) 
    {             
        player.GetHit(this);
        
        if (_timeSinceBounce > 0 || !player.GetAffected(this)) 
            return;
    
        _bounces--;
        _timeSinceBounce = TIME_BETWEEN_BOUNCES;

        if (_bounces <= 0)
            ToBeDeleted = true;
        else 
            Bounce();
    }

    public override void OnMobHit()
    {
        if (_timeSinceBounce > 0) 
            return;
    
        _bounces--;
        _timeSinceBounce = TIME_BETWEEN_BOUNCES;

        if (_bounces <= 0) 
            return;

        Bounce();
    }

    private static float CalculateVelocity(Vector3 origin, Vector3 target)
    {
        float distance = Math.Abs(Vector3.Distance(origin, target)) * 3;
        return Math.Clamp(distance, MIN_VELOCITY, MAX_VELOCITY);
    }
    
    public override void Throw(Vector3 origin, Vector3 target) 
    {
        velocity = (Holder is Player) ? CalculateVelocity(origin, target) : MIN_VELOCITY;
        _bounces = MAX_BOUNCES;
        base.Throw(origin, target);
    }
}
