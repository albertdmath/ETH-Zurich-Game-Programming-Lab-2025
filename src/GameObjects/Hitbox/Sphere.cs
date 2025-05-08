using Microsoft.Xna.Framework;

/*
    The idea for this class is, that the hitbox of a model consists of a set of bounding boxes.
    For the player we would have individual boxes for arms, legs, head etc. alltogether forming the hitbox.
*/

public class Sphere : Hitbox {

    // Axis-aligned bounding boxes
    public BoundingSphere BoundingSphere { get; private set;}

    public Sphere(Matrix transformation) 
    {
        BoundingSphere = new BoundingSphere(Vector3.Zero, 1f);
        BoundingSphere.Transform(transformation);
    }

    public bool Intersects(Hitbox other)
    {
        return other.IntersectsWith(this);
    }

    public bool IntersectsWith(OBB other) 
    {
        foreach(OrientedBoundingBox box in other.BoundingBoxes) 
        {
            if (box.Intersects(BoundingSphere)) 
                return true;
        }
        return false;
    }

    public bool IntersectsWith(Sphere other) 
    {
        return other.BoundingSphere.Intersects(BoundingSphere);
    }

    public void Transform(Matrix transformation) 
    {
        BoundingSphere.Transform(transformation);
    }
}