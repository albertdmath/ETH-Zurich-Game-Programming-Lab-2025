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
        { ProjectileType.Banana, 0.1f },
        { ProjectileType.Coconut, 0.1f },
        { ProjectileType.Frog, 0.1f },
        { ProjectileType.Mjoelnir, 0.1f },
        { ProjectileType.Spear, 0.1f },
        { ProjectileType.Swordfish, 0.1f },
        { ProjectileType.Tomato, 0.1f },
        { ProjectileType.Turtle, 0.1f },
        { ProjectileType.Chicken, 0.7f },
        { ProjectileType.Barrel, 0.1f }
    };

    // Projectile properties
    public ProjectileType Type { get; private set; }
    public GameModel Holder { get; protected set; } = null;
    public bool ToBeDeleted { get; set; } = false;
    public IndicatorModels IndicatorModel { get; private set; }

    protected static readonly GameStateManager gameStateManager = GameStateManager.GetGameStateManager();
    protected float velocity;

    protected readonly float height;

    public Projectile(ProjectileType type, DrawModel model, float scaling, float height, IndicatorModels indicatorModel, float radius = -1) : base(model, scaling, radius) 
    {
        this.height = height;
        this.Type = type;
        this.IndicatorModel = indicatorModel; 
    }

    // Hit detection for the projectile
    public virtual void OnPlayerHit(Player player) 
    {             
        ToBeDeleted = player.GetHit(this);  
    }

    public virtual void OnGroundHit(bool touching)
    {
        ToBeDeleted = touching;
    }

    public virtual void OnMobHit(Ellipse ellipse) {}

    public virtual void OnProjectileHit(Projectile projectile) {}

    // Catching and throwing the projectile
    public virtual bool Catch(GameModel player) 
    { 
        Holder = player;
        return true; 
    }

    public virtual void Throw(Vector3 target) 
    {
        Orientation = Vector3.Normalize(new Vector3(target.X, 0f, target.Z) - new Vector3(Holder.Position.X, 0f, Holder.Position.Z));
        Position += new Vector3(0f, height, 0f);
        Holder = null;
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
            Orientation = Holder.Orientation;
            Vector3 orthogonalHolderOrientation = new(-Orientation.Z, Orientation.Y, Orientation.X);
            Position = Holder.Position + orthogonalHolderOrientation * 0.2f + new Vector3(0,0.2f,0);
        }
    }
}