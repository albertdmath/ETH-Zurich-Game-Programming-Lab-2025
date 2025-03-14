using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace src.ObjectClasses
{
    public class RingOfDoom
    {
        /** 
        This is the class for the menacing mob. 
        **/
        private float initialRadius, radius;
        private Vector3 currCenter, endCenter;
        private static Random rng = new Random();
        private const float START_CLOSING_TIME = 60000f; // 1 minute in milliseconds
        private const float CLOSING_TIME = 60000f; // 1 minute in milliseconds

        public RingOfDoom(int planeWidth, int planeHeight)
        {
            // Get radius of the circle
            this.initialRadius = Math.Min(planeWidth, planeHeight) / 2 - 1;
            this.radius = this.initialRadius;

            // Assume the center of the plane is at (0, 0, 0)
            this.currCenter = Vector3.Zero;

            // Calculate a random point inside the circle
            float angle = (float) rng.NextDouble() * 2 * MathF.PI;
            float distance = this.initialRadius * MathF.Sqrt((float) rng.NextDouble());
            float x = distance * MathF.Cos(angle);
            float z = distance * MathF.Sin(angle);
            this.endCenter = new Vector3(x, 0, z);
        }

        public void CloseRing(GameTime gameTime)
        {
            float totalTime = (float)gameTime.TotalGameTime.TotalMilliseconds;
            if (totalTime < START_CLOSING_TIME) return;

            float progress = (totalTime - START_CLOSING_TIME) / CLOSING_TIME;
            // Linear Interpolate Center
            this.currCenter = Vector3.Lerp(Vector3.Zero, this.endCenter, progress);
            // Decrease Radius
            this.radius = this.initialRadius * (1 - progress);
        }
        //this is from where tho shoot the projectiles
        public Vector3 RndCircPoint()
        {
            float angle = (float) rng.NextDouble() * 2 * MathF.PI;
            float x = currCenter.X + this.radius * MathF.Cos(angle);
            float z = currCenter.Z + this.radius * MathF.Sin(angle);

            return new Vector3(x, 0, z);
        }
        //clamp the players position to the ring
        public Vector3 RingClamp(Vector3 playerPosition)
        {
            Vector3 offset = playerPosition - currCenter;

            if (offset.Length() <= this.radius) return playerPosition;

            Vector3 clampedPosition = currCenter + Vector3.Normalize(offset) * (radius - 1);
            return clampedPosition;
        }

        public void DrawRing(SpriteBatch spriteBatch, Texture2D ringTexture)
        {
            // Draw the ring
        }
    }
}