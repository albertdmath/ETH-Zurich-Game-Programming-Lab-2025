using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;

namespace src.ObjectClasses
{
    public class RingOfDoom
    {
        /** 
        This is the class for the menacing mob. 
        **/
        private Random rng = new Random();
        private float radius;
        private Vector3 curr_center, end_center;
        private const float START_CLOSING_TIME = 60000f; // 1 minute in milliseconds
        private const float CLOSING_TIME = 60000f; // 1 minute in milliseconds
        private float initialRadius; // Store the initial radius

        public RingOfDoom(int plane_width, int plane_height)
        {
            // Get radius of the circle
            this.initialRadius = Math.Min(plane_width, plane_height) / 2 - 1;
            this.radius = this.initialRadius;

            // Assume the center of the plane is at (0, 0, 0)
            this.curr_center = Vector3.Zero;

            // Calculate a random point inside the circle
            float angle = (float)this.rng.NextDouble() * 2 * MathF.PI;
            float distance = this.initialRadius * MathF.Sqrt((float)this.rng.NextDouble());
            float x = distance * MathF.Cos(angle);
            float z = distance * MathF.Sin(angle);
            //Debug.WriteLine($"end_center, x: {x}, z: {z}");
            this.end_center = new Vector3(x, 0, z);
        }

        public void CloseRing(GameTime gameTime)
        {
            float totalTime = (float)gameTime.TotalGameTime.TotalMilliseconds;
            if (totalTime < START_CLOSING_TIME) return;

            float progress = (totalTime - START_CLOSING_TIME) / CLOSING_TIME;
            //Debug.WriteLine($"time: {gameTime.TotalGameTime.TotalMilliseconds}, progress: {progress}");
            // Linear Interpolate Center
            this.curr_center = Vector3.Lerp(Vector3.Zero, this.end_center, progress);
            // Decrease Radius
            this.radius = this.initialRadius * (1 - progress);
        }

        public Vector3 RndCircPoint()
        {
            //this is from where tho shoot the projectiles
            float angle = (float)this.rng.NextDouble() * 2 * MathF.PI;
            float x = curr_center.X + this.radius * MathF.Cos(angle);
            float z = curr_center.Z + this.radius * MathF.Sin(angle);

            return new Vector3(x, 0, z);
        }

        public void DrawRing(GraphicsDeviceManager graphics, SpriteBatch spriteBatch, Texture2D ringTexture)
        {
            //draw the ring
            spriteBatch.Begin();
            spriteBatch.Draw(ringTexture, new Vector2(this.curr_center.X, this.curr_center.Z), Color.White);
            spriteBatch.End();
        }
    }
}