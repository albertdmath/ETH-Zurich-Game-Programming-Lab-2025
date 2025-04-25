using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace src.GameObjects
{
    public class Market : GameModel
    {
        private const float PROJECTILE_CREATION_TIME = 2f; 
        private const int MAX_PROJECTILES = 5;

        private DrawModel projectileModel;
        private int nProjectiles = 0;
        private float projectileTime = PROJECTILE_CREATION_TIME;

        public ProjectileType Type { get; private set; }

        public Market(Vector3 position, ProjectileType type, DrawModel model, DrawModel projectile, float scaling) : base(model, scaling)
        {
            //this.Position = position;
            //this.Orientation = Vector3.Normalize(-Position);
            this.Type = type;
            this.projectileModel = projectile;
            this.updateHitbox();
        }

        public override void Update(float dt)
        {
            if (nProjectiles == MAX_PROJECTILES) return;
            
            projectileTime -= dt;
            
            if (projectileTime > 0) return;
                
            nProjectiles++;
            projectileTime = PROJECTILE_CREATION_TIME;
        }

        public bool GrabProjectile()
        {
            if (nProjectiles > 0)
            {
                nProjectiles--;
                return true;
            }
            return false;
        }

        public void DrawFish(Matrix view, Matrix projection, Shader shader, bool shadowDraw)
        {
            for(int i = 0; i < nProjectiles; i++)
            {
                CalculateTransform();
                int j = 0; 
                foreach (ModelMesh mesh in DrawModel.model.Meshes)
                {
                    foreach(ModelMeshPart part in mesh.MeshParts){
                        part.Effect = shader.effect; 
                        shader.setWorldMatrix(Transform);
                        
                        if(!shadowDraw)
                            shader.setTexture(this.DrawModel.textures[j]);
                    }
                    j++;
                    mesh.Draw();
                } 
            }
        }
    }
}