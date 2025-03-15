using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector3 = Microsoft.Xna.Framework.Vector3;

/*
    The idea for this class is, that the hitbox of a model consists of a set of bounding boxes.
    For the player we would have individual boxes for arms, legs, head etc. alltogether forming the hitbox.
*/
public class Hitbox {

    private List<BoundingBox> boundingBoxes;
    public void initializeHitbox(Model model) {

    
        foreach (ModelMesh mesh in model.Meshes)
        {
            Vector3 min = new Vector3(float.MaxValue);
            Vector3 max = new Vector3(float.MinValue);

            foreach (ModelMeshPart part in mesh.MeshParts)
            {
                // Get vertex data
                VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[part.VertexBuffer.VertexCount];
                part.VertexBuffer.GetData(vertices);

                // Update min and max bounds
                foreach (var vertex in vertices)
                {
                    min = Vector3.Min(min, vertex.Position);
                    max = Vector3.Max(max, vertex.Position);
                }
            }

            // Create and store the bounding box for this wall mesh
            BoundingBox box = new BoundingBox(min, max);
            boundingBoxes.Add(box);
        }
    }
}