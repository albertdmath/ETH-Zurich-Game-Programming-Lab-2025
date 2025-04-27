using System;
using Accord.Math.Geometry;
using Microsoft.Xna.Framework;

//when you throw you are flying with it
namespace src.GameObjects;
public class Chicken : Projectile
{
    // Constants
    private const float MAX_VELOCITY = 20;
    private const float MAX_WALKING_VELOCITY = 5f;
    private const float MIN_VELOCITY = 2.5f;
    private const float MIN_STAND_STILL = 0.5f;
    private const float MAX_STAND_STILL = 3f;
    private const float FLIGHT_MAX_HEIGHT = 5f;
    private const float FLIGHT_MIN_HEIGHT = 2.5f;
    private const float FALLING_SPEED = 0.5f;
    
    private static readonly Random random = new();

    private float timeToStandStill = 0f;

    public float YCoordinate { get; private set;} = 0f;

    // Constructor:
    public Chicken(ProjectileType type, Vector3 origin, Vector3 target, DrawModel model, float scaling, float height) : base(type, origin, target, model, scaling, height) {}

    public override void Update(float dt)
    {
        if (Holder == null)
        {
            Move(dt);
        }
        else if (YCoordinate > 0)
        {
            Fly(dt);
        }
        else 
        {
            CarriedByPlayer();
        }
    }

    private void CarriedByPlayer()
    {
        Vector3 orthogonalHolderOrientation = new(-Holder.Orientation.Z, Holder.Orientation.Y, Holder.Orientation.X);
        Position = Holder.Position + orthogonalHolderOrientation * 0.2f + new Vector3(0,0.2f,0);
        Orientation = Holder.Orientation;
    }

    private void Fly(float dt)
    {
        YCoordinate -= dt * FALLING_SPEED;
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
            Velocity = MIN_VELOCITY + (MAX_WALKING_VELOCITY - MIN_VELOCITY) * (float)random.NextDouble();
            Orientation = Vector3.Normalize(new Vector3((float)(random.NextDouble() * 2.0 - 1.0), 0f, (float)(random.NextDouble() * 2.0 - 1.0)));
            timeToStandStill = MIN_STAND_STILL + (MAX_STAND_STILL - MIN_STAND_STILL) * (float)random.NextDouble();
        }  
    }

    private static float CalculateVelocity(Vector3 origin, Vector3 target)
    {
        // Calculate the horizontal distance (XZ-plane)
        float distance = Vector3.Distance(target, origin);

        // Calculate the initial velocity using the simplified formula
        return Math.Clamp(distance, MIN_VELOCITY, MAX_VELOCITY);
    }

    public override void Throw(Vector3 origin, Vector3 target) 
    {
        base.Throw(origin, target);
        Velocity = (Holder is Player) ? CalculateVelocity(origin, target) : MIN_VELOCITY;
    }

    public override bool Action(float chargeUp, Vector3 aimPoint)
    {
        (Holder as Player).FlyWithChicken();
        //this should be done linearly
        YCoordinate = MathHelper.Lerp(FLIGHT_MIN_HEIGHT, FLIGHT_MAX_HEIGHT, chargeUp/9f);
        return false;
    }
}
