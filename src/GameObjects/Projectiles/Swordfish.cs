using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace src.GameObjects;
public class Swordfish : Projectile
{
    // Private fields:

    // Constructor:
    public Swordfish(ProjectileType type, Vector3 origin, Vector3 target, Model model) : base(type, origin, target, model) 
    {
        Velocity = 2.0f;
    }

    public override void Move(float dt)
    {
        Position += Velocity * Orientation * dt;
    }

    public override void Throw(float chargeUp)
        {
            this.Position = Holder.Position + Holder.Orientation;
            this.Orientation = Holder.Orientation;
            this.Holder = null;
            Velocity += chargeUp;
        }
}
