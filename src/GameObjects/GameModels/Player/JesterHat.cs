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
            Position = player.Position + new Vector3(0f,-0.02f,0f);
            Orientation = player.Orientation;
        }
       

        // Updates the position of the hat
        public override void Update(float dt)
        {
            OnBody();
        }

        // Override draw to account for missing bells when the player loses hp
        public override void Draw(Matrix view, Matrix projection, Shader shader, GraphicsDevice graphicsDevice, bool shadowDraw)
        {
            CalculateTransform();
            int i = 0;
            foreach (GameMesh mesh in DrawModel.meshes)
        {
            graphicsDevice.SetVertexBuffer(mesh.vertexBuffer);
            graphicsDevice.Indices = mesh.indexBuffer;
            shader.setWorldMatrix(Transform);
            if (!shadowDraw && mesh.hasDiffuse)
            {
                shader.setTexture(mesh.diffuse);
            }
                // if(player.Life == 2 && i == 1) continue;
                // if(player.Life == 1 && (i == 1 || i == 2)) continue;
                // if(player.Life == 0 && (i == 1 || i == 2 || i == 3)) continue;
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
            i++;
            }
        }
       
    }
}