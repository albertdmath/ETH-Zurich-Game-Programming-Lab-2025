using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace src.GameObjects;

/** Superclass for all moving objects **/
public class GameModel {
    public Vector3 Position { get; set; }
    public Vector3 Orientation { get; set; }
    protected Model Model { get; set; }
    public Hitbox Hitbox { get; set; }

    public Matrix Transform { get; set; }
    public GameModel(Model model) {
        Model = model;
        CalculateTransform();
        Hitbox = new Hitbox(Model,Transform);
    }

    protected void CalculateTransform(){
        Transform = Matrix.CreateRotationY((float)Math.Atan2(-1f*Orientation.X,-1f*Orientation.Z))* Matrix.CreateTranslation(Position);
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

    public void Draw(Matrix view, Matrix projection){
        foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = Transform;
                    effect.View = view;
                    effect.Projection = projection;
                    effect.EnableDefaultLighting();
                }
                mesh.Draw();
            }
    }
}
