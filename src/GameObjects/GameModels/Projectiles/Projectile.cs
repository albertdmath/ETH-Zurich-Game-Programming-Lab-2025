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
    TurtleWalking,
    Spear,
    Mjoelnir
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
    public float Height {get; private set;}
    // Projectile spawn probabilities (can be adjusted via UI)
    public static Dictionary<ProjectileType, float> ProjectileProbability = new Dictionary<ProjectileType, float>
    {
        { ProjectileType.Banana, 0.1f },
        { ProjectileType.Coconut, 0.1f },
        { ProjectileType.Frog, 0.1f },
        { ProjectileType.Mjoelnir, 0.2f },
        { ProjectileType.Spear, 0.2f },
        { ProjectileType.Swordfish, 0.3f },
        { ProjectileType.Tomato, 0.3f },
        { ProjectileType.Turtle, 0.2f }
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

    public virtual void Throw(float chargeUp) {
        this.Position = Holder.Position + Holder.Orientation;
        this.Orientation = Holder.Orientation;
        this.Holder = null;
        //Console.WriteLine("Projectile thrown with orientation: " + Orientation + " and speedup: " + chargeUp);
    }

    public virtual void Throw(Vector3 origin, Vector3 target) 
    {
        this.Holder = null;
        Position = origin;
        Orientation = Vector3.Normalize(target - origin);
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

    public bool Free() { return Holder == null; } 
    public virtual bool Action(float chargeUp) {
        Throw(chargeUp);
        // If it is thrown return true.
        return true;
    }
}

