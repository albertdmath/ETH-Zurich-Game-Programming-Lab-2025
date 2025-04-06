using System;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Linq.Expressions;

namespace src.GameObjects;

public class Turtle : Projectile
{
    // Constants
    private const float TIME_TO_WEAR = 0.5f; //the time it takes the player to to wear the turtle
    private const float ROTATION_SPEED = 1.5f;
    private const float WALKING_VELOCITY = 0.7f;
    private const float THROW_VELOCITY = 2.0f;
    private const float BOUNCE_BACK_TIME = 0.3f;

    // Fields
    private float _bounceBackTime = 0f; // Time to transform from throwing to 
    private DrawModel catchModel;

    // Fields

    // Constructor:
    public Turtle(ProjectileType type, Vector3 origin, Vector3 target, DrawModel model, float scaling) : base(type, origin, target, model, scaling) 
    {
        catchModel = DrawModel;
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
        Velocity = WALKING_VELOCITY;
        _bounceBackTime = BOUNCE_BACK_TIME;
        Orientation *= -1;
        //this.DrawModel = this.walkingModel;
    }

    public override void OnPlayerHit(Player player) 
    {    
        if (ToBeDeleted) return; // If the projectile is already marked for deletion, do nothing

        if(Velocity == WALKING_VELOCITY)
        {
            if (_bounceBackTime > 0) return;

            base.OnPlayerHit(player);
        }
        else
        {
            player.GetHit(this);  
            BounceAfterHit();
        }
    }

    /*
    public override void OnMobHit()
    {
        //maybe a check of the orientation is not bad
        BounceAfterHit();
    }
    */
    

    protected override void Move(float dt)
    {
        if((_bounceBackTime -= dt) > 0)
        {
            float jumpProgress = _bounceBackTime / BOUNCE_BACK_TIME;
            Position += THROW_VELOCITY * Orientation * dt;
            Position = new Vector3(Position.X, (float)Math.Sin(jumpProgress * Math.PI)*0.5f, Position.Z);
        }
        else
        {
            if (Velocity == WALKING_VELOCITY) RotateAway(dt);
        
            Position += Velocity * Orientation * dt;
        }
    }

    public override void Catch(GameModel player)
    {
        base.Catch(player);
        this.DrawModel = this.catchModel;
    }

    public override void Throw(float chargeUp)
    {
        if (chargeUp < TIME_TO_WEAR) return;
        
        ToBeDeleted = true;
        (Holder as Player).SetArmor(true);
        this.Holder = null; 
    }

    public override void Throw(Vector3 origin, Vector3 target) 
    {
        base.Throw(origin, target);
        Velocity = THROW_VELOCITY;
    }
}

