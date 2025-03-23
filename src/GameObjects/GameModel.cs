using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace src.GameObjects;

/** Superclass for all moving objects **/
public class GameModel {
    public Vector3 Position { get; set; }
    public Vector3 Orientation { get; set; }
    protected DrawModel drawModel { get; set; }
    public Hitbox Hitbox { get; set; }

    public Matrix Transform { get; set; }
    protected Matrix Scaling;

    public GameModel(DrawModel model,float scale) {
        this.drawModel = model;
        Scaling = Matrix.CreateScale(scale);
        CalculateTransform();
        Hitbox = new Hitbox(this.drawModel.model,Transform);

    }

    protected void CalculateTransform(){
        Transform = Scaling * Matrix.CreateRotationY((float)Math.Atan2(-1f*Orientation.X,-1f*Orientation.Z))* Matrix.CreateTranslation(Position);
    }
    public void updateWrap(float dt){
        Update(dt);
        updateHitbox();
    }
    public void updateHitbox(){
        CalculateTransform();
        Hitbox.Transform(Transform);
    }
    public virtual void Update(float dt){}


    public virtual void Draw(Matrix view, Matrix projection, Shader shader, bool shadowDraw){
        CalculateTransform();
        int i = 0; 
        foreach (ModelMesh mesh in drawModel.model.Meshes)
            {
               foreach(ModelMeshPart part in mesh.MeshParts){
                    part.Effect = shader.effect; 
                    shader.setWorldMatrix(Transform);
                    shader.setTexture(this.drawModel.textures[i]);
               }
               i++;
            mesh.Draw();
            }
    }
}
