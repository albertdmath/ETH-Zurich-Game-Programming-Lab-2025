using System.Collections.Generic;
using Microsoft.Xna.Framework;

/*
    The idea for this class is, that the hitbox of a model consists of a set of bounding boxes.
    For the player we would have individual boxes for arms, legs, head etc. alltogether forming the hitbox.
*/
public class OBB : Hitbox 
{

    // Axis-aligned bounding boxes
    public List<OrientedBoundingBox> BoundingBoxes { get; private set; } = new List<OrientedBoundingBox>();

    public OBB(DrawModel model, Matrix transformation) 
    {
        foreach(GameMesh mesh in model.meshes) 
        {
            BoundingBoxes.Add(OrientedBoundingBox.ComputeOBB(mesh, transformation));
        }
    }

    public bool Intersects(Hitbox other)
    {
        return other.IntersectsWith(this);
    }

    public bool IntersectsWith(OBB other) 
    {
        foreach(OrientedBoundingBox box in BoundingBoxes) 
        {
            foreach(OrientedBoundingBox otherBox in other.BoundingBoxes) 
            {
                if (otherBox.Intersects(box)) 
                    return true;
            }
        }
        return false;
    }

    public bool IntersectsWith(Sphere other) 
    {
        foreach(OrientedBoundingBox box in BoundingBoxes) 
        {
            if (box.Intersects(other.BoundingSphere)) 
                return true;
        }
        return false;
    }

    public void Transform(Matrix transformation) {
        foreach(OrientedBoundingBox box in BoundingBoxes) {
            box.Transform(transformation);
        }
    }
}