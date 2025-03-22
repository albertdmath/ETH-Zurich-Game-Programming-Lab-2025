using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace src.GameObjects;
public class Swordfish : Projectile
{
    // Private fields:

    // Constructor:
    public Swordfish(ProjectileType type, Vector3 origin, Vector3 target,Model model, float scaling) : base(type, origin, target, model, scaling) {
        BaseVelocity = 2.0f;
        Velocity=BaseVelocity;
     }

    public override void Move(float dt)
    {
        Position += Velocity * Orientation * dt;
    }

    public override void Throw(float chargeUp)
        {
            base.Throw(chargeUp);
            Velocity += chargeUp;
        }
}
