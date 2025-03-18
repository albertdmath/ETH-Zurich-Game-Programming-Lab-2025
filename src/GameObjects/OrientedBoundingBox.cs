using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Vector3 = Microsoft.Xna.Framework.Vector3;
using Matrix = Microsoft.Xna.Framework.Matrix;
using Accord.Math.Decompositions;
using System.Numerics;
public class OrientedBoundingBox
{

    public Vector3 Center { get; private set; }
    public Vector3 Extents { get; private set; }
    public Matrix Orientation { get; private set; }
    public OrientedBoundingBox(Vector3 center, Vector3 extents, Matrix orientation)
    {
        Center = center;
        Extents = extents;
        Orientation = orientation;
    }

    // Returns a list of vertices of the given mesh
    private static List<Vector3> GetMeshVertices(ModelMesh mesh)
    {
        List<Vector3> vertices = new List<Vector3>();
        foreach (ModelMeshPart part in mesh.MeshParts)
        {
            VertexPositionNormalTexture[] vertexData = new VertexPositionNormalTexture[part.NumVertices];
            part.VertexBuffer.GetData(vertexData);

            foreach (var vertex in vertexData)
                vertices.Add(vertex.Position);
        }
        return vertices;
    }

    /* 
        This is a little complex, basically it finds the center of a mesh and 
        then does PCA to find the orientation of the most significant 3 axis in the point cloud
        Probs to a few parts of this go to GenAI who helped me put the individual parts together, 
        the Accord library does a very good job at eigendecomposition;
        it also computes the halfextents (so the distances from the center to the border of the box)
    */
    public static OrientedBoundingBox ComputeOBB(ModelMesh mesh, Matrix transformation)
    {
        List<Vector3> vertices = GetMeshVertices(mesh);
        if (vertices == null || vertices.Count == 0)
            throw new ArgumentException("No vertices provided!");

        // Compute mean (center of mass)
        Vector3 mean = Vector3.Zero;
        foreach (var v in vertices)
            mean += v;
        mean /= vertices.Count;

        // Construct 3x3 covariance matrix
        double[,] covarianceMatrix = new double[3, 3];

        foreach (var v in vertices)
        {
            Vector3 d = v - mean;
            covarianceMatrix[0, 0] += d.X * d.X;
            covarianceMatrix[0, 1] += d.X * d.Y;
            covarianceMatrix[0, 2] += d.X * d.Z;
            covarianceMatrix[1, 0] += d.Y * d.X;
            covarianceMatrix[1, 1] += d.Y * d.Y;
            covarianceMatrix[1, 2] += d.Y * d.Z;
            covarianceMatrix[2, 0] += d.Z * d.X;
            covarianceMatrix[2, 1] += d.Z * d.Y;
            covarianceMatrix[2, 2] += d.Z * d.Z;
        }

        // Compute eigenvectors using Accord.NET
        var evd = new EigenvalueDecomposition(covarianceMatrix);
        double[,] eigenvectors = evd.Eigenvectors;

        // Convert eigenvectors to Xna Vector3
        Vector3[] axes = new Vector3[3];
        for (int i = 0; i < 3; i++)
        {
            axes[i] = new Vector3(
                (float)eigenvectors[0, i],
                (float)eigenvectors[1, i],
                (float)eigenvectors[2, i]
            );
        }

        // Ensure the axes are properly oriented
        axes[0] = Vector3.Normalize(axes[0]);
        axes[1] = Vector3.Normalize(axes[1]);
        axes[2] = Vector3.Normalize(axes[2]);

        // Check for near-axis alignment (fixes small rotations)
        if (Math.Abs(Vector3.Dot(axes[0], Vector3.UnitX)) > 0.9999f) axes[0] = Vector3.UnitX;
        if (Math.Abs(Vector3.Dot(axes[1], Vector3.UnitY)) > 0.9999f) axes[1] = Vector3.UnitY;
        if (Math.Abs(Vector3.Dot(axes[2], Vector3.UnitZ)) > 0.9999f) axes[2] = Vector3.UnitZ;

        // Create Monogame rotation matrix
        Matrix rotation = new Matrix(
            axes[0].X, axes[0].Y, axes[0].Z, 0,
            axes[1].X, axes[1].Y, axes[1].Z, 0,
            axes[2].X, axes[2].Y, axes[2].Z, 0,
            0, 0, 0, 1
        );

        // Transform vertices into the new basis
        List<Vector3> transformed = new List<Vector3>();
        Matrix rotationTranspose = Matrix.Transpose(rotation);
        foreach (var v in vertices)
            transformed.Add(Vector3.Transform(v - mean, rotationTranspose));

        // Compute min/max extents
        Vector3 min = new Vector3(float.MaxValue);
        Vector3 max = new Vector3(float.MinValue);
        foreach (var v in transformed)
        {
            min = Vector3.Min(min, v);
            max = Vector3.Max(max, v);
        }

        // Compute the correct center and extents
        Vector3 obbCenter = (max + min) * 0.5f;  // Center in local space
        Vector3 extents = (max - min) * 0.5f;    // Half extents

        // Transform the center back to world space
        obbCenter = Vector3.Transform(obbCenter, rotation) + mean;


        // Apply global transformations that were passed to the function in @param transformation
        // Need to extract rotation and scale without translation
      
    
        transformation.Decompose(out Vector3 t_scale, out Microsoft.Xna.Framework.Quaternion t_rotation, out Vector3 _);

        return new OrientedBoundingBox(
            Vector3.Transform(obbCenter, transformation), 
            extents*new Vector3(Math.Abs(t_scale.X), Math.Abs(t_scale.Y), Math.Abs(t_scale.Z)), 
            rotation*Matrix.CreateFromQuaternion(t_rotation));
    }

}