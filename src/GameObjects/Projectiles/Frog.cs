using System;
using Microsoft.Xna.Framework;   
   
namespace src.GameObjects;
public class Frog : Projectile
{
    // Private fields:
    private const float HOP_TIME = 1000f; // 1 second in milliseconds
    private const int velocity = 1;
    private float timeBeforeHop = 0f;

    // Constructor:
    public Frog(int type, Vector3 origin, Vector3 target) : base(type, origin, target) { }

    public override void Move(float dt)
    {
        // timeBeforeHop += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
        // if (timeBeforeHop < HOP_TIME) return;
        // Maybe frog can sit still for one second. Too tired right now to figure out how to do this.
        this.position += velocity * orientation * dt;
        // Just a small trick to make the frog bounce, it's more visually appealing than teleporting frog:
        this.position.Y = (float)Math.Abs(Math.Sin(this.position.X));
        this.timeBeforeHop = 0f;
    }
}