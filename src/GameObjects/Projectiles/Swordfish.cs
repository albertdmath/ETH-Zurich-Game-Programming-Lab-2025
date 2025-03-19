using System;
using Microsoft.Xna.Framework;

namespace src.GameObjects;
public class Swordfish : Projectile
{
    // Private fields:
    private new const float velocity = 2.0f;

    // Constructor:
    public Swordfish(ProjectileType type, Vector3 origin, Vector3 target) : base(type, origin, target) { }

    public override void Move(float dt)
    {
        Position += velocity * Orientation * dt;
    }
}
