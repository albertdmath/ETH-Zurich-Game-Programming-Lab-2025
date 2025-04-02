using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/*
    The idea for this class is, that the hitbox of a model consists of a set of bounding boxes.
    For the player we would have individual boxes for arms, legs, head etc. alltogether forming the hitbox.
*/
public class Hitbox {

    // Axis-aligned bounding boxes
    public List<OrientedBoundingBox> BoundingBoxes { get; } = new List<OrientedBoundingBox>();

    public Hitbox(Model model, Matrix transformation) {
        foreach(ModelMesh mesh in model.Meshes) {
            BoundingBoxes.Add(OrientedBoundingBox.ComputeOBB(mesh, transformation));
        }
    }

    public bool Intersects(Hitbox other) {
        foreach(OrientedBoundingBox box in BoundingBoxes) {
            foreach(OrientedBoundingBox otherBox in other.BoundingBoxes) {
                if (otherBox.Intersects(box)) {
                    return true;
                }
            }
        }
        return false;
    }

    public void Transform(Matrix transformation) {
        foreach(OrientedBoundingBox box in BoundingBoxes) {
            box.Transform(transformation);
        }
    }

    public void DebugDraw(GraphicsDevice device, Matrix view, Matrix projection) {
        foreach(OrientedBoundingBox box in BoundingBoxes) {
            BoundingBoxRenderer.DrawOBB(device, box, view, projection);
        }
    } 

}