using System;
using System.Runtime.CompilerServices;
using Accord.Math.Optimization;
using Microsoft.Xna.Framework;

namespace src.GameObjects;

public class Coconut : Projectile
{
    // Constants
    private const float MAX_VELOCITY = 15;
    private const float MIN_VELOCITY = 3;
    private const int MAX_BOUNCES = 3;
    private const float TIME_BETWEEN_BOUNCES = 0.5f;
    private const float RADIUS = 0.25f; // Radius of the coconut

    // Fields
    private int bounces;
    private float timeSinceBounce;
    private bool isInside = false;


    // Constructor:
    public Coconut(ProjectileType type, DrawModel model, float scaling, float height) : base(type, model, scaling, height, IndicatorModels.Arrow, RADIUS) 
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

    public override void OnMobHit(Ellipse ellipse)
    {
        bool isInsideEll = ellipse.Inside(Position.X, Position.Z);
        // From outside to inside
        if(!isInside && isInsideEll)
            isInside = true;

        //From inside to outside
        else if(!isInsideEll && isInside && timeSinceBounce <= 0 && bounces >= 1)
        {
            bounces--;
            timeSinceBounce = TIME_BETWEEN_BOUNCES;
            Bounce();
        }
    }

    public override void Throw(Vector3 target)
    {
        bounces = MAX_BOUNCES;
        base.Throw(target);
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
