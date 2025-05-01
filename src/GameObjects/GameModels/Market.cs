using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace src.GameObjects
{
    public class Market : GameModel
    {
        private const float PROJECTILE_CREATION_TIME = 5f; 
        
        private readonly DrawModel projectileModel;
        private readonly Matrix marketRotTrans;
        private readonly Matrix projectileScale;
        //private static readonly float[] positions = new float[MAX_PROJECTILES] { -0.7f, -0.35f, 0f, 0.35f, 0.7f };
        //this are only the throwable projectiles
        private readonly static Dictionary<ProjectileType, float> projectileScaling = new()
        {
            { ProjectileType.Banana, 0.7f },
            { ProjectileType.Coconut, 0.3f },
            { ProjectileType.Frog, 0.5f },
            { ProjectileType.Swordfish, 0.6f },
            { ProjectileType.Tomato, 0.6f },
            { ProjectileType.Turtle, 0.3f },
            { ProjectileType.Spear, 0.5f },
            { ProjectileType.Mjoelnir, 0.9f },
            { ProjectileType.Chicken, 0.8f },
            { ProjectileType.Barrel, 0.5f }
        };

        private bool projectilesAvailable;
        private float projectileTime = 0f;

        public ProjectileType Type { get; private set; }

        public Market(Vector3 position, ProjectileType type, DrawModel model, DrawModel projectile, float scaling) : base(model, scaling)
        {
            this.Position = position;
            this.Orientation = Vector3.Normalize(-Position);
            this.Type = type;
            this.projectileModel = projectile;
            this.marketRotTrans = Matrix.CreateRotationY((float)Math.Atan2(-Orientation.X, -Orientation.Z))
                                * Matrix.CreateRotationY(MathHelper.ToRadians(90))
                                * Matrix.CreateTranslation(Position);
            this.projectileScale = Matrix.CreateScale(projectileScaling[type]);
            this.updateHitbox();
        }

        public override void Update(float dt)
        {   
            projectileTime -= dt;
            projectilesAvailable = projectileTime <= 0f;
        }

        public bool GrabProjectile()
        {
            if(projectilesAvailable)
            {
                projectileTime = PROJECTILE_CREATION_TIME;
                return true;
            }
            return false;
        }

        public void DrawFish(Matrix view, GraphicsDevice graphicsDevice, Shader shader, bool shadowDraw)
        {  
            if(!projectilesAvailable) return;
            
            Matrix projectileTranslation =  Matrix.CreateTranslation(0, 0.5f, 0.15f);

            Matrix finalTransform = projectileScale * projectileTranslation * marketRotTrans;

            foreach (GameMesh mesh in projectileModel.meshes)
            {   
                VertexBuffer buff = mesh.vertexBuffer;
                graphicsDevice.SetVertexBuffer(buff);
                graphicsDevice.Indices = mesh.indexBuffer;
                shader.setWorldMatrix(finalTransform);
                if (!shadowDraw)
                {   
                    if(mesh.hasDiffuse){
                        shader.setTexture(mesh.diffuse);
                    }
                    shader.setNormalMatrix(view, finalTransform);
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
}