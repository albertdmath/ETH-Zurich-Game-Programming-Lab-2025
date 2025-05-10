using Microsoft.Xna.Framework;

/*
    The idea for this class is, that the hitbox of a model consists of a set of bounding boxes.
    For the player we would have individual boxes for arms, legs, head etc. alltogether forming the hitbox.
*/

public class Sphere : Hitbox {

    // Axis-aligned bounding boxes
    public Vector3 Center { get; private set; }
    public float Radius { get; set; }

    public Sphere(Vector3 center, float radius) 
    {
        this.Center = center;
        this.Radius = radius;
    }

    public bool Intersects(Hitbox other)
    {
        return other.IntersectsWith(this);
    }

    public bool IntersectsWith(OBB other) 
    {
        foreach(OrientedBoundingBox box in other.BoundingBoxes) 
        {
            if (box.Intersects(this)) 
                return true;
        }
        return false;
    }

    public bool IntersectsWith(Sphere other) 
    {
        return Vector3.DistanceSquared(this.Center, other.Center) <= (this.Radius + other.Radius) * (this.Radius + other.Radius);
    }

    public void Transform(Vector3 position) 
    {
        this.Center = position;
    }
}