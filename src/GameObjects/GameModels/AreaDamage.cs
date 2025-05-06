using Microsoft.Xna.Framework;


namespace src.GameObjects;

public class AreaDamage : GameModel
{
    // Private fields:
    public float timeSinceCreation = 0f;
    private float scaleSquared;
    public bool ToBeDeleted { get; set; } = false;
    private Player player = null;
    
    public AreaDamage(Vector3 position, Player player, DrawModel model, float scale) : base(model,scale)
    {
        this.player = player;
        this.Position = position;
        scaleSquared = scale * scale * 0.25f;
    }

    // Checks if the player should receive damage
    public bool Intersects(Player player)
    {
        return player != this.player && Vector3.DistanceSquared(player.Position,Position) < scaleSquared && Hitbox.Intersects(player.Hitbox);
    }
    public override void Update(float dt)
    {
        timeSinceCreation += dt;
        ToBeDeleted  = (timeSinceCreation > 1.5f);
    }
}
