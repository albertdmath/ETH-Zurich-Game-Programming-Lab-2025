using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace src.GameObjects
{
    // Jester hat class takes care of losing bells upon hits
    public class JesterHat : GameModel
    {
        private Player player;
        
        // Need player reference to check their HP
        public JesterHat(Player player, DrawModel model, float scale) : base(model,scale)
        {
            this.player=player;
            OnBody();
        }

       
        // Places hat on head
        private void OnBody()
        {
            Position = player.Position;
            Orientation = player.Orientation;
        }
       

        // Updates the position of the hat
        public override void Update(float dt)
        {
            OnBody();
        }

        // Override draw to account for missing bells when the player loses hp
        public override void Draw(Matrix view, Matrix projection, Shader shader, bool shadowDraw)
        {
            CalculateTransform();
            int i = 0; 
            foreach (ModelMesh mesh in DrawModel.model.Meshes)
            {
                foreach(ModelMeshPart part in mesh.MeshParts){
                    part.Effect = shader.effect; 
                    shader.setWorldMatrix(Transform);
                    
                    if(!shadowDraw){
                    shader.setTexture(this.DrawModel.textures[i]);
                    }
                }
                i++;
                if(player.Life == 2 && i == 1) continue;
                if(player.Life == 1 && (i == 1 || i == 2)) continue;
                if(player.Life == 0 && (i == 1 || i == 2 || i == 3)) continue;
                mesh.Draw();
            }
        }
       
    }
}