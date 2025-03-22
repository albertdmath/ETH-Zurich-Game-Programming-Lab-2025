using Microsoft.Xna.Framework;
using System;

namespace src.GameObjects
{
    /** Represents an ellipse in 2D space with a center, major axis (a), and minor axis (b). **/
    public class Ellipse
    {
        private float a; // Major axis
        private float b; // Minor axis
        private Vector3 center; // Center of the ellipse

        // Constructor
        public Ellipse(float a, float b, Vector3 center)
        {
            this.a = a;
            this.b = b;
            this.center = center;
        }

        // Method returns true if the point (x, y) is inside the ellipse
        public bool Inside(float x, float y)
        {
            // Translate the point relative to the ellipse's center
            float translatedX = x - center.X;
            float translatedY = y - center.Z; // Using Z for 2D Y-axis

            // Check if the point is inside the ellipse
            return (translatedX * translatedX) / (a * a) + (translatedY * translatedY) / (b * b) <= 1f;
        }

        // Method to set new major and minor axes
        public void Set(float a, float b, Vector3 center)
        {
            this.a = a;
            this.b = b;
            this.center = center;
        }

        // Method to calculate the tangent vector at a point (x, y) on the ellipse
        private Vector3 Tangent(float x, float y)
        {
            // Translate the point relative to the ellipse's center
            float translatedX = x - center.X;
            float translatedY = y - center.Z;

            return (translatedY == 0f) 
                ? new Vector3(0f, 0f, 1f) 
                : new Vector3(1f, 0f, -1f * b * b * translatedX / (a * a * translatedY));
        }

        // Method to calculate the normal vector at a point (x, y) on the ellipse
        public Vector3 Normal(float x, float y)
        {
            // Translate the point relative to the ellipse's center
            float translatedX = x - center.X;
            float translatedY = y - center.Z;

            if (translatedX == 0f)
            {
                return new Vector3(0f, 0f, 1f) * (translatedY > 0f ? -1f : 1f); // Vertical normal
            }
            else
            {
                // Calculate the normal vector
                float temp = (translatedX < 0) ? 1f : -1f;
                return Vector3.Normalize(new Vector3(temp, 0f, temp * (a * a * translatedY) / (b * b * translatedX)));
            }
        }

        // Method to calculate the tangent component of a vector at a point (x, y) on the ellipse
        public Vector3 TangentPart(float x, float y, Vector3 orientation)
        {
            Vector3 tangent = Tangent(x, y);
            return Vector3.Dot(orientation, tangent) * tangent;
        }
    }
}