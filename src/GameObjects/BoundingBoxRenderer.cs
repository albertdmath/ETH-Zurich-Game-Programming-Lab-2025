using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
public static class BoundingBoxRenderer
{
    private static VertexBuffer vertexBuffer;
    private static BasicEffect effect;
    private static bool initialized = false;

    public static void DrawOBB(GraphicsDevice graphicsDevice, OrientedBoundingBox obb, Matrix view, Matrix projection)
    {
        if (!initialized)
            Initialize(graphicsDevice);

        // Compute the 8 corners of the OBB
        Vector3[] corners = GetOBBCorners(obb);

        // Define the indices for drawing the edges
        short[] indices = {
            0, 1, 1, 3, 3, 2, 2, 0, // Bottom face edges
            4, 5, 5, 7, 7, 6, 6, 4, // Top face edges
            0, 4, 1, 5, 3, 7, 2, 6  // Connecting edges
        };

        // Create vertices
        VertexPositionColor[] vertices = new VertexPositionColor[corners.Length];
        for (int i = 0; i < corners.Length; i++)
            vertices[i] = new VertexPositionColor(corners[i], Color.Red);

        // Update vertex buffer
        vertexBuffer.SetData(vertices);

        // Apply effect
        effect.World = Matrix.Identity;
        effect.View = view;
        effect.Projection = projection;
        effect.CurrentTechnique.Passes[0].Apply();

        // Draw the wireframe box
        graphicsDevice.SetVertexBuffer(vertexBuffer);
        graphicsDevice.DrawUserIndexedPrimitives(
            PrimitiveType.LineList, vertices, 0, vertices.Length, indices, 0, indices.Length / 2);
    }

    private static void Initialize(GraphicsDevice graphicsDevice)
    {
        effect = new BasicEffect(graphicsDevice)
        {
            VertexColorEnabled = true
        };

        vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionColor),
            8, BufferUsage.WriteOnly);

        initialized = true;
    }

    private static Vector3[] GetOBBCorners(OrientedBoundingBox obb)
    {
        Vector3 center = obb.Center;
        Vector3 extents = obb.Extents;
        Matrix orientation = obb.Orientation;

        Vector3[] localCorners = {
            new Vector3(-extents.X, -extents.Y, -extents.Z),
            new Vector3( extents.X, -extents.Y, -extents.Z),
            new Vector3(-extents.X,  extents.Y, -extents.Z),
            new Vector3( extents.X,  extents.Y, -extents.Z),
            new Vector3(-extents.X, -extents.Y,  extents.Z),
            new Vector3( extents.X, -extents.Y,  extents.Z),
            new Vector3(-extents.X,  extents.Y,  extents.Z),
            new Vector3( extents.X,  extents.Y,  extents.Z)
        };

        Vector3[] worldCorners = new Vector3[8];
        for (int i = 0; i < 8; i++)
            worldCorners[i] = Vector3.Transform(localCorners[i], orientation) + center;

        return worldCorners;
    }
}