using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace src.GameObjects;
public class Swordfish : Projectile
{
    // Private fields:
    private new const float velocity = 2.0f;

    // Constructor:
    public Swordfish(ProjectileType type, Vector3 origin, Vector3 target,Model model) : base(type, origin, target, model) { }

    public override void Move(float dt)
    {
        Position += velocity * Orientation * dt;
        Position = new Vector3(Position.X, 0.1f, Position.Z);
    }
}
