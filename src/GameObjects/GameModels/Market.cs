using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace src.GameObjects
{
    public class Market : GameModel
    {
        private const float PROJECTILE_CREATION_TIME = 2f; 
        private const int MAX_PROJECTILES = 5;
        private readonly DrawModel projectileModel;
        private readonly float[] positions = new float[MAX_PROJECTILES] { -0.5f, -0.25f, 0f, 0.25f, 0.5f };
        //this are only the throwable projectiles
        private readonly Dictionary<ProjectileType, float> projectileScaling = new Dictionary<ProjectileType, float>()
        {
            { ProjectileType.Banana, 0.5f},
            { ProjectileType.Coconut, 0.3f},
            { ProjectileType.Swordfish, 0.5f},
            { ProjectileType.Tomato, 0.5f}
        };

        private int nProjectiles = 0;
        private float projectileTime = PROJECTILE_CREATION_TIME;

        public ProjectileType Type { get; private set; }

        public Market(Vector3 position, ProjectileType type, DrawModel model, DrawModel projectile, float scaling) : base(model, scaling)
        {
            this.Position = position;
            this.Orientation = Vector3.Normalize(-Position);
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

        public void DrawFish(Shader shader, bool shadowDraw)
        {   
            // Base transform (scale + rotation + position)
            Matrix marketRot = Matrix.CreateRotationY((float)Math.Atan2(-Orientation.X, -Orientation.Z));
            Matrix marketTranslation = Matrix.CreateTranslation(Position);
            Matrix projectileScale = Matrix.CreateScale(projectileScaling[Type]);

            for (int i = 0; i < nProjectiles; i++)
            {
                Matrix projectileTranslation =  Matrix.CreateTranslation(positions[i], 0.5f, 0f);

                shader.setWorldMatrix(projectileScale * 
                                      projectileTranslation * 
                                      marketRot * 
                                      marketTranslation);

                for (int j = 0; j < projectileModel.model.Meshes.Count; j++)
                {
                    ModelMesh mesh = projectileModel.model.Meshes[j];
                    foreach(ModelMeshPart part in mesh.MeshParts)
                    {
                        part.Effect = shader.effect; 
                        if(!shadowDraw)
                            shader.setTexture(projectileModel.textures[j]);
                    }
                    mesh.Draw();
                }  
            }
        }
    }
}