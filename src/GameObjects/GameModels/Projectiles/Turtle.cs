using System;
using Microsoft.Xna.Framework;
using System.Linq;

namespace src.GameObjects;

public class Turtle : Projectile
{
    // Constants
    //private const float TIME_TO_WEAR = 0.5f; //the time it takes the player to to wear the turtle
    private const float ROTATION_SPEED = 1.5f;
    private const float WALKING_VELOCITY = 0.7f;
    private const float MIN_VELOCITY = 2.0f;
    private const float MAX_VELOCITY = 20.0f;
    private const float BOUNCE_BACK_TIME = 0.3f;

    // Fields
    private float _bounceBackTime = 0f; // Time to transform from throwing to walking
    private readonly DrawModel walkingModel;
    private readonly DrawModel shellModel;
    // Fields

    // Constructor:
    public Turtle(ProjectileType type, Vector3 origin, Vector3 target, DrawModel model, DrawModel walkingModel, float scaling, float height) : base(type, origin, target, model, scaling, height, IndicatorModels.Arrow) 
    {
        this.shellModel = model;
        this.walkingModel = walkingModel;
    }

    private void RotateAway(float dt)
    {
        // Get nearest player
        Player nearestPlayer = gameStateManager.livingPlayers
            .OrderBy(player => Vector3.DistanceSquared(Position, player.Position))
            .First();

        //i need access to the ellipse

        // Desired direction toward the player
        Vector3 targetDirection = Vector3.Normalize(Position - nearestPlayer.Position);

        // Smoothly interpolate (lerp) away from the target direction
        Orientation = Vector3.Lerp(Orientation, targetDirection, ROTATION_SPEED * dt);
        //sometimes turtles fly around
        Orientation = new Vector3(Orientation.X, 0, Orientation.Z);
        Orientation.Normalize(); // Ensure it's a unit vector
    }

    private void BounceAfterHit()
    {
        velocity = WALKING_VELOCITY;
        _bounceBackTime = BOUNCE_BACK_TIME;
        Orientation *= -1;
        this.DrawModel = this.walkingModel;
        this.DrawModel = this.walkingModel;
    }

    public override void OnPlayerHit(Player player) 
    {    
        if (_bounceBackTime > 0) return;

        if(velocity == WALKING_VELOCITY)
        {
            base.OnPlayerHit(player);
        }
        else
        {
            player.GetHit(this);
            if(player.GetAffected(this))    
                BounceAfterHit();
        }
    }

    protected override void Move(float dt)
    {
        if((_bounceBackTime -= dt) > 0)
        {
            float jumpProgress = _bounceBackTime / BOUNCE_BACK_TIME;
            Position += MIN_VELOCITY * Orientation * dt;
            Position = new Vector3(Position.X, (float)Math.Sin(jumpProgress * Math.PI)*0.5f, Position.Z);
        }
        else
        {
            if (velocity == WALKING_VELOCITY) RotateAway(dt);
        
            Position += velocity * Orientation * dt;
        }
    }

    public override void Catch(GameModel player)
    {
        base.Catch(player);
        this.DrawModel = this.shellModel;
        this.DrawModel = this.shellModel;
    }

    public override bool Action(float chargeUp, Vector3 aimPoint)
    {
        if((Holder as Player).SetArmor())
        {
            (Holder as Player).Drop();
            return false;
        }
        else
        {
            base.Action(chargeUp, aimPoint);
            return true;
        }
    }

    private static float Calculatevelocity(Vector3 origin, Vector3 target)
    {
        float distance = Vector3.Distance(target, origin);
        return Math.Clamp(distance, MIN_VELOCITY, MAX_VELOCITY);
    }

    public override void Throw(Vector3 origin, Vector3 target) 
    {
        velocity = (Holder is Player) ? Calculatevelocity(origin, target) : MIN_VELOCITY;
        base.Throw(origin, target);
    }
}

