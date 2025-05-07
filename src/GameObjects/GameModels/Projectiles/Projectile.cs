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
    Chicken,
    Barrel
}

/** Class for the projectiles **/
public class Projectile : GameModel
{
    // Projectile spawn probabilities (can be adjusted via UI)
    public readonly static Dictionary<ProjectileType, float> ProjectileProbability = new()
    {
        { ProjectileType.Banana, 0.0f },
        { ProjectileType.Coconut, 0.0f },
        { ProjectileType.Frog, 0.0f },
        { ProjectileType.Mjoelnir, 0.0f },
        { ProjectileType.Spear, 0.0f },
        { ProjectileType.Swordfish, 1.0f },
        { ProjectileType.Tomato, 0.0f },
        { ProjectileType.Turtle, 0.0f },
        { ProjectileType.Chicken, 0.0f },
        { ProjectileType.Barrel, 1.0f }
    };

    // Projectile properties
    public ProjectileType Type { get; private set; }
    public GameModel Holder { get; private set; } = null;
    public bool ToBeDeleted { get; set; } = false;
    public IndicatorModels IndicatorModel { get; private set; }

    protected static readonly GameStateManager gameStateManager = GameStateManager.GetGameStateManager();
    protected float velocity;

    private readonly float height;

    public Projectile(ProjectileType type, DrawModel model, float scaling, float height, IndicatorModels indicatorModel) : base(model, scaling) 
    {
        this.height = height;
        this.Type = type;
        this.IndicatorModel = indicatorModel; 
    }

    // Hit detection for the projectile
    public virtual void OnPlayerHit(Player player) 
    {             
        ToBeDeleted = ToBeDeleted || player.GetHit(this);  
    }

    public virtual void OnGroundHit()
    {
        ToBeDeleted = true;
    }

    public virtual void OnMobHit() {}

    public virtual void OnProjectileHit(Projectile projectile) {}

    // Catching and throwing the projectile
    public virtual void Catch(GameModel player) 
    { 
        Holder = player; 
    }

    public virtual void Throw(Vector3 target) 
    {
        Holder = null;
        Orientation = Vector3.Normalize(new Vector3(target.X, 0f, target.Z) - new Vector3(Position.X, 0f, Position.Z));
        Position += new Vector3(0f, height, 0f);
    }
 
    public virtual bool Action(float chargeUp, Vector3 aimPoint, bool isOutside) 
    {
        Throw(aimPoint);
        return true;
    }

    // Movement of the projectile
    protected virtual void Move(float dt) 
    {
        Position += velocity * Orientation * dt;
    }

    public override void Update(float dt)
    {
        if (Holder == null)
        {
            Move(dt);
        }
        else 
        {
            // Ensures projectile is held in right hand for a more realistic look:
            Vector3 orthogonalHolderOrientation = new(-Holder.Orientation.Z, Holder.Orientation.Y, Holder.Orientation.X);
            Position = Holder.Position + orthogonalHolderOrientation * 0.2f + new Vector3(0,0.2f,0);
            Orientation = Holder.Orientation;
        }
    }
}