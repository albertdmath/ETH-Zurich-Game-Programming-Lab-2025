using System;
using Microsoft.Xna.Framework;
using System.Linq;

namespace src.GameObjects;

public class Turtle : Projectile
{
    // Constants
    private const float ROTATION_SPEED = 1.5f;
    private const float WALKING_VELOCITY = 0.7f;
    private const float MIN_VELOCITY = 2.0f;
    private const float MAX_VELOCITY = 20.0f;
    private const float BOUNCE_BACK_TIME = 0.3f;

    // Fields
    private float bounceBackTime = 0f; // Time to transform from throwing to walking
    private readonly DrawModel walkingModel;
    private readonly DrawModel shellModel;

    // Constructor:
    public Turtle(ProjectileType type, DrawModel model, DrawModel walkingModel, float scaling, float height) : base(type, model, scaling, height, IndicatorModels.Arrow) 
    {
        this.shellModel = model;
        this.walkingModel = walkingModel;
        this.velocity = MIN_VELOCITY;
    }

    private void BounceAfterHit()
    {
        velocity = WALKING_VELOCITY;
        bounceBackTime = BOUNCE_BACK_TIME;
        Orientation *= -1;
        DrawModel = walkingModel;
    }

    public override void OnPlayerHit(Player player) 
    {    
        if (bounceBackTime > 0) return;

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

    public override void Catch(GameModel player)
    {
        base.Catch(player);
        DrawModel = shellModel;
    }

    public override bool Action(float chargeUp, Vector3 aimPoint, bool isOutside)
    {
        if(isOutside || !(Holder as Player).SetArmor())
        {
            velocity = MathHelper.Lerp(MIN_VELOCITY, MAX_VELOCITY, chargeUp);
            base.Throw(aimPoint);
            return true;
        }

        (Holder as Player).Drop();
        return false;
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
        
        Orientation = Vector3.Normalize(new(Orientation.X, 0, Orientation.Z));
    }

    protected override void Move(float dt)
    {
        if((bounceBackTime -= dt) > 0)
        {
            float jumpProgress = bounceBackTime / BOUNCE_BACK_TIME;
            Position += MIN_VELOCITY * Orientation * dt;
            Position = new Vector3(Position.X, MathF.Sin(jumpProgress * MathF.PI)*0.5f, Position.Z);
        }
        else
        {
            if (velocity == WALKING_VELOCITY) RotateAway(dt);
        
            base.Move(dt);
        }
    }
}

