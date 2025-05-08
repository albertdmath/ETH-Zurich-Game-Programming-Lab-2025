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

    public GameModel(DrawModel model, float scale, HitboxType hitboxType = HitboxType.OBB)
    {
        DrawModel = model;
        Scaling = Matrix.CreateScale(scale);
        CalculateTransform();
        Hitbox = (hitboxType == HitboxType.OBB) ? new OBB(this.DrawModel, Transform) : new Sphere(Transform);
        this.animations = new List<GameAnimation>();
        if(model.hasAnimations){
            hasAnimation = true; 
            for(int i = 0; i < model.scene.AnimationCount; i++){
                GameAnimation anim = new("Animation " +i, model.scene.Animations[i], model.scene, model);
                animations.Add(anim);
            }
            this.animator = new Animator(animations[0], false);
        }
    }
    public void UpdateScale(float scale){
        Scaling = Matrix.CreateScale(scale,1f,scale);
    }

    public void UpdateAnimation(float dt){
        this.animator?.UpdateAnimation(dt);
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
        Transform = Scaling * Matrix.CreateRotationY(MathF.Atan2(-1f * Orientation.X, -1f * Orientation.Z)) * Matrix.CreateTranslation(Position);
    }
    public virtual void updateWrap(float dt)
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
}

