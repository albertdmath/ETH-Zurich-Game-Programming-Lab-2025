using Assimp;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace src.GameObjects;
public enum ProjectileType
{
    Frog,
    Swordfish,
    Tomato,
    Coconut,
    Banana,
    Turtle,
    Spear,
    Mjoelnir,
    Chicken
}

/** Class for the projectiles **/
public class Projectile : GameModel
{
    // Projectile properties
    public ProjectileType Type { get; private set; }
    protected float Velocity { get; set; }
    public GameModel Holder { get; set; }
    protected GameStateManager gameStateManager;
    public bool ToBeDeleted { get; set; } = false;
    public bool DestroysOtherProjectiles { get; set; } = false;
    protected bool aimIndicatorIsArrow;

    public float Height {get; private set;}
    // Projectile spawn probabilities (can be adjusted via UI)
    public readonly static Dictionary<ProjectileType, float> ProjectileProbability = new()
    {
        { ProjectileType.Banana, 0.0f },
        { ProjectileType.Coconut, 0.5f },
        { ProjectileType.Frog, 0.0f },
        { ProjectileType.Mjoelnir, 0.0f },
        { ProjectileType.Spear, 0.0f },
        { ProjectileType.Swordfish, 0.0f },
        { ProjectileType.Tomato, 0.0f },
        { ProjectileType.Turtle, 0.0f },
        { ProjectileType.Chicken, 0.0f }
    };

    public Projectile(ProjectileType type, Vector3 origin, Vector3 target, DrawModel model, float scaling, float height) : base(model, scaling) 
    {
        Height = height;
        Type = type;
        gameStateManager = GameStateManager.GetGameStateManager();
        Throw(origin,target);
    }

    // Virtual methods for derived classes to override
    protected virtual void Move(float dt) { }

    public virtual void OnPlayerHit(Player player) 
    {             
        ToBeDeleted = ToBeDeleted || player.GetHit(this);  
    }

    public virtual void OnMobHit()
    {
        return;
    }
    public virtual void OnGroundHit()
    {
        ToBeDeleted = true;
    }

    public virtual void Throw(Vector3 origin, Vector3 target) 
    {
        this.Holder = null;
        Position = origin;
        Orientation = Vector3.Normalize(new Vector3(target.X, 0f, target.Z) - new Vector3(origin.X, 0f, origin.Z));
    }

    // Update the projectile's state
    public override void Update(float dt)
    {
        if (Holder == null)
        {
            Move(dt);
        }
        else 
        {
            // Ensures projectile is held in right hand for a more realistic look:
            Vector3 orthogonalHolderOrientation = new Vector3(-Holder.Orientation.Z, Holder.Orientation.Y, Holder.Orientation.X);
            Position = Holder.Position + orthogonalHolderOrientation * 0.2f + new Vector3(0,0.2f,0);
            Orientation = Holder.Orientation;
        }
    }

    // Catch the projectile
    public virtual void Catch(GameModel player) { Holder = player; }
 
    public virtual bool Action(float chargeUp, Vector3 aimPoint) {
        Throw(Position, aimPoint);
        // If it is thrown return true.
        return true;
    }
    public bool aimIndicator(){
        return aimIndicatorIsArrow;
    }
}

