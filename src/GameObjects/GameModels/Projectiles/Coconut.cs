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
    private int bounces = MAX_BOUNCES;
    private float timeSinceBounce;


    // Constructor:
    public Coconut(ProjectileType type, DrawModel model, float scaling, float height) : base(type, model, scaling, height, IndicatorModels.Arrow) 
    {
        this.velocity = MIN_VELOCITY;
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
        
        if (timeSinceBounce > 0 || !player.GetAffected(this)) 
            return;
    
        bounces--;
        timeSinceBounce = TIME_BETWEEN_BOUNCES;

        if (bounces <= 0)
            ToBeDeleted = true;
        else 
            Bounce();
    }

    public override void OnMobHit()
    {
        if (timeSinceBounce > 0) 
            return;
    
        bounces--;
        timeSinceBounce = TIME_BETWEEN_BOUNCES;

        if (bounces <= 0) 
            return;

        Bounce();
    }

    public override bool Action(float chargeUp, Vector3 aimPoint, bool isOutside) 
    {
        velocity = MathHelper.Lerp(MIN_VELOCITY, MAX_VELOCITY, chargeUp);
        Throw(aimPoint);
        return true;
    }

    protected override void Move(float dt)
    {
        base.Move(dt);
        timeSinceBounce -= dt;
    }
}
