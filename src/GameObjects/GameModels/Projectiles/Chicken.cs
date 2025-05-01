using System;
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

    private float timeToStandStill = 0f;
    private bool upwards = false;
    private float targetHeight = 0f;
    public float YCoordinate { get; private set;} = 0f;
     

    // Constructor:
    public Chicken(ProjectileType type, DrawModel model, float scaling, float height) : base(type, model, scaling, height, IndicatorModels.Arrow) 
    {
        this.velocity = MIN_THROW_VELOCITY;
    }

    public override bool Action(float chargeUp, Vector3 aimPoint, bool isOutside)
    {
        if(isOutside)
        {
            velocity =  MathHelper.Lerp(MIN_THROW_VELOCITY, MAX_THROW_VELOCITY, chargeUp);
            base.Throw(aimPoint);
            return true;
        }
        else
        {
            (Holder as Player).FlyWithChicken();
            targetHeight = MathHelper.Lerp(FLIGHT_MIN_HEIGHT, FLIGHT_MAX_HEIGHT, chargeUp);
            upwards = true;
            return false;
        }
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
    
        if(Rng.NextBool(0.995f))
        {
            Position += velocity * Orientation * dt;
        }
        else
        {
            velocity = Rng.NextFloat(MIN_WALKING_VELOCITY, MAX_WALKING_VELOCITY);
            Orientation = Vector3.Normalize( new(Rng.NextFloat(-1, 1), 0f, Rng.NextFloat(-1, 1)) );
            timeToStandStill = Rng.NextFloat(MIN_STAND_STILL, MAX_STAND_STILL);
        }  
    }

    public override void Update(float dt)
    {
        if (targetHeight > 0)
            Fly(dt);
        else
            base.Update(dt);
    }
}
