using System;
using Accord.Math.Geometry;
using Microsoft.Xna.Framework;

//when you throw you are flying with it
namespace src.GameObjects;
public class Chicken : Projectile
{
    // Constants
    private const float MAX_THROW_VELOCITY = 15f;
    private const float MIN_THROW_VELOCITY = 2.5f;
    private const float MAX_WALKING_VELOCITY = 4f;
    private const float MIN_WALKING_VELOCITY = 2f;
    private const float MAX_STAND_STILL = 3f;
    private const float MIN_STAND_STILL = 0.5f;
    private const float FLIGHT_MAX_HEIGHT = 5f;
    private const float FLIGHT_MIN_HEIGHT = 2.5f;
    private const float FALLING_SPEED = 0.7f;
    private const float UPWARD_VELOCITY = 2.5f;
    
    private static readonly Random random = new();

    private float timeToStandStill = 0f;
    private bool upwards = false;
    private float targetHeight = 0f;
    public float YCoordinate { get; private set;} = 0f;

    private static float RngRange(float minNumber, float maxNumber)
    {
        return (float)random.NextDouble() * (maxNumber - minNumber) + minNumber;
    }

    // Constructor:
    public Chicken(ProjectileType type, Vector3 origin, Vector3 target, DrawModel model, float scaling, float height) : base(type, origin, target, model, scaling, height) {}

    public override void Update(float dt)
    {
        if (Holder == null)
            Move(dt);

        else if (targetHeight > 0)
            Fly(dt);

        else 
            CarriedByPlayer();
    }

    private void CarriedByPlayer()
    {
        Vector3 orthogonalHolderOrientation = new(-Holder.Orientation.Z, Holder.Orientation.Y, Holder.Orientation.X);
        Position = Holder.Position + orthogonalHolderOrientation * 0.2f + new Vector3(0,0.2f,0);
        Orientation = Holder.Orientation;
    }

    private void Fly(float dt)
    {
        if (YCoordinate >= targetHeight)
            upwards = false;
        
        YCoordinate += dt * (upwards ? UPWARD_VELOCITY : -FALLING_SPEED);
        Position = new(Holder.Position.X, YCoordinate+0.8f, Holder.Position.Z);
        Orientation = Holder.Orientation;
    }

    protected override void Move(float dt)
    {
        if((timeToStandStill -= dt) > 0f)
            return;
    
        if(random.NextDouble() < 0.995f)
        {
            Position += Velocity * Orientation * dt;
        }
        else
        {
            Velocity = RngRange(MIN_WALKING_VELOCITY, MAX_WALKING_VELOCITY);
            Orientation = Vector3.Normalize( new(RngRange(-1, 1), 0f, RngRange(-1, 1)) );
            timeToStandStill = RngRange(MIN_STAND_STILL, MAX_STAND_STILL);
        }  
    }

    private static float CalculateVelocity(Vector3 origin, Vector3 target)
    {
        float distance = Vector3.Distance(target, origin);
        return Math.Clamp(distance, MIN_THROW_VELOCITY, MAX_THROW_VELOCITY);
    }

    public override void Throw(Vector3 origin, Vector3 target) 
    {
        base.Throw(origin, target);
        Velocity = (Holder is Player) ? CalculateVelocity(origin, target) : MIN_THROW_VELOCITY;
    }

    public override bool Action(float chargeUp, Vector3 aimPoint)
    {
        (Holder as Player).FlyWithChicken();
        targetHeight = MathHelper.Lerp(FLIGHT_MIN_HEIGHT, FLIGHT_MAX_HEIGHT, chargeUp/9f);
        upwards = true;
        return false;
    }
}
