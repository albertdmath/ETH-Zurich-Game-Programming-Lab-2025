using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

public class DrawModel {

    public Model model {get; set;} 

    public List<Texture2D> textures {get;}



    public DrawModel(Model model){
        this.model = model;
        this.textures = new List<Texture2D>();
        extractTextures();
    }


  private void extractTextures(){
        foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    if(effect.Texture != null){
                        this.textures.Add(effect.Texture);
                    }
                }
            }
    }

}