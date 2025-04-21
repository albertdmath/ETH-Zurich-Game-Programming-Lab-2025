using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace src.GameObjects;

/** Superclass for all moving objects **/
public class GameModel
{
    public Vector3 Position { get; set; }
    public Vector3 Orientation { get; set; }
    public DrawModel DrawModel { get; set; }
    public Hitbox Hitbox { get; set; }
    public Matrix Transform { get; set; }
    protected Matrix Scaling;

    public GameModel(DrawModel model, float scale)
    {
        DrawModel = model;
        Scaling = Matrix.CreateScale(scale);
        CalculateTransform();
        Hitbox = new Hitbox(this.DrawModel.model, Transform);

    }

    protected void CalculateTransform()
    {
        Transform = Scaling * Matrix.CreateRotationY((float)Math.Atan2(-1f * Orientation.X, -1f * Orientation.Z)) * Matrix.CreateTranslation(Position);
    }
    public void updateWrap(float dt)
    {
        Update(dt);
        updateHitbox();
    }
    public void updateHitbox()
    {
        CalculateTransform();
        Hitbox.Transform(Transform);
    }
    public virtual void Update(float dt) { }

    public virtual void Draw(Matrix view, Matrix projection, Shader shader, GraphicsDevice graphicsDevice, bool shadowDraw)
    {
        CalculateTransform();
        foreach (GameMesh mesh in DrawModel.meshes)
        {   
            VertexBuffer buff = mesh.vertexBuffer;
            graphicsDevice.SetVertexBuffer(buff);
            graphicsDevice.Indices = mesh.indexBuffer;
            shader.setWorldMatrix(Transform);
            if (!shadowDraw && mesh.hasDiffuse)
            {
                shader.setTexture(mesh.diffuse);
            }
            foreach (var pass in shader.effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                graphicsDevice.DrawIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    baseVertex: 0,
                    startIndex: 0,
                    primitiveCount: mesh.indices.Count / 3
                );

               
            }
        }
    }

    
    // public virtual void Draw(Matrix view, Matrix projection, Shader shader, GraphicsDevice graphicsDevice, bool shadowDraw){
    //     CalculateTransform();
    //     int i = 0; 
    //     foreach (ModelMesh mesh in DrawModel.model.Meshes)
    //     {
    //         foreach(ModelMeshPart part in mesh.MeshParts){
    //             part.Effect = shader.effect; 
    //            shader.setWorldMatrix(Transform);

    //             if(!shadowDraw){
    //             shader.setTexture(this.DrawModel.textures[i]);
    //             }
    //         }
    //         i++;
    //         mesh.Draw();
    //     }
    // }
}

