using System;
using Microsoft.Xna.Framework;

namespace src.GameObjects;
public class Swordfish : Projectile
  {
      // Private fields:
      private const float velocity = 5;

      // Constructor:
      public Swordfish(int type, Vector3 origin, Vector3 target) : base(type, origin, target) { }

      public override void Move(float dt)
      {
          this.position += velocity * orientation * dt;
      }
  }