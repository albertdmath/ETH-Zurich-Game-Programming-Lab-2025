using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Vector3 = Microsoft.Xna.Framework.Vector3;
using Matrix = Microsoft.Xna.Framework.Matrix;
using Accord.Math.Decompositions;
using Microsoft.Xna.Framework;
public class OrientedBoundingBox
{

    public Vector3 Center { get; private set; }
    public Vector3 Extents { get; private set; }
    public Matrix Orientation { get; private set; }
    public Vector3 CenterCentered { get; private set; }
    public Vector3 ExtentsCentered { get; private set; }
    public Matrix OrientationCentered { get; private set; }
    public OrientedBoundingBox(Vector3 center, Vector3 extents, Matrix orientation)
    {
        Center = center;
        Extents = extents;
        Orientation = orientation;
        CenterCentered = center;
        ExtentsCentered = extents;
        OrientationCentered = orientation;
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

    public void Transform(Matrix transformation) {
        // Apply global transformations that were passed to the function in @param transformation
        // We extract scaling and rotation so we can apply them to the extents / rotation individually
        transformation.Decompose(out Vector3 t_scale, out Microsoft.Xna.Framework.Quaternion t_rotation, out Vector3 _);

        Center = Vector3.Transform(CenterCentered, transformation);
        Extents = ExtentsCentered* new Vector3(Math.Abs(t_scale.X), Math.Abs(t_scale.Y), Math.Abs(t_scale.Z));
        Orientation = OrientationCentered*Matrix.CreateFromQuaternion(t_rotation);
    }

    /* 
        This is a little complex, basically it finds the center of a mesh and 
        then does PCA to find the orientation of the most significant 3 axis in the point cloud
        Probs to a few parts of this go to GPT who helped me put the individual parts together, 
        the Accord library does a very good job at eigendecomposition;
        it also computes the halfextents (so the distances from the center to the border of the box)
        Center, (half)extents and orientation form the OBB
        If I ever come back here to steal this genius piece of code: hey, you can do it! :)
    */
    public static OrientedBoundingBox ComputeOBB(GameMesh mesh, Matrix transformation)
    {
        List<GameModelVertex> vertices = mesh.vertices;
        if (vertices == null || vertices.Count == 0)
            throw new ArgumentException("No vertices provided!");

        // Compute mean (center of mass)
        Vector3 mean = Vector3.Zero;
        foreach (GameModelVertex v in vertices)
            mean += v.Position;
        mean /= vertices.Count;

        // Construct 3x3 covariance matrix
        double[,] covarianceMatrix = new double[3, 3];

        foreach (GameModelVertex v in vertices)
        {
            Vector3 d = v.Position - mean;
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

        // Create Xna rotation matrix
        Matrix rotation = new Matrix(
            axes[0].X, axes[0].Y, axes[0].Z, 0,
            axes[1].X, axes[1].Y, axes[1].Z, 0,
            axes[2].X, axes[2].Y, axes[2].Z, 0,
            0, 0, 0, 1
        );

        // Transform vertices into the new basis
        List<Vector3> transformed = new List<Vector3>();
        Matrix rotationTranspose = Matrix.Transpose(rotation);
        foreach (GameModelVertex v in vertices)
            transformed.Add(Vector3.Transform(v.Position - mean, rotationTranspose));

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


        OrientedBoundingBox OBB = new OrientedBoundingBox(
            obbCenter, 
            extents, 
            rotation
        );
        // Apply global transformations
        OBB.Transform(transformation);
        return OBB;
    }
    
 
    /*
        This function again is kinda complex, thanks to claude for some of the math. Basically it uses the separating axis theorem
        to see if we can fit a plane between this box and the other one. If we can they dont intersect and if we can't they don't.
    */
    public bool Intersects(OrientedBoundingBox other)
    {
        // Get the axes of both boxes (the columns of the orientation matrices)
        Vector3[] thisAxes = new Vector3[3]
        {
            new Vector3(Orientation.M11, Orientation.M12, Orientation.M13),
            new Vector3(Orientation.M21, Orientation.M22, Orientation.M23),
            new Vector3(Orientation.M31, Orientation.M32, Orientation.M33)
        };

        Vector3[] otherAxes = new Vector3[3]
        {
            new Vector3(other.Orientation.M11, other.Orientation.M12, other.Orientation.M13),
            new Vector3(other.Orientation.M21, other.Orientation.M22, other.Orientation.M23),
            new Vector3(other.Orientation.M31, other.Orientation.M32, other.Orientation.M33)
        };

        // Normalize the axes
        for (int i = 0; i < 3; i++)
        {
            thisAxes[i].Normalize();
            otherAxes[i].Normalize();
        }

        // Vector from center of this box to center of other box
        Vector3 toCenter = other.Center - Center;

        // Check this box's axes (face normals)
        for (int i = 0; i < 3; i++)
        {
            if (!OverlapOnAxis(thisAxes[i], toCenter, thisAxes, otherAxes, this.Extents, other.Extents))
                return false;
        }

        // Check other box's axes (face normals)
        for (int i = 0; i < 3; i++)
        {
            if (!OverlapOnAxis(otherAxes[i], toCenter, thisAxes, otherAxes, this.Extents, other.Extents))
                return false;
        }

        // Check axes formed by cross products of edges (9 potential separating axes)
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Vector3 crossAxis = Vector3.Cross(thisAxes[i], otherAxes[j]);
                
                // If the cross product is near zero, the edges are parallel and this is not a separating axis
                if (crossAxis.LengthSquared() < 0.0001f)
                    continue;
                
                crossAxis.Normalize();
                
                if (!OverlapOnAxis(crossAxis, toCenter, thisAxes, otherAxes, this.Extents, other.Extents))
                    return false;
            }
        }

        // No separating axis found, boxes must intersect
        return true;
    }

    public bool Intersects(BoundingSphere other)
    {
        // Step 1: Transform the sphere's center into the OBB's local space
        Vector3 sphereCenterLocal = other.Center - this.Center;
        sphereCenterLocal = Vector3.Transform(sphereCenterLocal, Matrix.Transpose(this.Orientation)); // Inverse of rotation matrix is its transpose

        // Step 2: Clamp the sphere's center to the OBB's extents
        Vector3 clampedCenter = new Vector3(
            MathHelper.Clamp(sphereCenterLocal.X, -this.Extents.X, this.Extents.X),
            MathHelper.Clamp(sphereCenterLocal.Y, -this.Extents.Y, this.Extents.Y),
            MathHelper.Clamp(sphereCenterLocal.Z, -this.Extents.Z, this.Extents.Z)
        );

        // Step 3: Calculate the squared distance between the sphere's center and the clamped point
        float distanceSquared = Vector3.DistanceSquared(sphereCenterLocal, clampedCenter);

        // Step 4: Check intersection
        return distanceSquared <= other.Radius * other.Radius;
    }

    
    // Does the axis tests yessir
    private bool OverlapOnAxis(Vector3 axis, Vector3 toCenter, Vector3[] thisAxes, Vector3[] otherAxes, Vector3 thisExtents, Vector3 otherExtents)
    {
        // Project the half-sizes of each box onto the axis
        float thisProjection = 
            Math.Abs(Vector3.Dot(axis, thisAxes[0]) * thisExtents.X) +
            Math.Abs(Vector3.Dot(axis, thisAxes[1]) * thisExtents.Y) +
            Math.Abs(Vector3.Dot(axis, thisAxes[2]) * thisExtents.Z);
            
        float otherProjection = 
            Math.Abs(Vector3.Dot(axis, otherAxes[0]) * otherExtents.X) +
            Math.Abs(Vector3.Dot(axis, otherAxes[1]) * otherExtents.Y) +
            Math.Abs(Vector3.Dot(axis, otherAxes[2]) * otherExtents.Z);

        // Project the center-to-center vector onto the axis
        float distance = Math.Abs(Vector3.Dot(toCenter, axis));

        // If the sum of the projections is less than the distance between centers, 
        // then there is a separating axis and the boxes do not intersect
        return distance <= thisProjection + otherProjection;
    }
}