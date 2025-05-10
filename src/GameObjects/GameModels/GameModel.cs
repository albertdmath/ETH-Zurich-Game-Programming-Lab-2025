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
    public Matrix Scaling { get; set; }

    public bool hasAnimation {get; set;} = false;

    public Animator animator {get; set;} = null;
    public List<GameAnimation> animations {get; set;}

    public GameModel(DrawModel model, float scale, float radius = -1)
    {
        DrawModel = model;
        Scaling = Matrix.CreateScale(scale);
        CalculateTransform();
        Hitbox = (radius == -1) ? new OBB(this.DrawModel, Transform) : new Sphere(Position, radius);
        
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
        Transform = Scaling * Matrix.CreateRotationY(MathF.Atan2(-Orientation.X, -Orientation.Z)) * Matrix.CreateTranslation(Position);
    }

    public virtual void updateWrap(float dt)
    {
        Update(dt);
        updateHitbox();
    }
    public void updateHitbox()
    {
        if (Hitbox is OBB obb)
        {
            CalculateTransform();
            obb.Transform(Transform);
        }
        else if (Hitbox is Sphere sphere)
        {
            sphere.Transform(Position);
        }
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

