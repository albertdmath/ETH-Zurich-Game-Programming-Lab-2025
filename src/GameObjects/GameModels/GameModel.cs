using System;
using System.Collections.Generic;
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

    public bool hasAnimation {get; set;} = false;

    public Animator animator {get; set;} = null;
    public List<GameAnimation> animations {get; set;}
    protected Matrix Scaling;

    public GameModel(DrawModel model, float scale)
    {
        DrawModel = model;
        Scaling = Matrix.CreateScale(scale);
        CalculateTransform();
        Hitbox = new Hitbox(this.DrawModel, Transform);
        this.animations = new List<GameAnimation>();
        if(model.hasAnimations){
            hasAnimation = true; 
            for(int i = 0; i < model.scene.AnimationCount; i++){
                GameAnimation anim = new GameAnimation("Animation " +i, model.scene.Animations[i], model.scene, model);
                animations.Add(anim);
            }
            this.animator = new Animator(animations[0], true);
        }


    }
    public void UpdateScale(float scale){
        Scaling = Matrix.CreateScale(scale);
    }

    public void UpdateAnimation(float dt){
        if(this.animator != null){
            this.animator.UpdateAnimation(dt);
        }
    }

    public Matrix[] GetFinalBoneMatrices(){
        if(this.animator != null){
            return this.animator.finalBoneMatrices;
        }
        return new Matrix[]{}; 
    }


    public void SwitchAnimation(int index, bool loop = false){
        if(this.animator != null && index < animations.Count){
            this.animator.PlayAnimation(animations[index], loop);
        } 
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
            if (!shadowDraw)
            {   
                if(mesh.hasDiffuse){
                    shader.setTexture(mesh.diffuse);
                }
                shader.setNormalMatrix(view, Transform);
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

