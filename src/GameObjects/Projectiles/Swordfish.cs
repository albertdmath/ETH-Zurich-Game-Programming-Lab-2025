using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace src.GameObjects;
public class Swordfish : Projectile
{
    // Private fields:

    // Constructor:
    public Swordfish(ProjectileType type, Vector3 origin, Vector3 target,Model model) : base(type, origin, target, model) {
        baseVelocity = 2.0f;
        velocity=baseVelocity;
     }

    public override void Move(float dt)
    {
        Position += velocity * Orientation * dt;
    }
}
