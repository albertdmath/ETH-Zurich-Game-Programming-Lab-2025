using Microsoft.Xna.Framework;


namespace src.GameObjects
{
    public class Market : GameModel
    {
        public ProjectileType Type { get; private set; }

        public Market(Vector3 position, ProjectileType type, DrawModel model, float scaling) : base(model, scaling)
        {
            this.Position = position;
            this.Orientation = Vector3.Normalize(-Position);
            this.Type = type;
            this.updateHitbox();
        }
    }
}